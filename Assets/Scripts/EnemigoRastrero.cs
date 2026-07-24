using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Enemigo Rastrero (E3, #69): camina hacia el jugador mas cercano. Corre
/// SOLO en el host (el enemigo es un objeto de red que los clientes ven
/// sincronizado por NetworkTransform). Retarget cada frame: si el objetivo
/// cae o se desconecta, elige otro sin quedarse quieto. Movimiento horizontal
/// tipo rastrero; la gravedad lo mantiene sobre el suelo.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoRastrero : NetworkBehaviour
{
    [Header("Balance (visible desde el Inspector)")]
    [Tooltip("Velocidad horizontal de persecucion.")]
    [SerializeField] private float velocidad = 2.5f;

    [Tooltip("Distancia horizontal minima al objetivo para dejar de empujar (evita vibrar encima).")]
    [SerializeField] private float distanciaMinima = 0.1f;

    [Header("Sprite (opcional)")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    // Hacia donde mira, sincronizado para que el flip se vea igual en todos.
    private readonly NetworkVariable<bool> miraDerecha = new NetworkVariable<bool>(
        true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        AplicarFlip(miraDerecha.Value);
        miraDerecha.OnValueChanged += (_, nueva) => AplicarFlip(nueva);
    }

    private void FixedUpdate()
    {
        // La IA y el movimiento solo los simula el host.
        if (!IsServer) return;

        Transform objetivo = BuscarObjetivoMasCercano();
        if (objetivo == null)
        {
            // Sin objetivo valido (p. ej. todos caidos): se queda quieto.
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dx = objetivo.position.x - transform.position.x;
        float direccion = Mathf.Abs(dx) <= distanciaMinima ? 0f : Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);

        if (direccion != 0f)
        {
            miraDerecha.Value = direccion > 0f;
        }
    }

    // El jugador VIVO (no caido) mas cercano; null si no hay ninguno.
    private Transform BuscarObjetivoMasCercano()
    {
        Transform mejor = null;
        float mejorDistancia = float.MaxValue;
        Vector2 pos = transform.position;

        foreach (EstadoCaido jugador in EstadoCaido.Jugadores)
        {
            if (jugador == null) continue;
            if (jugador.EstaCaido) continue; // no persigue a un caido

            float distancia = ((Vector2)jugador.transform.position - pos).sqrMagnitude;
            if (distancia < mejorDistancia)
            {
                mejorDistancia = distancia;
                mejor = jugador.transform;
            }
        }

        return mejor;
    }

    private void AplicarFlip(bool derecha)
    {
        if (spriteRenderer != null) spriteRenderer.flipX = !derecha;
    }
}
