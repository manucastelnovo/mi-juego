using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Vida de un enemigo en red (E3, #69). La vida es una NetworkVariable que
/// solo escribe el host: el host aplica el dano y decide la muerte; los
/// clientes solo ven el feedback y el despawn. Al llegar a 0 el enemigo se
/// despawnea limpio y su muerte se ve a la vez en todas las pantallas.
/// No busca sprites por nombre: usa el SpriteRenderer que tenga el prefab.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SaludEnemigo : NetworkBehaviour
{
    [Header("Balance (visible desde el Inspector)")]
    [Tooltip("Vida del enemigo. 1 = muere de un solo golpe.")]
    [SerializeField] private int vidaMaxima = 1;

    [Tooltip("Cada cuantos segundos alterna el parpadeo del feedback de golpe.")]
    [SerializeField] private float intervaloParpadeo = 0.05f;

    [Tooltip("Cuanto dura el destello de muerte antes de despawnear.")]
    [SerializeField] private float duracionMuerte = 0.12f;

    [Tooltip("Color del destello al recibir un golpe / morir.")]
    [SerializeField] private Color colorGolpe = Color.red;

    private readonly NetworkVariable<int> vida = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private SpriteRenderer sprite;
    private Color colorOriginal = Color.white;
    private Coroutine rutinaParpadeo;
    private bool muriendo;

    /// <summary>True cuando el enemigo ya no tiene vida.</summary>
    public bool Muerto => vida.Value <= 0;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        colorOriginal = sprite.color;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            vida.Value = vidaMaxima;
        }
        vida.OnValueChanged += AlCambiarVida;
    }

    public override void OnNetworkDespawn()
    {
        vida.OnValueChanged -= AlCambiarVida;
    }

    /// <summary>
    /// Aplica dano al enemigo. SOLO en el host: el cliente pide el ataque
    /// (espada), pero el dano y la muerte los resuelve el host.
    /// </summary>
    public void AplicarDanio(int cantidad)
    {
        if (!IsServer) return;
        if (cantidad <= 0) return;
        if (vida.Value <= 0) return;

        vida.Value = Mathf.Max(0, vida.Value - cantidad);
        if (vida.Value <= 0)
        {
            Morir();
        }
    }

    // Host: apaga el dano/movimiento, avisa el golpe a todos y despawnea limpio.
    private void Morir()
    {
        if (muriendo) return;
        muriendo = true;

        var contacto = GetComponent<DanioPorContacto>();
        if (contacto != null) contacto.enabled = false;
        var rastrero = GetComponent<EnemigoRastrero>();
        if (rastrero != null) rastrero.enabled = false;

        MostrarGolpeClientRpc();
        Invoke(nameof(DespawnFinal), duracionMuerte);
    }

    private void DespawnFinal()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
    }

    private void AlCambiarVida(int anterior, int nueva)
    {
        // Un golpe que no mata: parpadeo de feedback en todas las pantallas.
        if (nueva < anterior && nueva > 0)
        {
            IniciarParpadeo();
        }
    }

    [ClientRpc]
    private void MostrarGolpeClientRpc()
    {
        // Destello de muerte visible en todos los clientes antes del despawn.
        if (sprite != null) sprite.color = colorGolpe;
    }

    private void IniciarParpadeo()
    {
        if (sprite == null) return;
        if (rutinaParpadeo != null) StopCoroutine(rutinaParpadeo);
        rutinaParpadeo = StartCoroutine(Parpadear());
    }

    private IEnumerator Parpadear()
    {
        sprite.color = colorGolpe;
        yield return new WaitForSeconds(intervaloParpadeo);
        sprite.color = colorOriginal;
        rutinaParpadeo = null;
    }
}
