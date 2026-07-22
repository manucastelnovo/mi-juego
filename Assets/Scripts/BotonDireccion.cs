using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Botón táctil de control. Al mantenerlo presionado mueve (izquierda/derecha)
/// y al soltarlo frena; el botón de salto dispara un salto al presionar.
/// Llama a los métodos públicos de PlayerController, que encuentra por tag.
/// </summary>
public class BotonDireccion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Accion { Izquierda, Derecha, Saltar }

    [SerializeField] private Accion accion = Accion.Izquierda;

    private PlayerController player;

    private void Awake()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.GetComponent<PlayerController>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player == null) return;

        switch (accion)
        {
            case Accion.Izquierda: player.Mover(-1f); break;
            case Accion.Derecha:   player.Mover(1f);  break;
            case Accion.Saltar:    player.Saltar();   break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (player == null) return;

        // Al soltar un botón de movimiento, el personaje se detiene.
        if (accion == Accion.Izquierda || accion == Accion.Derecha)
        {
            player.DetenerMovimiento();
        }
    }
}
