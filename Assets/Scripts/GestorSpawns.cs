using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Asigna un punto de aparicion distinto a cada jugador que se conecta, para
/// que nadie aparezca encima de otro. Usa el callback de aprobacion de
/// conexion de Netcode (el unico lugar donde se puede fijar la posicion
/// inicial del jugador antes de que se cree su objeto de red). Si un
/// cliente se desconecta, su punto vuelve a quedar libre para el siguiente.
/// </summary>
public class GestorSpawns : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Transform[] puntosDeAparicion;

    // Indices de puntosDeAparicion todavia sin ocupar.
    private readonly List<int> puntosLibres = new List<int>();
    // Que punto ocupa cada cliente, para poder liberarlo si se desconecta.
    private readonly Dictionary<ulong, int> puntoPorCliente = new Dictionary<ulong, int>();

    private void Awake()
    {
        for (int i = 0; i < puntosDeAparicion.Length; i++)
        {
            puntosLibres.Add(i);
        }

        if (networkManager == null)
        {
            Debug.LogError("[GestorSpawns] Falta asignar el NetworkManager en el inspector.");
            return;
        }

        // La aprobacion de conexion es el unico punto donde Netcode deja
        // fijar la posicion/rotacion inicial del jugador que se conecta.
        networkManager.NetworkConfig.ConnectionApproval = true;
        networkManager.ConnectionApprovalCallback += AprobarConexion;
        networkManager.OnClientDisconnectCallback += LiberarPunto;
    }

    private void OnDestroy()
    {
        if (networkManager == null) return;

        networkManager.ConnectionApprovalCallback -= AprobarConexion;
        networkManager.OnClientDisconnectCallback -= LiberarPunto;
    }

    private void AprobarConexion(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;

        if (puntosLibres.Count == 0)
        {
            // No deberia pasar con hasta 4 jugadores, pero no se rechaza la
            // conexion: aparece en la posicion del prefab en vez de romper.
            Debug.LogWarning("[GestorSpawns] No quedan puntos de aparicion libres.");
            return;
        }

        int indice = puntosLibres[0];
        puntosLibres.RemoveAt(0);
        puntoPorCliente[request.ClientNetworkId] = indice;

        response.Position = puntosDeAparicion[indice].position;
        response.Rotation = puntosDeAparicion[indice].rotation;
    }

    private void LiberarPunto(ulong clienteId)
    {
        if (puntoPorCliente.TryGetValue(clienteId, out int indice))
        {
            puntoPorCliente.Remove(clienteId);
            puntosLibres.Add(indice);
        }
    }
}
