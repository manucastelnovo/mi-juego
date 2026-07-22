using System;
using UnityEngine;

/// <summary>
/// Moneda recogible: al entrar en contacto con el jugador desaparece y
/// notifica el evento estático AlRecoger. El HUD (S5, issue #6) se
/// suscribirá a este evento para incrementar su marcador; esta clase no
/// conoce ni depende de la UI.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Moneda : MonoBehaviour
{
    /// <summary>Se dispara cada vez que se recoge una moneda.</summary>
    public static event Action AlRecoger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        AlRecoger?.Invoke();
        Destroy(gameObject);
    }
}
