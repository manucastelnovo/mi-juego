using UnityEngine;

/// <summary>
/// Movimiento del personaje: desplazamiento horizontal y salto con
/// deteccion de suelo. Se controla desde los botones tactiles en pantalla
/// (ver BotonDireccion), que llaman a los metodos publicos Mover/Saltar/
/// DetenerMovimiento. No lee teclado: el input llega por la UI tactil.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float fuerzaSalto = 12f;

    private Rigidbody2D rb;
    private float direccionHorizontal;
    private bool saltoSolicitado;
    private bool enSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Movimiento horizontal conservando la velocidad vertical.
        rb.linearVelocity = new Vector2(direccionHorizontal * velocidad, rb.linearVelocity.y);

        // Salto solo si esta pisando el suelo.
        if (saltoSolicitado && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
        saltoSolicitado = false;
    }

    // === API publica para los controles tactiles (#4) ===

    /// <summary>Fija la direccion horizontal (-1 izquierda, 1 derecha, 0 quieto).</summary>
    public void Mover(float direccion)
    {
        direccionHorizontal = Mathf.Clamp(direccion, -1f, 1f);
    }

    /// <summary>Detiene el desplazamiento horizontal.</summary>
    public void DetenerMovimiento()
    {
        direccionHorizontal = 0f;
    }

    /// <summary>Solicita un salto; solo se ejecuta si el personaje esta en el suelo.</summary>
    public void Saltar()
    {
        if (enSuelo)
        {
            saltoSolicitado = true;
        }
    }

    // === Deteccion de suelo por normal de contacto (sin capas ni referencias) ===

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            // Una normal apuntando hacia arriba significa que pisamos una superficie.
            if (contacto.normal.y > 0.5f)
            {
                enSuelo = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        enSuelo = false;
    }
}
