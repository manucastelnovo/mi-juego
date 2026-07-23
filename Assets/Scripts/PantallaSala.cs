using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra la sala de espera: el codigo grande y copiable, mientras se
/// espera a que entren mas jugadores. La usan tanto el host (al crear la
/// sala) como el cliente (al confirmar que entra a una que encontro).
/// Escucha a GestorSala igual que MenuInicio escucha a GestorRed. La lista de
/// jugadores y el boton de salir los agrega la historia #40; este panel es
/// el enganche para eso.
/// </summary>
public class PantallaSala : MonoBehaviour
{
    [SerializeField] private GameObject panelSalaEspera;
    [SerializeField] private Text textoCodigo;
    [SerializeField] private Text textoTitulo;

    private void Awake()
    {
        if (panelSalaEspera != null) panelSalaEspera.SetActive(false);
    }

    private void OnEnable()
    {
        GestorSala.AlCrearSala += ManejarSalaCreada;
        GestorSala.AlEntrarSala += ManejarEntroASala;
    }

    private void OnDisable()
    {
        GestorSala.AlCrearSala -= ManejarSalaCreada;
        GestorSala.AlEntrarSala -= ManejarEntroASala;
    }

    private void ManejarSalaCreada(string codigo)
    {
        if (textoTitulo != null) textoTitulo.text = "SALA CREADA";
        MostrarSala(codigo);
    }

    private void ManejarEntroASala(string codigo)
    {
        if (textoTitulo != null) textoTitulo.text = "TE UNISTE A LA SALA";
        MostrarSala(codigo);
    }

    private void MostrarSala(string codigo)
    {
        if (textoCodigo != null) textoCodigo.text = codigo;
        if (panelSalaEspera != null) panelSalaEspera.SetActive(true);
    }

    /// <summary>Enganche del boton COPIAR: copia el codigo actual al portapapeles.</summary>
    public void OnCopiarCodigo()
    {
        if (textoCodigo == null) return;
        GUIUtility.systemCopyBuffer = textoCodigo.text;
    }
}
