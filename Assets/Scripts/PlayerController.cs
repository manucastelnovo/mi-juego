using UnityEngine;

/// <summary>
/// Movimiento del personaje: desplazamiento horizontal y salto con
/// deteccion de suelo. Expone metodos publicos (Mover/Saltar) para que
/// los botones tactiles de #4 lo controlen. El control por teclado es
/// temporal para poder probar #3 y sera reemplazado en #4.
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

    private void Update()
    {
        // --- Control temporal por teclado (se elimina en #4) ---
        direccionHorizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Saltar();
        }
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
