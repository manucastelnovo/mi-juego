using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Botón táctil de control. Al mantenerlo presionado mueve (izquierda/derecha)
/// y al soltarlo frena; el botón de salto dispara un salto al presionar.
/// Se engancha al jugador LOCAL (el dueño en este dispositivo) escuchando
/// PlayerController.AlCambiarJugadorLocal, en vez de buscarlo por tag: con
/// varios jugadores en escena un tag agarraría a cualquiera, y en Awake
/// todavía no existe ninguno (la red los instancia recién al conectarse).
/// </summary>
public class BotonDireccion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Accion { Izquierda, Derecha, Saltar, Atacar }

    [SerializeField] private Accion accion = Accion.Izquierda;

    private PlayerController jugadorLocal;

    private void OnEnable()
    {
        // Si el jugador local ya estaba listo antes de que este boton se
        // activara, se engancha directo; si no, o si se reconecta despues,
        // el evento avisa cuando corresponda.
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
        Debug.Log($"[Botones] {name} ({accion}) {(jugadorLocal != null ? "enganchado al jugador local" : "desenganchado (sin jugador local)")}.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (jugadorLocal == null) return;

        switch (accion)
        {
            case Accion.Izquierda: jugadorLocal.Mover(-1f); break;
            case Accion.Derecha:   jugadorLocal.Mover(1f);  break;
            case Accion.Saltar:    jugadorLocal.Saltar();   break;
            case Accion.Atacar:    jugadorLocal.Atacar();  break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (jugadorLocal == null) return;

        // Al soltar un botón de movimiento, el personaje se detiene.
        if (accion == Accion.Izquierda || accion == Accion.Derecha)
        {
            jugadorLocal.DetenerMovimiento();
        }
    }
}
