using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Botonera minima solo para desarrollo: arranca Host o Cliente por
/// conexion directa (sin Relay/Lobby, que todavia esperan el proyecto de
/// Unity Cloud vinculado). Sirve para poder probar #32/#33 en Play mode con
/// los jugadores virtuales de Multiplayer Play Mode mientras no existe un
/// menu de salas real (historia futura). No corre fuera del editor o de
/// builds de desarrollo, igual que el input de teclado de PlayerController.
/// </summary>
#if UNITY_EDITOR || DEVELOPMENT_BUILD
public class ConexionDev : MonoBehaviour
{
    // Flags que se pueden togglear desde el Inspector (o el bridge de
    // automatizacion) para arrancar Host/Cliente sin tener que clickear los
    // botones de OnGUI; utiles para pruebas automatizadas en Play mode.
    public bool ordenIniciarHost;
    public bool ordenIniciarCliente;

    private void Update()
    {
        NetworkManager gestor = NetworkManager.Singleton;
        if (gestor == null) return;

        if (ordenIniciarHost)
        {
            ordenIniciarHost = false;
            if (!gestor.IsClient && !gestor.IsServer) gestor.StartHost();
        }
        if (ordenIniciarCliente)
        {
            ordenIniciarCliente = false;
            if (!gestor.IsClient && !gestor.IsServer) gestor.StartClient();
        }
    }

    private void OnGUI()
    {
        NetworkManager gestor = NetworkManager.Singleton;
        if (gestor == null) return;

        if (gestor.IsClient || gestor.IsServer)
        {
            GUI.Label(new Rect(10, 10, 300, 20),
                gestor.IsHost ? "Conectado como Host" : "Conectado como Cliente");
            return;
        }

        if (GUI.Button(new Rect(10, 10, 100, 30), "Host"))
        {
            gestor.StartHost();
        }
        if (GUI.Button(new Rect(120, 10, 100, 30), "Cliente"))
        {
            gestor.StartClient();
        }
    }
}
#endif
