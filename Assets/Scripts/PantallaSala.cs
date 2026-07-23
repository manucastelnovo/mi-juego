using System.Text;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra la sala de espera: el codigo, la lista de jugadores conectados
/// (con el cupo, ej. "2/4") y quien es el anfitrion. Solo el host ve el
/// boton EMPEZAR; los clientes ven un aviso de que esperan al anfitrion.
/// La usan tanto el host (al crear la sala) como el cliente (al confirmar
/// que entra a una que encontro).
/// Escucha a GestorSala igual que MenuInicio escucha a GestorRed, y ademas
/// se suscribe a los eventos de jugadores de la sesion activa para refrescar
/// la lista en vivo sin hacer polling (historia #40). Arrancar la partida de
/// verdad con EMPEZAR es la historia #41.
/// </summary>
public class PantallaSala : MonoBehaviour
{
    [SerializeField] private GameObject panelSalaEspera;
    [SerializeField] private Text textoCodigo;
    [SerializeField] private Text textoTitulo;
    [SerializeField] private Text textoListaJugadores;
    [SerializeField] private GameObject botonEmpezar;
    [SerializeField] private GameObject textoEsperandoHost;

    // Sesion a la que estamos escuchados ahora mismo, para poder
    // desuscribirnos sin depender de que GestorSala todavia la tenga (por
    // ejemplo, despues de salir de la sala).
    private ISession sesionEscuchada;

    private void Awake()
    {
        if (panelSalaEspera != null) panelSalaEspera.SetActive(false);
    }

    private void OnEnable()
    {
        GestorSala.AlCrearSala += ManejarSalaCreada;
        GestorSala.AlEntrarSala += ManejarEntroASala;
        GestorSala.AlSalirDeSala += ManejarSalioDeSala;
    }

    private void OnDisable()
    {
        GestorSala.AlCrearSala -= ManejarSalaCreada;
        GestorSala.AlEntrarSala -= ManejarEntroASala;
        GestorSala.AlSalirDeSala -= ManejarSalioDeSala;
        DejarDeEscucharSesion();
    }

    private void ManejarSalaCreada(string codigo)
    {
        if (textoTitulo != null) textoTitulo.text = "SALA CREADA";
        MostrarSala(codigo);
    }

    private void ManejarEntroASala(string codigo)
    {
        if (textoTitulo != null) textoTitulo.text = "TE UNISTE A LA SALA";
        MostrarSala(codigo);
    }

    private void MostrarSala(string codigo)
    {
        if (textoCodigo != null) textoCodigo.text = codigo;
        if (panelSalaEspera != null) panelSalaEspera.SetActive(true);

        EscucharSesion(GestorSala.Instancia != null ? GestorSala.Instancia.SesionEnCurso : null);
        ActualizarListaJugadores();
    }

    /// <summary>Enganche del boton COPIAR: copia el codigo actual al portapapeles.</summary>
    public void OnCopiarCodigo()
    {
        if (textoCodigo == null) return;
        GUIUtility.systemCopyBuffer = textoCodigo.text;
    }

    /// <summary>
    /// Enganche del boton EMPEZAR (solo lo ve el host). Arrancar la partida
    /// de verdad para todos los jugadores es la historia #41: por ahora
    /// este metodo es el punto de entrada donde se va a enganchar.
    /// </summary>
    public void OnEmpezar()
    {
        Debug.Log("[Sala] EMPEZAR presionado (arrancar la partida es la historia #41).");
    }

    /// <summary>Enganche del boton SALIR: deja la sala y vuelve al menu de inicio.</summary>
    public void OnSalir()
    {
        if (GestorSala.Instancia != null) GestorSala.Instancia.SalirDeSala();
    }

    /// <summary>
    /// Cuando se sale de la sala (propia o por error) se limpia todo: se
    /// deja de escuchar la sesion vieja y no quedan nombres de una sala
    /// anterior si se vuelve a entrar a otra despues.
    /// </summary>
    private void ManejarSalioDeSala()
    {
        DejarDeEscucharSesion();
        if (panelSalaEspera != null) panelSalaEspera.SetActive(false);
        if (textoListaJugadores != null) textoListaJugadores.text = string.Empty;
        if (textoCodigo != null) textoCodigo.text = string.Empty;
    }

    /// <summary>
    /// Se suscribe a los eventos de la sesion (PlayerJoined/PlayerHasLeft)
    /// para refrescar la lista cuando entra o sale alguien, sin hacer
    /// polling. Antes de suscribirse a una sesion nueva, se desuscribe de
    /// la anterior si quedo alguna colgada.
    /// </summary>
    private void EscucharSesion(ISession sesion)
    {
        DejarDeEscucharSesion();
        sesionEscuchada = sesion;
        if (sesionEscuchada == null) return;

        sesionEscuchada.PlayerJoined += ManejarCambioDeJugadores;
        sesionEscuchada.PlayerHasLeft += ManejarCambioDeJugadores;
    }

    private void DejarDeEscucharSesion()
    {
        if (sesionEscuchada == null) return;
        sesionEscuchada.PlayerJoined -= ManejarCambioDeJugadores;
        sesionEscuchada.PlayerHasLeft -= ManejarCambioDeJugadores;
        sesionEscuchada = null;
    }

    private void ManejarCambioDeJugadores(string idJugador)
    {
        ActualizarListaJugadores();
    }

    /// <summary>
    /// Redibuja la lista de jugadores (con el cupo y quien es el anfitrion)
    /// y decide si mostrar EMPEZAR (host) o el aviso de espera (cliente).
    /// </summary>
    private void ActualizarListaJugadores()
    {
        if (sesionEscuchada == null) return;

        if (textoListaJugadores != null)
        {
            string miId = GestorRed.Instancia != null ? GestorRed.Instancia.IdJugador : null;
            StringBuilder texto = new StringBuilder();
            texto.AppendLine($"JUGADORES {sesionEscuchada.PlayerCount}/{sesionEscuchada.MaxPlayers}");

            foreach (IReadOnlyPlayer jugador in sesionEscuchada.Players)
            {
                texto.Append(IdCorto(jugador.Id));
                if (jugador.Id == sesionEscuchada.Host) texto.Append(" (anfitrion)");
                if (jugador.Id == miId) texto.Append(" (vos)");
                texto.AppendLine();
            }

            textoListaJugadores.text = texto.ToString().TrimEnd();
        }

        bool soyHost = sesionEscuchada.IsHost;
        if (botonEmpezar != null) botonEmpezar.SetActive(soyHost);
        if (textoEsperandoHost != null) textoEsperandoHost.SetActive(!soyHost);
    }

    /// <summary>Todavia no hay apodos de jugador (historia futura): se muestra un fragmento corto del id.</summary>
    private static string IdCorto(string id)
    {
        if (string.IsNullOrEmpty(id)) return id;
        return id.Length > 8 ? id.Substring(0, 8) : id;
    }
}
