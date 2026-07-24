using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Dano por contacto de un enemigo a los jugadores (E3, #69). Corre SOLO en
/// el host: mientras el enemigo toca a un jugador, le aplica dano con una
/// cadencia por jugador (ademas de la invulnerabilidad propia del jugador).
/// Ignora todo lo que no sea un jugador (suelo, plataformas, otros enemigos).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DanioPorContacto : NetworkBehaviour
{
    [Header("Balance (visible desde el Inspector)")]
    [Tooltip("Puntos de vida que quita cada golpe por contacto.")]
    [SerializeField] private int danio = 1;

    [Tooltip("Segundos entre golpes al mismo jugador.")]
    [SerializeField] private float cadencia = 1f;

    // Momento (Time.time) a partir del cual se puede volver a golpear a cada jugador.
    private readonly Dictionary<SaludJugador, float> proximoGolpe = new Dictionary<SaludJugador, float>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IntentarGolpe(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        IntentarGolpe(collision.collider);
    }

    private void IntentarGolpe(Collider2D col)
    {
        if (!IsServer) return;

        SaludJugador salud = col.GetComponentInParent<SaludJugador>();
        if (salud == null) return;

        // Cadencia por jugador para no vaciar la vida en un solo contacto.
        if (proximoGolpe.TryGetValue(salud, out float momento) && Time.time < momento)
        {
            return;
        }

        salud.AplicarDanio(danio);
        proximoGolpe[salud] = Time.time + cadencia;
    }
}
