using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Movimiento del personaje: desplazamiento horizontal y salto con
/// deteccion de suelo. Es un objeto de red (NetworkBehaviour): cada jugador
/// tiene autoridad sobre su propio movimiento (no espera al host) y el
/// script solo procesa input si este cliente es el dueno del objeto. Se
/// controla desde los botones tactiles en pantalla (ver BotonDireccion),
/// que llaman a los metodos publicos Mover/Saltar/DetenerMovimiento. En el
/// editor o en builds de desarrollo tambien acepta teclado (ver
/// LeerInputTeclado), solo como ayuda de pruebas.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour
{
    // === Enganche para los controles tactiles (#31) ===
    // Cada proceso (dispositivo/instancia) solo llega a tener IsOwner true en
    // el PlayerController que le pertenece, asi que esta referencia estatica
    // nunca mezcla el jugador de otro. Se avisa por evento (mismo patron que
    // Moneda.AlRecoger/GestorRed.AlIniciarSesion): BotonDireccion escucha en
    // vez de buscar por tag, y no necesita nada antes de OnNetworkSpawn.
    /// <summary>Jugador local actual (el que pertenece a este dispositivo), o null si todavia no aparecio.</summary>
    public static PlayerController JugadorLocal { get; private set; }

    /// <summary>Se dispara cuando el jugador local aparece (con la instancia) o desaparece (con null).</summary>
    public static event Action<PlayerController> AlCambiarJugadorLocal;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float fuerzaSalto = 12f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private float direccionHorizontal;
    private bool saltoSolicitado;

    // Colliders que actualmente hacen contacto de suelo (normal hacia arriba).
    // Contar contactos en vez de un booleano evita que un roce con otro
    // objeto (enemigo, otro jugador) que se separa deje al personaje sin salto.
    private readonly HashSet<Collider2D> contactosSuelo = new HashSet<Collider2D>();

    /// <summary>Indica si el personaje esta apoyado en el suelo (lo necesita el ataque contextual de la E3).</summary>
    public bool EstaEnSuelo => contactosSuelo.Count > 0;

    // Hacia donde mira el personaje. Se sincroniza para que todos los
    // clientes vean el mismo flip de sprite (lo necesita el ataque
    // direccional de la E3). Solo el dueno la puede escribir.
    private readonly NetworkVariable<bool> miraDerecha = new NetworkVariable<bool>(
        true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    /// <summary>True si el personaje mira hacia la derecha (lo necesita el ataque direccional de la E3).</summary>
    public bool MiraDerecha => miraDerecha.Value;

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
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public override void OnNetworkSpawn()
    {
        AplicarFlip(miraDerecha.Value);
        miraDerecha.OnValueChanged += AlCambiarMiraDerecha;

        // Solo el dueno de este objeto es "el jugador local" en este dispositivo.
        if (IsOwner)
        {
            JugadorLocal = this;
            AlCambiarJugadorLocal?.Invoke(this);
            Debug.Log($"[Jugador] Jugador local listo (OwnerClientId {OwnerClientId}). Botones enganchados.");
        }
    }

    public override void OnNetworkDespawn()
    {
        miraDerecha.OnValueChanged -= AlCambiarMiraDerecha;

        // Si se desconecta (o se recicla) el jugador local, los botones se
        // desenganchan hasta que la red instancie uno nuevo al reconectar.
        if (IsOwner && JugadorLocal == this)
        {
            JugadorLocal = null;
            AlCambiarJugadorLocal?.Invoke(null);
        }
    }

    private void AlCambiarMiraDerecha(bool anterior, bool nueva)
    {
        AplicarFlip(nueva);
    }

    private void AplicarFlip(bool derecha)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !derecha;
        }
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Update()
    {
        // Solo el dueno del objeto procesa su propio input.
        if (!IsOwner) return;
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
        // La fisica del movimiento solo la simula el dueno: en los demas
        // clientes la posicion llega sincronizada por NetworkTransform.
        if (!IsOwner) return;

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
    // Todas quedan sin efecto si quien las llama no es el dueno del objeto:
    // asi, aunque un boton tactil encuentre al jugador equivocado (ver #31),
    // nunca mueve al personaje de otro.

    /// <summary>Fija la direccion horizontal (-1 izquierda, 1 derecha, 0 quieto).</summary>
    public void Mover(float direccion)
    {
        if (!IsOwner) return;

        direccionHorizontal = Mathf.Clamp(direccion, -1f, 1f);
        if (direccionHorizontal != 0f)
        {
            miraDerecha.Value = direccionHorizontal > 0f;
        }
    }

    /// <summary>Detiene el desplazamiento horizontal.</summary>
    public void DetenerMovimiento()
    {
        if (!IsOwner) return;

        direccionHorizontal = 0f;
    }

    /// <summary>Solicita un salto; solo se ejecuta si el personaje esta en el suelo.</summary>
    public void Saltar()
    {
        if (!IsOwner) return;

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
