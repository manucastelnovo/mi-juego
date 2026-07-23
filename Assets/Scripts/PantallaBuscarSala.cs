using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel de "BUSCAR SALA": el jugador pega el codigo, lo busca y si existe ve
/// quien la creo y cuantos hay antes de confirmar que entra. Escucha a
/// GestorSala igual que MenuInicio escucha a GestorRed: pide la accion y
/// espera el resultado por evento, sin preguntar directamente al servicio.
/// </summary>
public class PantallaBuscarSala : MonoBehaviour
{
    [SerializeField] private GameObject grupoFormulario;
    [SerializeField] private InputField inputCodigo;
    [SerializeField] private Button botonBuscar;
    [SerializeField] private Text textoEstado;
    [SerializeField] private GameObject grupoResultado;
    [SerializeField] private Text textoInfoSala;
    [SerializeField] private Button botonEntrar;

    // Misma animacion de puntos que usa MenuInicio mientras crea la sala,
    // para que buscar tampoco parezca una pantalla congelada.
    private static readonly string[] PasosBuscando = { "Buscando sala.", "Buscando sala..", "Buscando sala..." };
    private Coroutine animacionBuscando;

    // BuscarSala() ya une de verdad al jugador a la sala (el SDK no permite
    // "solo mirar" una sala privada). Este flag distingue "encontre la sala y
    // ya estoy adentro, esperando que confirme" de "todavia no busque nada" o
    // "la busqueda fallo", para que VOLVER sepa si tiene que liberar el lugar.
    private bool unidoSinConfirmar;

    private void OnEnable()
    {
        ReiniciarFormulario();
        GestorSala.AlEmpezarBusqueda += ManejarEmpezarBusqueda;
        GestorSala.AlEncontrarSala += ManejarSalaEncontrada;
        GestorSala.AlFallarBusqueda += ManejarFalloBusqueda;
    }

    private void OnDisable()
    {
        GestorSala.AlEmpezarBusqueda -= ManejarEmpezarBusqueda;
        GestorSala.AlEncontrarSala -= ManejarSalaEncontrada;
        GestorSala.AlFallarBusqueda -= ManejarFalloBusqueda;
        DetenerAnimacionBuscando();
    }

    /// <summary>Deja el panel como al abrirlo: formulario vacio, sin resultado ni error.</summary>
    private void ReiniciarFormulario()
    {
        if (inputCodigo != null) inputCodigo.text = string.Empty;
        if (grupoFormulario != null) grupoFormulario.SetActive(true);
        if (grupoResultado != null) grupoResultado.SetActive(false);
        MostrarEstado(string.Empty);
        HabilitarBusqueda(true);
        unidoSinConfirmar = false;
    }

    private void HabilitarBusqueda(bool interactuable)
    {
        if (botonBuscar != null) botonBuscar.interactable = interactuable;
        if (inputCodigo != null) inputCodigo.interactable = interactuable;
    }

    private void MostrarEstado(string mensaje)
    {
        if (textoEstado != null) textoEstado.text = mensaje;
    }

    /// <summary>Enganche del boton BUSCAR: pide a GestorSala que busque el codigo escrito.</summary>
    public void OnBuscar()
    {
        if (GestorSala.Instancia == null)
        {
            MostrarEstado("El servicio de salas no esta disponible. Intenta de nuevo.");
            return;
        }

        string codigo = inputCodigo != null ? inputCodigo.text : string.Empty;
        HabilitarBusqueda(false);
        GestorSala.Instancia.BuscarSala(codigo);
    }

    /// <summary>Enganche del boton ENTRAR: confirma y cierra este panel para dejar ver la sala de espera.</summary>
    public void OnEntrar()
    {
        if (GestorSala.Instancia == null) return;
        unidoSinConfirmar = false;
        GestorSala.Instancia.ConfirmarEntrada();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enganche del boton VOLVER: cierra este panel y vuelve al menu
    /// principal. Si ya encontramos una sala (BuscarSala nos unio de verdad)
    /// y no confirmamos la entrada, hay que liberar el lugar para no dejar
    /// un jugador fantasma ocupando cupo en la sala del host.
    /// </summary>
    public void OnVolver()
    {
        if (unidoSinConfirmar && GestorSala.Instancia != null)
        {
            GestorSala.Instancia.SalirDeSala();
        }

        unidoSinConfirmar = false;
        gameObject.SetActive(false);
    }

    private void ManejarEmpezarBusqueda()
    {
        if (grupoResultado != null) grupoResultado.SetActive(false);
        DetenerAnimacionBuscando();
        animacionBuscando = StartCoroutine(AnimarBuscando());
    }

    private void ManejarSalaEncontrada(InfoSalaEncontrada info)
    {
        DetenerAnimacionBuscando();
        MostrarEstado(string.Empty);
        unidoSinConfirmar = true;

        if (grupoFormulario != null) grupoFormulario.SetActive(false);
        if (grupoResultado != null) grupoResultado.SetActive(true);

        // Todavia no hay apodos de jugador (historia futura): se identifica
        // al host con un fragmento corto de su id mientras tanto.
        string idCorto = !string.IsNullOrEmpty(info.HostId) && info.HostId.Length > 8
            ? info.HostId.Substring(0, 8)
            : info.HostId;
        if (textoInfoSala != null)
        {
            textoInfoSala.text = $"Anfitrion: {idCorto}\n{info.JugadoresActuales}/{info.JugadoresMaximos} jugadores";
        }
    }

    private void ManejarFalloBusqueda(string motivo)
    {
        DetenerAnimacionBuscando();
        MostrarEstado(motivo);
        HabilitarBusqueda(true);
        unidoSinConfirmar = false;
    }

    private void DetenerAnimacionBuscando()
    {
        if (animacionBuscando != null)
        {
            StopCoroutine(animacionBuscando);
            animacionBuscando = null;
        }
    }

    private IEnumerator AnimarBuscando()
    {
        int paso = 0;
        while (true)
        {
            MostrarEstado(PasosBuscando[paso % PasosBuscando.Length]);
            paso++;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
