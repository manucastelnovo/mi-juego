using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Harness de pruebas SOLO para desarrollo (E3, #69): spawnea Rastreros y
/// jugadores de prueba server-side para poder validar persecucion, dano por
/// contacto, retargeteo y muerte de un espadazo sin depender del gestor de
/// oleadas (todavia no existe, es la E4) ni de una segunda pantalla real.
/// La secuencia corre sola (Start) porque las herramientas de automatizacion
/// no pueden togglear propiedades mientras el juego esta en Play; en el
/// Inspector, en modo edicion, se puede desactivar con autoEjecutar. El
/// spawn definitivo de enemigos (oleadas) es responsabilidad de una historia
/// futura.
/// </summary>
#if UNITY_EDITOR || DEVELOPMENT_BUILD
public class PruebaE3Rastrero : MonoBehaviour
{
    [Header("Prefabs (asignar en el Inspector)")]
    [SerializeField] private NetworkObject prefabRastrero;
    [SerializeField] private NetworkObject prefabJugador;

    [Header("Secuencia automatica")]
    [Tooltip("Si esta activo, al arrancar el host corre sola la secuencia de prueba.")]
    [SerializeField] private bool autoEjecutar = true;

    [Tooltip("Segundos de espera tras detectar el host antes de empezar a spawnear.")]
    [SerializeField] private float esperaInicial = 1f;

    [Tooltip("Cada cuantos segundos se loguea el estado de jugadores y enemigos (solo consola del host).")]
    [SerializeField] private float intervaloLog = 1f;

    // Player1 real (el del host) aparece en PlayerSpawn_1 (-15,-2): el
    // Rastrero1 spawnea pegado a el para forzar el dano por contacto hasta
    // que caiga. JugadorFalsoLejos queda fuera de alcance para verificar que,
    // al caer Player1, el Rastrero1 retargetea hacia el en vez de quedarse quieto.
    [Header("Puntos de spawn de prueba")]
    [SerializeField] private Vector3 puntoSpawnRastrero1 = new Vector3(-14f, -1f, 0f);
    [SerializeField] private Vector3 puntoSpawnJugadorFalsoLejos = new Vector3(10f, -2f, 0f);

    // JugadorFalsoEspada + Rastrero2 se spawnean pegados entre si, lejos de
    // los anteriores, solo para probar la muerte de un espadazo sin
    // interferir con la prueba de dano por contacto de arriba.
    [SerializeField] private Vector3 puntoSpawnJugadorFalsoEspada = new Vector3(0f, -1f, 0f);
    [SerializeField] private Vector3 puntoSpawnRastrero2 = new Vector3(0.8f, -1f, 0f);

    private void Start()
    {
        if (!autoEjecutar) return;
        StartCoroutine(EjecutarSecuencia());
    }

    private IEnumerator EjecutarSecuencia()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer);
        yield return new WaitForSeconds(esperaInicial);

        Debug.Log("[PruebaE3] Arranca la secuencia de prueba del Rastrero (#69).");

        SpawnJugador(puntoSpawnJugadorFalsoLejos, "JugadorFalsoLejos");
        SpawnRastrero(puntoSpawnRastrero1, "Rastrero1_Contacto");

        NetworkObject jugadorEspada = SpawnJugador(puntoSpawnJugadorFalsoEspada, "JugadorFalsoEspada");
        SpawnRastrero(puntoSpawnRastrero2, "Rastrero2_Espada");

        // Deja que ambos terminen de caer y acomodarse en el suelo antes de
        // probar el espadazo: si el jugador todavia esta en el aire, el corte
        // contextual apunta hacia abajo en vez de hacia el Rastrero2 pegado.
        yield return new WaitForSeconds(1.2f);

        AtaqueEspada espada = jugadorEspada != null ? jugadorEspada.GetComponent<AtaqueEspada>() : null;
        if (espada != null)
        {
            Debug.Log("[PruebaE3] JugadorFalsoEspada ataca a Rastrero2 (debe morir de un solo golpe).");
            espada.Atacar();
        }
        else
        {
            Debug.LogWarning("[PruebaE3] No se encontro AtaqueEspada en JugadorFalsoEspada.");
        }

        // Sigue logueando el estado (dano por contacto y retargeteo del
        // Rastrero1 al caer Player1) durante el resto de la prueba.
        while (true)
        {
            yield return new WaitForSeconds(intervaloLog);
            LogEstado();
        }
    }

    private NetworkObject SpawnRastrero(Vector3 posicion, string nombre)
    {
        if (prefabRastrero == null)
        {
            Debug.LogWarning("[PruebaE3] Falta asignar prefabRastrero.");
            return null;
        }
        NetworkObject instancia = Instantiate(prefabRastrero, posicion, Quaternion.identity);
        instancia.name = nombre;
        instancia.Spawn(true);
        return instancia;
    }

    // Jugador extra server-side (sin cliente real detras) para probar el
    // retargeteo por cercania y la muerte de un espadazo con 2+ jugadores
    // en una sola pantalla.
    private NetworkObject SpawnJugador(Vector3 posicion, string nombre)
    {
        if (prefabJugador == null)
        {
            Debug.LogWarning("[PruebaE3] Falta asignar prefabJugador.");
            return null;
        }
        NetworkObject instancia = Instantiate(prefabJugador, posicion, Quaternion.identity);
        instancia.name = nombre;
        instancia.Spawn(true);
        return instancia;
    }

    // Estado legible por consola: vida de jugadores y si cada enemigo sigue vivo.
    private void LogEstado()
    {
        foreach (SaludJugador j in FindObjectsByType<SaludJugador>(FindObjectsSortMode.None))
        {
            Debug.Log($"[PruebaE3] Jugador {j.name} vida={j.VidaActual}/{j.VidaMaxima} pos={j.transform.position}");
        }
        foreach (SaludEnemigo e in FindObjectsByType<SaludEnemigo>(FindObjectsSortMode.None))
        {
            Debug.Log($"[PruebaE3] Rastrero {e.name} muerto={e.Muerto} pos={e.transform.position}");
        }
    }
}
#endif
