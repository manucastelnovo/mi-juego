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
    // fuera del nivel. Arena de ~2,5 pantallas (52 u de piso, de -26 a 26);
    // con la camara ortografica en 5.77 y aspecto 16:9 el semiancho visible
    // es 5.77*16/9 = 10.26 u, asi que el centro de camara puede llegar hasta
    // 26 - 10.26 = 15.74 u de cada borde sin mostrar el vacio (historia #35).
    [Header("Limites de la arena (definidos por #35)")]
    [SerializeField] private bool usarLimites = true;
    [SerializeField] private float limiteMinX = -15.74f;
    [SerializeField] private float limiteMaxX = 15.74f;

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
