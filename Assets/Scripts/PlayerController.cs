using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movimiento del personaje: desplazamiento horizontal y salto con
/// deteccion de suelo. Se controla desde los botones tactiles en pantalla
/// (ver BotonDireccion), que llaman a los metodos publicos Mover/Saltar/
/// DetenerMovimiento. En el editor o en builds de desarrollo tambien acepta
/// teclado (ver LeerInputTeclado), solo como ayuda de pruebas.
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

    // Colliders que actualmente hacen contacto de suelo (normal hacia arriba).
    // Contar contactos en vez de un booleano evita que un roce con otro
    // objeto (enemigo, otro jugador) que se separa deje al personaje sin salto.
    private readonly HashSet<Collider2D> contactosSuelo = new HashSet<Collider2D>();

    /// <summary>Indica si el personaje esta apoyado en el suelo (lo necesita el ataque contextual de la E3).</summary>
    public bool EstaEnSuelo => contactosSuelo.Count > 0;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // Estado de las teclas de direccion para no pisar el movimiento tactil:
    // solo se dispara Mover/DetenerMovimiento en los flancos de tecla, igual
    // que hacen los botones tactiles al presionar/soltar.
    private bool izquierdaPresionada;
    private bool derechaPresionada;
#endif

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Update()
    {
        LeerInputTeclado();
    }

    /// <summary>Input de teclado solo para desarrollo (editor / development build); no corre en produccion.</summary>
    private void LeerInputTeclado()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            izquierdaPresionada = true;
            Mover(-1f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            derechaPresionada = true;
            Mover(1f);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            izquierdaPresionada = false;
            if (derechaPresionada) Mover(1f);
            else DetenerMovimiento();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            derechaPresionada = false;
            if (izquierdaPresionada) Mover(-1f);
            else DetenerMovimiento();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Saltar();
        }
    }
#endif

    private void FixedUpdate()
    {
        // Movimiento horizontal conservando la velocidad vertical.
        rb.linearVelocity = new Vector2(direccionHorizontal * velocidad, rb.linearVelocity.y);

        // Salto solo si esta pisando el suelo.
        if (saltoSolicitado && EstaEnSuelo)
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
        if (EstaEnSuelo)
        {
            saltoSolicitado = true;
        }
    }

    // === Deteccion de suelo por conteo de contactos (sin capas ni referencias) ===
    // Se cuenta por collider en contacto, no un booleano global: si un enemigo
    // o companero roza al jugador y se separa, no debe afectar el contacto real con el suelo.

    private void OnCollisionStay2D(Collision2D collision)
    {
        bool esContactoDeSuelo = false;
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            // Una normal apuntando hacia arriba significa que pisamos una superficie.
            if (contacto.normal.y > 0.5f)
            {
                esContactoDeSuelo = true;
                break;
            }
        }

        if (esContactoDeSuelo)
        {
            contactosSuelo.Add(collision.collider);
        }
        else
        {
            // Ya no aporta contacto de suelo (por ejemplo, quedo pegado de costado).
            contactosSuelo.Remove(collision.collider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Solo se quita ese contacto puntual; el resto de los contactos de suelo se mantienen.
        contactosSuelo.Remove(collision.collider);
    }
}
