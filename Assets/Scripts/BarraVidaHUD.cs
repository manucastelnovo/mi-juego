using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD de la vida del jugador local (E3, #66). Escucha el evento estatico
/// SaludJugador.AlCambiarVidaLocal y actualiza la barra en tiempo real, sin
/// conocer al jugador, igual que ContadorMonedas escucha a Moneda.AlRecoger.
/// Se suscribe en OnEnable y se desuscribe en OnDisable para no acumular
/// suscriptores muertos entre corridas de Play mode.
///
/// Funciona con lo que tenga asignado el Canvas: una Image de relleno
/// (fillAmount 0..1) y/o un Text "Vida: 4/5". Si falta alguno, se ignora.
/// </summary>
public class BarraVidaHUD : MonoBehaviour
{
    [Header("Referencias del HUD (asignar en el Inspector)")]
    [Tooltip("Image con Image Type = Filled; su fillAmount refleja la vida.")]
    [SerializeField] private Image relleno;

    [Tooltip("Texto opcional con el numero de vida (ej. \"Vida: 4/5\").")]
    [SerializeField] private Text texto;

    private void OnEnable()
    {
        // Arranca vacia hasta que el jugador local avise su vida.
        Refrescar(0, 0);
        SaludJugador.AlCambiarVidaLocal += Refrescar;
    }

    private void OnDisable()
    {
        SaludJugador.AlCambiarVidaLocal -= Refrescar;
    }

    private void Refrescar(int vidaActual, int vidaMaxima)
    {
        if (relleno != null)
        {
            relleno.fillAmount = vidaMaxima > 0 ? (float)vidaActual / vidaMaxima : 0f;
        }

        if (texto != null)
        {
            texto.text = $"Vida: {vidaActual}/{vidaMaxima}";
        }
    }
}
