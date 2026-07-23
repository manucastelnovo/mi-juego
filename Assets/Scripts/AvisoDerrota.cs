using UnityEngine;

/// <summary>
/// Aviso de derrota (E3, #68): se suscribe al evento estático
/// EstadoCaido.AlCaerTodosLosJugadores y deja constancia en la consola
/// cuando el host detecta que todos los jugadores quedaron caídos a la vez.
/// La pantalla de derrota real (UI) es de la E5 (#49); acá solo el aviso.
/// </summary>
public class AvisoDerrota : MonoBehaviour
{
    private void OnEnable()
    {
        EstadoCaido.AlCaerTodosLosJugadores += AvisarDerrota;
    }

    private void OnDisable()
    {
        EstadoCaido.AlCaerTodosLosJugadores -= AvisarDerrota;
    }

    private void AvisarDerrota()
    {
        Debug.Log("DERROTA: todos caidos");
    }
}
