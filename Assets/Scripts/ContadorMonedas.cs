using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD que muestra la cantidad de monedas recogidas en la sesión actual.
/// Se suscribe al evento estático Moneda.AlRecoger y actualiza el texto
/// en tiempo real. Al ser estático el evento, se suscribe en OnEnable y
/// se desuscribe en OnDisable para no acumular suscriptores muertos entre
/// corridas de Play mode; el contador arranca en 0 en cada OnEnable.
/// </summary>
[RequireComponent(typeof(Text))]
public class ContadorMonedas : MonoBehaviour
{
    private Text texto;
    private int monedas;

    private void Awake()
    {
        texto = GetComponent<Text>();
    }

    private void OnEnable()
    {
        monedas = 0;
        ActualizarTexto();
        Moneda.AlRecoger += SumarMoneda;
    }

    private void OnDisable()
    {
        Moneda.AlRecoger -= SumarMoneda;
    }

    private void SumarMoneda()
    {
        monedas++;
        ActualizarTexto();
    }

    private void ActualizarTexto()
    {
        texto.text = $"Monedas: {monedas}";
    }
}
