using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Estado caido y revivir por proximidad (E3, #68). Al llegar a 0 de vida el
/// jugador queda caido: no se mueve ni ataca (lo respetan PlayerController y
/// AtaqueEspada) y no recibe mas dano (lo respeta SaludJugador). Un companero
/// lo revive quedandose cerca 3 s seguidos; si se aleja, el conteo se reinicia.
/// Al revivir vuelve con la mitad de la vida.
///
/// Autoridad de red: el HOST decide quien esta caido, mide la proximidad y
/// confirma el revivir; los clientes solo ven el estado y el progreso
/// replicados (NetworkVariable). Nadie puede auto-revivirse. Si TODOS quedan
/// caidos, el host emite el aviso de derrota (la pantalla es de la E5).
/// </summary>
[RequireComponent(typeof(SaludJugador))]
public class EstadoCaido : NetworkBehaviour
{
    /// <summary>El host detecto que todos los jugadores estan caidos (derrota).</summary>
    public static event Action AlCaerTodosLosJugadores;

    [Header("Revivir (visible desde el Inspector)")]
    [Tooltip("Distancia maxima a la que un companero puede revivir.")]
    [SerializeField] private float distanciaRevivir = 2f;

    [Tooltip("Segundos seguidos que hay que quedarse cerca para revivir.")]
    [SerializeField] private float tiempoRevivir = 3f;

    [Header("Feedback visual (opcional, asignar en el prefab)")]
    [Tooltip("Barra de progreso del revivir; se escala en X de 0 a 1. Hijo del jugador.")]
    [SerializeField] private SpriteRenderer barraProgreso;

    [Tooltip("Color con el que se tinta el sprite mientras esta caido.")]
    [SerializeField] private Color tinteCaido = new Color(0.5f, 0.5f, 0.5f, 1f);

    // Todos los jugadores vivos en la escena (para medir proximidad en el host).
    private static readonly List<EstadoCaido> Todos = new List<EstadoCaido>();

    /// <summary>Todos los jugadores en escena (solo lectura); lo usa el enemigo para buscar objetivo (#69).</summary>
    public static IReadOnlyList<EstadoCaido> Jugadores => Todos;

    // Caido y progreso los escribe solo el host; todos los leen.
    private readonly NetworkVariable<bool> caido = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<float> progreso = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private SaludJugador salud;
    private SpriteRenderer sprite;
    private Color colorOriginal = Color.white;
    private float tiempoAcumulado;

    /// <summary>True si el jugador esta caido (lo consultan PlayerController, AtaqueEspada y SaludJugador).</summary>
    public bool EstaCaido => caido.Value;

    private void Awake()
    {
        salud = GetComponent<SaludJugador>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            colorOriginal = sprite.color;
        }
        if (barraProgreso != null)
        {
            barraProgreso.gameObject.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        Todos.Add(this);
        caido.OnValueChanged += AlCambiarCaido;
        progreso.OnValueChanged += AlCambiarProgreso;

        // Estado inicial ya sincronizado (por si aparece con red en curso).
        AplicarVisualCaido(caido.Value);
        AplicarBarra(caido.Value, progreso.Value);

        // El host reacciona a la caida (0 de vida) para marcar el estado.
        if (IsServer)
        {
            salud.AlCaerServidor += CaerEnServidor;
        }
    }

    public override void OnNetworkDespawn()
    {
        Todos.Remove(this);
        caido.OnValueChanged -= AlCambiarCaido;
        progreso.OnValueChanged -= AlCambiarProgreso;
        if (IsServer && salud != null)
        {
            salud.AlCaerServidor -= CaerEnServidor;
        }
    }

    // Corre solo en el host: mide la proximidad de un companero y cuenta el revivir.
    private void Update()
    {
        if (!IsServer) return;
        if (!caido.Value) return;

        if (HayCompaneroCerca())
        {
            tiempoAcumulado += Time.deltaTime;
            progreso.Value = Mathf.Clamp01(tiempoAcumulado / tiempoRevivir);
            if (tiempoAcumulado >= tiempoRevivir)
            {
                RevivirEnServidor();
            }
        }
        else if (tiempoAcumulado > 0f)
        {
            // El companero se alejo antes de completar: se reinicia el conteo.
            tiempoAcumulado = 0f;
            progreso.Value = 0f;
        }
    }

    // Marca al jugador como caido (host). Lo llama SaludJugador al llegar a 0.
    private void CaerEnServidor()
    {
        if (caido.Value) return;

        caido.Value = true;
        progreso.Value = 0f;
        tiempoAcumulado = 0f;

        RevisarDerrota();
    }

    private void RevivirEnServidor()
    {
        caido.Value = false;
        progreso.Value = 0f;
        tiempoAcumulado = 0f;
        salud.RevivirServidor();
    }

    // Un companero (otro jugador, no caido) dentro de la distancia de revivir.
    private bool HayCompaneroCerca()
    {
        foreach (EstadoCaido otro in Todos)
        {
            if (otro == this) continue;      // nadie se auto-revive
            if (otro.caido.Value) continue;   // un caido no puede revivir
            if (Vector2.Distance(transform.position, otro.transform.position) <= distanciaRevivir)
            {
                return true;
            }
        }
        return false;
    }

    // Si todos los jugadores estan caidos, avisa derrota (jugando solo, cae directo).
    private void RevisarDerrota()
    {
        foreach (EstadoCaido e in Todos)
        {
            if (!e.caido.Value) return;
        }
        AlCaerTodosLosJugadores?.Invoke();
    }

    private void AlCambiarCaido(bool antes, bool ahora)
    {
        AplicarVisualCaido(ahora);
        AplicarBarra(ahora, progreso.Value);
    }

    private void AlCambiarProgreso(float antes, float ahora)
    {
        AplicarBarra(caido.Value, ahora);
    }

    // Tinta el sprite mientras esta caido (marca visible aunque no haya animacion aun).
    private void AplicarVisualCaido(bool estaCaido)
    {
        if (sprite == null) return;
        sprite.color = estaCaido ? tinteCaido : colorOriginal;
    }

    // Muestra la barra de progreso del revivir (visible para los dos jugadores).
    private void AplicarBarra(bool estaCaido, float valor)
    {
        if (barraProgreso == null) return;

        bool visible = estaCaido && valor > 0f;
        barraProgreso.gameObject.SetActive(visible);
        if (visible)
        {
            Vector3 escala = barraProgreso.transform.localScale;
            escala.x = valor;
            barraProgreso.transform.localScale = escala;
        }
    }
}
