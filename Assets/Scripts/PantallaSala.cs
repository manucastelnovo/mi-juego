using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra la sala una vez creada: el codigo grande y copiable, mientras se
/// espera a que entren mas jugadores. Escucha a GestorSala igual que
/// MenuInicio escucha a GestorRed. La lista de jugadores y el boton de salir
/// los agrega la historia #40; este panel es el enganche para eso.
/// </summary>
public class PantallaSala : MonoBehaviour
{
    [SerializeField] private GameObject panelSalaEspera;
    [SerializeField] private Text textoCodigo;

    private void Awake()
    {
        if (panelSalaEspera != null) panelSalaEspera.SetActive(false);
    }

    private void OnEnable()
    {
        GestorSala.AlCrearSala += ManejarSalaCreada;
    }

    private void OnDisable()
    {
        GestorSala.AlCrearSala -= ManejarSalaCreada;
    }

    private void ManejarSalaCreada(string codigo)
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
