using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Barra chica de vida sobre la cabeza de los COMPANEROS (E3, #67). Cada
/// cliente la muestra sobre los demas jugadores, nunca sobre el suyo (el
/// jugador local ya tiene la barra del HUD). Es solo lectura: se alimenta del
/// mismo dato de red de vida (SaludJugador), sin logica de dano propia. El
/// host es el unico que cambia la vida; aca solo se refleja. Si el jugador se
/// desconecta, su barra se va con el (es hija de su objeto de red).
/// </summary>
[RequireComponent(typeof(SaludJugador))]
public class BarraVidaCompanero : NetworkBehaviour
{
    [Header("Referencias (hijos sobre la cabeza)")]
    [Tooltip("Raiz de la barra; se oculta sobre el jugador local.")]
    [SerializeField] private GameObject contenedor;

    [Tooltip("Sprite de relleno; se escala en X de 0 a 1 (pivot a la izquierda).")]
    [SerializeField] private SpriteRenderer relleno;

    [Header("Colores")]
    [SerializeField] private Color colorVida = new Color(0.3f, 0.85f, 0.3f, 1f);
    [Tooltip("Color cuando el companero esta caido (0 de vida).")]
    [SerializeField] private Color colorCaido = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    private SaludJugador salud;

    private void Awake()
    {
        salud = GetComponent<SaludJugador>();
    }

    public override void OnNetworkSpawn()
    {
        // El jugador local no ve barra sobre su propia cabeza.
        if (IsOwner)
        {
            if (contenedor != null) contenedor.SetActive(false);
            enabled = false;
            return;
        }

        if (contenedor != null) contenedor.SetActive(true);

        // Se lee el valor ya sincronizado y se escucha cada cambio (solo lectura).
        salud.AlCambiarVidaReplicada += Refrescar;
        Refrescar(salud.VidaActual, salud.VidaMaxima);
    }

    public override void OnNetworkDespawn()
    {
        if (salud != null)
        {
            salud.AlCambiarVidaReplicada -= Refrescar;
        }
    }

    private void Refrescar(int vidaActual, int vidaMaxima)
    {
        if (relleno == null) return;

        float fraccion = vidaMaxima > 0 ? Mathf.Clamp01((float)vidaActual / vidaMaxima) : 0f;

        Vector3 escala = relleno.transform.localScale;
        escala.x = fraccion;
        relleno.transform.localScale = escala;

        // Caido (0 de vida): barra vacia y apagada, distinguible del resto.
        relleno.color = vidaActual <= 0 ? colorCaido : colorVida;
    }
}
