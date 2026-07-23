using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Ataque de espada contextual (E3, #65). Un solo boton: la direccion del
/// corte se deduce de la situacion (en el suelo hacia donde mira; en el aire
/// con direccion mantenida hacia ese lado; cayendo sin direccion estocada
/// abajo con rebote; subiendo sin direccion hacia arriba). Cadencia limitada.
///
/// Autoridad de red: el cliente DUENO pide el ataque y ve la animacion al
/// instante (no espera al host); el HOST resuelve el golpe con OverlapBox y
/// decide a quien le pega. Un cliente no dueno no puede disparar el ataque de
/// otro jugador (el ServerRpc exige propiedad).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class AtaqueEspada : NetworkBehaviour
{
    // Direccion del corte. byte para viajar barato por RPC.
    private enum Direccion : byte { Izquierda, Derecha, Arriba, Abajo }

    [Header("Balance (visible desde el Inspector)")]
    [Tooltip("Segundos minimos entre golpes: apretar mas rapido no da mas golpes.")]
    [SerializeField] private float cadencia = 0.35f;

    [Tooltip("Distancia del centro del jugador al centro de la zona de golpe.")]
    [SerializeField] private float alcance = 0.9f;

    [Tooltip("Ancho y alto de la zona de golpe (OverlapBox).")]
    [SerializeField] private Vector2 tamanoZona = new Vector2(1.2f, 1f);

    [Tooltip("Puntos de vida que quita cada espadazo.")]
    [SerializeField] private int danio = 1;

    [Tooltip("Impulso hacia arriba cuando la estocada abajo acierta (rebote).")]
    [SerializeField] private float fuerzaRebote = 12f;

    [Tooltip("Umbral de velocidad vertical para considerar que el jugador sube.")]
    [SerializeField] private float umbralSubida = 0.1f;

    [Header("Feedback visual (opcional)")]
    [Tooltip("Objeto hijo que representa el corte; se muestra un instante al atacar. Si es null, no pasa nada.")]
    [SerializeField] private GameObject indicadorGolpe;

    [Tooltip("Segundos que se muestra el indicador de golpe.")]
    [SerializeField] private float duracionIndicador = 0.12f;

    private Rigidbody2D rb;
    private PlayerController jugador;
    private float ultimoAtaque = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GetComponent<PlayerController>();
        if (indicadorGolpe != null)
        {
            indicadorGolpe.SetActive(false);
        }
    }

    /// <summary>
    /// Pide un ataque. Lo llama el boton tactil / teclado del jugador local.
    /// Sin efecto si no es el dueno o si no paso la cadencia.
    /// </summary>
    public void Atacar()
    {
        if (!IsOwner) return;
        if (Time.time - ultimoAtaque < cadencia) return;
        ultimoAtaque = Time.time;

        Direccion dir = DeducirDireccion();

        // El dueno ve el corte al instante, sin esperar al host.
        MostrarCorte(dir);

        // El host resuelve el golpe (dano y rebote).
        AtacarServerRpc(dir);
    }

    // Traduce la situacion del personaje a una direccion de corte.
    private Direccion DeducirDireccion()
    {
        if (jugador != null && jugador.EstaEnSuelo)
        {
            return jugador.MiraDerecha ? Direccion.Derecha : Direccion.Izquierda;
        }

        // En el aire: si mantiene una direccion horizontal, corta hacia ese lado.
        float horizontal = jugador != null ? jugador.DireccionHorizontal : 0f;
        if (horizontal > 0f) return Direccion.Derecha;
        if (horizontal < 0f) return Direccion.Izquierda;

        // En el aire sin direccion: subiendo corta arriba, cayendo estocada abajo.
        return rb.linearVelocity.y > umbralSubida ? Direccion.Arriba : Direccion.Abajo;
    }

    // Centro de la zona de golpe segun la direccion.
    private Vector2 CentroZona(Direccion dir)
    {
        Vector2 origen = transform.position;
        switch (dir)
        {
            case Direccion.Izquierda: return origen + Vector2.left * alcance;
            case Direccion.Derecha:   return origen + Vector2.right * alcance;
            case Direccion.Arriba:    return origen + Vector2.up * alcance;
            default:                  return origen + Vector2.down * alcance;
        }
    }

    [ServerRpc]
    private void AtacarServerRpc(Direccion dir)
    {
        // El host decide a quien le pega.
        bool pego = ResolverGolpe(dir);

        // Todos los clientes ven el corte (misma direccion en host y cliente).
        MostrarCorteClientRpc(dir);

        // La estocada abajo que acierta rebota al atacante (su movimiento es
        // autoritativo del dueno, asi que el rebote se aplica en su cliente).
        if (dir == Direccion.Abajo && pego)
        {
            var destino = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } }
            };
            RebotarClientRpc(destino);
        }
    }

    // Corre solo en el host: busca objetos golpeables y les aplica dano.
    private bool ResolverGolpe(Direccion dir)
    {
        bool pego = false;
        Collider2D[] tocados = Physics2D.OverlapBoxAll(CentroZona(dir), tamanoZona, 0f);
        foreach (Collider2D col in tocados)
        {
            SaludJugador salud = col.GetComponentInParent<SaludJugador>();
            if (salud == null) continue;
            // No se pega a si mismo.
            if (salud.NetworkObject == NetworkObject) continue;

            salud.AplicarDanio(danio);
            pego = true;
        }
        // NOTA: cuando existan los enemigos (#69) se agregan aca como objetivos.
        return pego;
    }

    [ClientRpc]
    private void MostrarCorteClientRpc(Direccion dir)
    {
        // El dueno ya lo mostro al instante; los demas clientes lo ven ahora.
        if (IsOwner) return;
        MostrarCorte(dir);
    }

    [ClientRpc]
    private void RebotarClientRpc(ClientRpcParams destino = default)
    {
        // Solo llega al dueno: impulso hacia arriba conservando la horizontal.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaRebote);
    }

    // Muestra el indicador de corte un instante en la direccion dada.
    private void MostrarCorte(Direccion dir)
    {
        if (indicadorGolpe == null) return;

        indicadorGolpe.transform.position = CentroZona(dir);
        indicadorGolpe.SetActive(true);
        CancelInvoke(nameof(OcultarCorte));
        Invoke(nameof(OcultarCorte), duracionIndicador);
    }

    private void OcultarCorte()
    {
        if (indicadorGolpe != null)
        {
            indicadorGolpe.SetActive(false);
        }
    }

#if UNITY_EDITOR
    // Dibuja la zona de golpe en la escena para ajustar tamano/alcance.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (Direccion dir in new[] { Direccion.Izquierda, Direccion.Derecha, Direccion.Arriba, Direccion.Abajo })
        {
            Gizmos.DrawWireCube(CentroZona(dir), tamanoZona);
        }
    }
#endif
}
