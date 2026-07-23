using UnityEngine;

/// <summary>
/// Hace que la camara de este dispositivo siga en X al jugador LOCAL (el
/// dueño en este dispositivo), con suavizado. Se engancha escuchando
/// PlayerController.AlCambiarJugadorLocal en vez de buscar por tag: el
/// jugador local no existe al arrancar la escena (la red lo instancia
/// recien al conectarse) y con varios jugadores en pantalla un tag
/// agarraria a cualquiera. Mismo patron que usa BotonDireccion (#31).
/// </summary>
public class CamaraSeguidora : MonoBehaviour
{
    [Header("Seguimiento")]
    [SerializeField] private float tiempoSuavizado = 0.15f;

    // Limites de la arena en X, para que la camara nunca muestre el vacio
    // fuera del nivel. Los valores reales los define la historia #35 (arena
    // ancha); por ahora quedan desactivados para no inventar un tamaño de
    // arena que todavia no existe.
    [Header("Limites de la arena (los define #35)")]
    [SerializeField] private bool usarLimites = false;
    [SerializeField] private float limiteMinX = 0f;
    [SerializeField] private float limiteMaxX = 0f;

    private PlayerController jugadorLocal;
    private float velocidadActualX;

    private void OnEnable()
    {
        // Si el jugador local ya estaba listo (por ejemplo, al reconectar),
        // se engancha directo; si no, el evento avisa cuando corresponda.
        jugadorLocal = PlayerController.JugadorLocal;
        PlayerController.AlCambiarJugadorLocal += ActualizarJugadorLocal;
    }

    private void OnDisable()
    {
        PlayerController.AlCambiarJugadorLocal -= ActualizarJugadorLocal;
    }

    private void ActualizarJugadorLocal(PlayerController nuevoJugadorLocal)
    {
        jugadorLocal = nuevoJugadorLocal;
    }

    private void LateUpdate()
    {
        // Sin jugador local todavia (recien arranco la escena, o se
        // desconecto) la camara se queda quieta donde esta.
        if (jugadorLocal == null) return;

        float objetivoX = jugadorLocal.transform.position.x;
        if (usarLimites)
        {
            objetivoX = Mathf.Clamp(objetivoX, limiteMinX, limiteMaxX);
        }

        Vector3 posicion = transform.position;
        posicion.x = Mathf.SmoothDamp(posicion.x, objetivoX, ref velocidadActualX, tiempoSuavizado);
        transform.position = posicion;
    }
}
