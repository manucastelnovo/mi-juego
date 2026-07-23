using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

/// <summary>
/// Arranca los servicios de Unity y abre una sesion anonima al iniciar el
/// juego. Es el primer eslabon del online: sin sesion abierta no se puede
/// crear ni buscar una sala. Sobrevive a los cambios de escena y avisa por
/// evento cuando la sesion esta lista, siguiendo el mismo patron que
/// Moneda.AlRecoger: la UI escucha, no pregunta.
/// </summary>
public class GestorRed : MonoBehaviour
{
    public static GestorRed Instancia { get; private set; }

    /// <summary>Se dispara cuando la sesion anonima quedo abierta.</summary>
    public static event Action AlIniciarSesion;

    /// <summary>Se dispara si la sesion no pudo abrirse, con el motivo.</summary>
    public static event Action<string> AlFallarSesion;

    /// <summary>True cuando hay sesion valida contra los servicios de Unity.</summary>
    public bool SesionIniciada { get; private set; }

    /// <summary>Identificador del jugador que devuelve el servicio.</summary>
    public string IdJugador { get; private set; }

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

    private async void Start()
    {
        await IniciarSesionAnonima();
    }

    /// <summary>
    /// Inicializa los servicios y abre sesion anonima. Es seguro llamarla
    /// varias veces: si ya hay sesion, no hace nada.
    /// </summary>
    public async Task IniciarSesionAnonima()
    {
        if (SesionIniciada) return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            SesionIniciada = true;
            IdJugador = AuthenticationService.Instance.PlayerId;
            Debug.Log($"[Red] Sesion anonima iniciada. Jugador: {IdJugador}");
            AlIniciarSesion?.Invoke();
        }
        catch (Exception e)
        {
            // Sin conexion o proyecto mal vinculado: el juego sigue abriendo,
            // pero el menu tiene que poder avisar que no hay online.
            SesionIniciada = false;
            Debug.LogError($"[Red] No se pudo iniciar sesion: {e.Message}");
            AlFallarSesion?.Invoke(e.Message);
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
