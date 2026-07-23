using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la pantalla de inicio (escena Menu). Los botones CREAR SALA y
/// BUSCAR SALA arrancan deshabilitados hasta que GestorRed confirma que la
/// sesion anonima quedo lista; si la sesion falla, se avisa en pantalla en
/// vez de dejar los botones muertos sin explicacion. Sigue el mismo patron
/// que ContadorMonedas: la UI escucha los eventos estaticos, no pregunta.
/// </summary>
public class MenuInicio : MonoBehaviour
{
    [SerializeField] private Button botonCrearSala;
    [SerializeField] private Button botonBuscarSala;
    [SerializeField] private Text textoEstado;

    private void OnEnable()
    {
        MostrarConectando();
        GestorRed.AlIniciarSesion += ManejarSesionIniciada;
        GestorRed.AlFallarSesion += ManejarSesionFallida;

        // Si el gestor ya tenia sesion antes de que este menu se habilitara
        // (por ejemplo al volver de otra escena), no esperamos un evento
        // que ya se disparo y no va a repetirse.
        if (GestorRed.Instancia != null && GestorRed.Instancia.SesionIniciada)
        {
            ManejarSesionIniciada();
        }
    }

    private void OnDisable()
    {
        GestorRed.AlIniciarSesion -= ManejarSesionIniciada;
        GestorRed.AlFallarSesion -= ManejarSesionFallida;
    }

    private void ManejarSesionIniciada()
    {
        HabilitarBotones(true);
        MostrarEstado(string.Empty);
    }

    private void ManejarSesionFallida(string motivo)
    {
        HabilitarBotones(false);
        MostrarEstado("Sin conexion. Revisa tu internet e intenta de nuevo.");
    }

    private void MostrarConectando()
    {
        HabilitarBotones(false);
        MostrarEstado("Conectando...");
    }

    private void HabilitarBotones(bool interactuable)
    {
        if (botonCrearSala != null) botonCrearSala.interactable = interactuable;
        if (botonBuscarSala != null) botonBuscarSala.interactable = interactuable;
    }

    private void MostrarEstado(string mensaje)
    {
        if (textoEstado != null) textoEstado.text = mensaje;
    }

    /// <summary>Enganche de CREAR SALA. El flujo real lo implementa la historia #38.</summary>
    public void OnCrearSala()
    {
        Debug.Log("[Menu] CREAR SALA presionado. Flujo pendiente: historia #38.");
    }

    /// <summary>Enganche de BUSCAR SALA. El flujo real lo implementa la historia #39.</summary>
    public void OnBuscarSala()
    {
        Debug.Log("[Menu] BUSCAR SALA presionado. Flujo pendiente: historia #39.");
    }
}
