using UnityEngine;
using UnityEngine.AI;

public enum EstadoIA { 
    //enumerado con los distintos estados en los que puede estar el enemigo
    Patrullando, 
    Persiguiendo 
}

public class ControlEnemigoMejorado : MonoBehaviour
{
    public EstadoIA estadoActual = EstadoIA.Patrullando;

    [Header("Estadística")]
    public int vidasActual;
    public int vidasMax;
    public int puntuacionEnemigo;

    [Header("Movimiento e IA")]
    public float radioDeteccion = 10f; 
    public float rangoAtaque = 3f;     
    public Transform[] puntosPatrulla; 
    private int indicePatrulla = 0;

    private NavMeshAgent agente; 
    private ControlArma arma;
    private GameObject objetivo;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        arma = GetComponent<ControlArma>();
        objetivo = GameObject.FindGameObjectWithTag("Jugador");
    }

    void Start()
    {
        //comprueba si el enemigo esta en una psoición valida del navmesh si no esta en una posicion valida hace tp a una valida en un rang de 5 metros
        if (agente != null && NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            agente.Warp(hit.position);
        }
    }

    void Update()
    {
        //hace que ele enemigo se comporte de una forma o otra dependiendo de si esta patrullando o persiguiendo

        if (agente == null || !agente.isOnNavMesh) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.transform.position);

        switch (estadoActual)
        {
            case EstadoIA.Patrullando:
                ComportamientoPatrulla();
                if (distanciaAlJugador < radioDeteccion)
                    estadoActual = EstadoIA.Persiguiendo;
                break;

            case EstadoIA.Persiguiendo:
                ComportamientoPersecucion(distanciaAlJugador);

                //hace que el enemigo siga persiguiendo sin disparar hasta que salgas del rango
                if (distanciaAlJugador > radioDeteccion * 1.2f)
                {
                    agente.isStopped = false; 
                    estadoActual = EstadoIA.Patrullando;
                }
                break;
        }
    }

    void ComportamientoPatrulla()
    {
        if (puntosPatrulla.Length == 0) return;

        agente.isStopped = false;

        agente.SetDestination(puntosPatrulla[indicePatrulla].position);

        if (!agente.pathPending && agente.remainingDistance < 0.6f)
        {
            indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
        }
    }

    void ComportamientoPersecucion(float distancia)
    {
        if (distancia <= rangoAtaque)
        {
            // si estas en rango de ataque con el enemigo se para, te apunta y dispara
            agente.isStopped = true;

            Vector3 direccion = (objetivo.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            if (arma.PuedeDisparar()) arma.Disparar();
        }
        else
        {
            agente.isStopped = false;
            agente.SetDestination(objetivo.transform.position);
        }
    }

    public void QuitarVidasEnemigo(int cantidad)
    {
        vidasActual -= cantidad;
        if (vidasActual <= 0)
        {
            ControlJuego.instancia.PonerPuntuacion(puntuacionEnemigo);

            ControlJuego.instancia.EnemigoDerrotado();

            Destroy(gameObject);
        }
    }
}