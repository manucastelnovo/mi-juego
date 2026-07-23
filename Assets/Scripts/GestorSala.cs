using System;
using Unity.Services.Multiplayer;
using UnityEngine;

/// <summary>
/// Crea la sala como host usando el servicio de Multiplayer de Unity (Lobby +
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

    /// <summary>La sesion activa como host, una vez creada. Null si todavia no hay sala.</summary>
    public IHostSession SesionActual { get; private set; }

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

    private void OnDestroy()
    {
        if (Instancia == this)
        {
            Instancia = null;
        }
    }
}
