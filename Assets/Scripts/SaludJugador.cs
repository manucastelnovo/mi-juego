using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Vida del jugador en red (E3, #66). La vida es una NetworkVariable que
/// solo escribe el host: el host resuelve el dano y los clientes solo ven
/// bajar el numero y el parpadeo. Cada dispositivo avisa por evento estatico
/// la vida de SU jugador local, para que el HUD (BarraVidaHUD) escuche sin
/// conocer al jugador, igual que ContadorMonedas escucha a Moneda.AlRecoger.
///
/// Los ataques (espada #65, enemigos #69) llaman a AplicarDanio en el host.
/// El estado "caido" al llegar a 0 vive en su propia historia (#68): aca solo
/// se emite el aviso AlQuedarSinVida.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SaludJugador : NetworkBehaviour
{
    // === Eventos para el HUD del jugador local (patron ContadorMonedas) ===
    // El HUD no conoce al jugador: se suscribe a estos eventos estaticos.
    // Solo se disparan para el jugador local (IsOwner) de este dispositivo,
    // asi cada pantalla muestra su propia vida.

    /// <summary>Vida del jugador local cambio: (vidaActual, vidaMaxima).</summary>
    public static event Action<int, int> AlCambiarVidaLocal;

    /// <summary>El jugador local llego a 0 de vida (el estado caido va en #68).</summary>
    public static event Action AlQuedarSinVida;

    /// <summary>Solo en el host: el jugador llego a 0 de vida. Lo usa EstadoCaido (#68) para marcar el caido.</summary>
    public event Action AlCaerServidor;

    /// <summary>La vida de ESTE jugador cambio; se dispara en todos los peers. Lo usa la barra sobre companeros (#67).</summary>
    public event Action<int, int> AlCambiarVidaReplicada;

    [Header("Balance (visible desde el Inspector)")]
    [Tooltip("Puntos de vida al aparecer y tope maximo.")]
    [SerializeField] private int vidaMaxima = 5;

    [Tooltip("Segundos de invulnerabilidad con parpadeo tras recibir dano.")]
    [SerializeField] private float segundosInvulnerable = 1f;

    [Tooltip("Cada cuantos segundos alterna el parpadeo durante la invulnerabilidad.")]
    [SerializeField] private float intervaloParpadeo = 0.1f;

    // Vida actual: la escribe solo el host; todos la leen para ver la barra bajar.
    private readonly NetworkVariable<int> vidaActual = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private SpriteRenderer spriteRenderer;
    private EstadoCaido estadoCaido;
    private Coroutine rutinaParpadeo;

    // Solo en el host: momento (Time.time) hasta el que el jugador es invulnerable.
    private float invulnerableHasta;

    /// <summary>Puntos de vida actuales (lectura para otras historias, p. ej. la barra sobre companeros #67).</summary>
    public int VidaActual => vidaActual.Value;

    /// <summary>Vida maxima configurada en el Inspector.</summary>
    public int VidaMaxima => vidaMaxima;

    /// <summary>True mientras el jugador no tiene vida (lo usara la historia de caido #68).</summary>
    public bool SinVida => vidaActual.Value <= 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        estadoCaido = GetComponent<EstadoCaido>();
    }

    public override void OnNetworkSpawn()
    {
        // El host fija la vida inicial una sola vez; los clientes la reciben.
        if (IsServer)
        {
            vidaActual.Value = vidaMaxima;
        }

        vidaActual.OnValueChanged += AlCambiarVida;

        // El HUD local arranca con el valor ya sincronizado.
        if (IsOwner)
        {
            AlCambiarVidaLocal?.Invoke(vidaActual.Value, vidaMaxima);
        }
    }

    public override void OnNetworkDespawn()
    {
        vidaActual.OnValueChanged -= AlCambiarVida;
    }

    /// <summary>
    /// Aplica dano al jugador. SOLO tiene efecto en el host (autoridad): el
    /// cliente pide el ataque, pero el dano lo resuelve el host. Respeta la
    /// invulnerabilidad y no deja bajar la vida de 0.
    /// </summary>
    public void AplicarDanio(int cantidad)
    {
        if (!IsServer) return;
        if (cantidad <= 0) return;
        if (vidaActual.Value <= 0) return;

        // Un jugador caido no recibe mas dano (#68).
        if (estadoCaido != null && estadoCaido.EstaCaido) return;

        // Durante la ventana de invulnerabilidad, los golpes no restan.
        if (Time.time < invulnerableHasta) return;

        vidaActual.Value = Mathf.Max(0, vidaActual.Value - cantidad);
        invulnerableHasta = Time.time + segundosInvulnerable;
    }

    // Corre en todos los clientes cuando cambia la vida: baja la barra local,
    // parpadea si bajo la vida y avisa si quedo en 0.
    private void AlCambiarVida(int anterior, int nueva)
    {
        // Se replica en todos los peers para la barra sobre companeros (#67).
        AlCambiarVidaReplicada?.Invoke(nueva, vidaMaxima);

        if (IsOwner)
        {
            AlCambiarVidaLocal?.Invoke(nueva, vidaMaxima);
        }

        // Un golpe (la vida bajo) dispara el parpadeo en todas las pantallas.
        if (nueva < anterior)
        {
            IniciarParpadeo();
        }

        if (nueva <= 0 && anterior > 0)
        {
            // El dueno avisa a su HUD; el host marca el estado caido (#68).
            if (IsOwner) AlQuedarSinVida?.Invoke();
            if (IsServer) AlCaerServidor?.Invoke();
        }
    }

    /// <summary>
    /// Revive al jugador con la mitad de la vida (E3, #68). SOLO en el host.
    /// Lo llama EstadoCaido cuando un companero completa el revivir.
    /// </summary>
    public void RevivirServidor()
    {
        if (!IsServer) return;

        // Mitad de la vida maxima, redondeando hacia arriba (5 -> 3).
        vidaActual.Value = Mathf.Max(1, Mathf.CeilToInt(vidaMaxima / 2f));
        invulnerableHasta = Time.time + segundosInvulnerable;
    }

    private void IniciarParpadeo()
    {
        if (spriteRenderer == null) return;

        if (rutinaParpadeo != null)
        {
            StopCoroutine(rutinaParpadeo);
        }
        rutinaParpadeo = StartCoroutine(Parpadear());
    }

    private IEnumerator Parpadear()
    {
        float fin = Time.time + segundosInvulnerable;
        bool visible = false;
        while (Time.time < fin)
        {
            spriteRenderer.enabled = visible;
            visible = !visible;
            yield return new WaitForSeconds(intervaloParpadeo);
        }
        spriteRenderer.enabled = true;
        rutinaParpadeo = null;
    }
}
