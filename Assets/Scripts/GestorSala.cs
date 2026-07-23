using System;
using System.Linq;
using Unity.Services.Multiplayer;
using UnityEngine;

/// <summary>
/// Crea o busca la sala usando el servicio de Multiplayer de Unity (Lobby +
/// Relay). El codigo para invitar amigos lo genera el servicio: esta clase
/// solo lo pide y lo expone. Sigue el mismo patron que GestorRed: sobrevive a
/// los cambios de escena y avisa por eventos estaticos, la UI escucha sin
/// preguntar directamente al servicio.
/// </summary>
public class GestorSala : MonoBehaviour
{
    private const int MaxJugadores = 4;

    public static GestorSala Instancia { get; private set; }

    /// <summary>Se dispara al empezar a crear la sala (para avisar "creando...").</summary>
    public static event Action AlEmpezarCreacion;

    /// <summary>Se dispara cuando la sala quedo creada, con el codigo para compartir.</summary>
    public static event Action<string> AlCrearSala;

    /// <summary>Se dispara si la creacion fallo, con un motivo legible.</summary>
    public static event Action<string> AlFallarCreacion;

    /// <summary>Se dispara al empezar a buscar una sala por codigo (para avisar "buscando...").</summary>
    public static event Action AlEmpezarBusqueda;

    /// <summary>Se dispara cuando se encontro la sala. El cliente ya quedo conectado a ella.</summary>
    public static event Action<InfoSalaEncontrada> AlEncontrarSala;

    /// <summary>Se dispara si la busqueda/entrada fallo, con un motivo legible.</summary>
    public static event Action<string> AlFallarBusqueda;

    /// <summary>Se dispara cuando el jugador confirma que quiere entrar a la sala ya encontrada.</summary>
    public static event Action<string> AlEntrarSala;

    /// <summary>Se dispara cuando el jugador deja la sala (boton SALIR) y la UI puede volver al menu.</summary>
    public static event Action AlSalirDeSala;

    /// <summary>La sesion activa como host, una vez creada. Null si todavia no hay sala.</summary>
    public IHostSession SesionActual { get; private set; }

    /// <summary>La sesion a la que nos unimos como cliente. Null si todavia no encontramos ninguna.</summary>
    public ISession SesionUnida { get; private set; }

    /// <summary>
    /// La sesion en la que estoy metido ahora mismo, sea como host o como
    /// cliente. Null si no hay ninguna. Pensada para que la UI (sala de
    /// espera, y a futuro la historia #41) no tenga que preguntar cual de
    /// las dos sesiones usar.
    /// </summary>
    public ISession SesionEnCurso => (ISession)SesionActual ?? SesionUnida;

    private void Awake()
    {
        // Un solo gestor vivo: si ya hay uno de una escena anterior, este sobra.
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Crea una sala nueva como host, vía Relay + Lobby (com.unity.services.multiplayer).
    /// El codigo generado por el servicio llega por AlCrearSala; si algo falla, por
    /// AlFallarCreacion. Es seguro llamarla otra vez si una creacion anterior fallo.
    /// </summary>
    public async void CrearSala()
    {
        AlEmpezarCreacion?.Invoke();

        try
        {
            SessionOptions opciones = new SessionOptions
            {
                MaxPlayers = MaxJugadores,
                IsPrivate = true // se entra por codigo, no aparece en listados publicos
            }.WithRelayNetwork();

            IHostSession sesion = await MultiplayerService.Instance.CreateSessionAsync(opciones);
            SesionActual = sesion;

            Debug.Log($"[Sala] Sala creada como host. Codigo: {sesion.Code}");
            AlCrearSala?.Invoke(sesion.Code);
        }
        catch (SessionException e)
        {
            Debug.LogError($"[Sala] No se pudo crear la sala: {e.Message}");
            AlFallarCreacion?.Invoke("No se pudo crear la sala. Revisa tu conexion e intenta de nuevo.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Sala] Error inesperado creando la sala: {e.Message}");
            AlFallarCreacion?.Invoke("Ocurrio un error. Intenta de nuevo.");
        }
    }

    /// <summary>
    /// Busca una sala por codigo y se une a ella como cliente (no hay forma de
    /// "solo mirar" una sala privada sin unirse: el propio servicio de Lobby
    /// valida existencia y cupo al unirse). Si encuentra la sala, el resultado
    /// llega por AlEncontrarSala con el host y la cantidad de jugadores; si
    /// no, por AlFallarBusqueda con un motivo legible. Es seguro reintentar
    /// tras un fallo.
    /// </summary>
    public async void BuscarSala(string codigoIngresado)
    {
        AlEmpezarBusqueda?.Invoke();

        string codigo = LimpiarCodigo(codigoIngresado);
        if (string.IsNullOrEmpty(codigo))
        {
            AlFallarBusqueda?.Invoke("Escribi un codigo primero.");
            return;
        }

        try
        {
            ISession sesion = await MultiplayerService.Instance.JoinSessionByCodeAsync(codigo);
            SesionUnida = sesion;

            Debug.Log($"[Sala] Sala encontrada. Host: {sesion.Host}, jugadores: {sesion.PlayerCount}/{sesion.MaxPlayers}");
            AlEncontrarSala?.Invoke(new InfoSalaEncontrada(sesion.Host, sesion.PlayerCount, sesion.MaxPlayers));
        }
        catch (SessionException e)
        {
            Debug.LogError($"[Sala] No se pudo entrar a la sala '{codigo}': {e.Message}");
            AlFallarBusqueda?.Invoke(MensajeParaFalloDeBusqueda(e));
        }
        catch (Exception e)
        {
            Debug.LogError($"[Sala] Error inesperado buscando la sala: {e.Message}");
            AlFallarBusqueda?.Invoke("Ocurrio un error. Intenta de nuevo.");
        }
    }

    /// <summary>
    /// Confirma la entrada a la sala ya encontrada (el jugador ya esta
    /// conectado como cliente desde BuscarSala; esto solo avisa a la UI que
    /// pase a la sala de espera, con su lista de jugadores en vivo). Arrancar
    /// la partida con el boton EMPEZAR es la historia #41.
    /// </summary>
    public void ConfirmarEntrada()
    {
        if (SesionUnida == null) return;
        AlEntrarSala?.Invoke(SesionUnida.Code);
    }

    /// <summary>
    /// Deja la sala actual (como host o como cliente) y avisa por
    /// AlSalirDeSala para que la UI vuelva al menu. Es seguro llamarla
    /// aunque ya no haya sesion activa.
    /// </summary>
    public async void SalirDeSala()
    {
        ISession sesion = SesionEnCurso;
        SesionActual = null;
        SesionUnida = null;

        if (sesion != null)
        {
            try
            {
                await sesion.LeaveAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Sala] Error al salir de la sala: {e.Message}");
            }
        }

        AlSalirDeSala?.Invoke();
    }

    /// <summary>
    /// Deja el codigo pegado listo para buscar: sin espacios (incluidos los
    /// que quedan pegados por copiar/pegar desde un chat) y en mayusculas,
    /// que es como el servicio genera los codigos.
    /// </summary>
    private static string LimpiarCodigo(string codigoIngresado)
    {
        if (string.IsNullOrEmpty(codigoIngresado)) return string.Empty;
        return new string(codigoIngresado.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();
    }

    /// <summary>
    /// El SDK no distingue "sala llena" con un error propio: la mapea junto a
    /// otros casos como SessionError.Unknown, con el detalle solo en el
    /// mensaje. Por eso se revisa el texto para dar un aviso claro en los
    /// casos mas comunes (llena, codigo invalido) y un mensaje generico en el resto.
    /// </summary>
    private static string MensajeParaFalloDeBusqueda(SessionException e)
    {
        if (e.Error == SessionError.SessionNotFound)
        {
            return "No encontramos esa sala. Revisa el codigo e intenta de nuevo.";
        }

        string mensaje = e.Message ?? string.Empty;
        if (mensaje.IndexOf("full", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "Esa sala ya esta completa (4/4). Pedile otro codigo a tu amigo.";
        }

        if (mensaje.IndexOf("join code", StringComparison.OrdinalIgnoreCase) >= 0
            || mensaje.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "No encontramos esa sala. Revisa el codigo e intenta de nuevo.";
        }

        return "No se pudo entrar a la sala. Intenta de nuevo.";
    }

    private void OnDestroy()
    {
        if (Instancia == this)
        {
            Instancia = null;
        }
    }
}

/// <summary>Datos minimos de una sala encontrada para mostrarlos antes de entrar.</summary>
public readonly struct InfoSalaEncontrada
{
    /// <summary>Id del jugador que la creo (todavia no hay apodos, historia futura).</summary>
    public string HostId { get; }
    public int JugadoresActuales { get; }
    public int JugadoresMaximos { get; }

    public InfoSalaEncontrada(string hostId, int jugadoresActuales, int jugadoresMaximos)
    {
        HostId = hostId;
        JugadoresActuales = jugadoresActuales;
        JugadoresMaximos = jugadoresMaximos;
    }
}
