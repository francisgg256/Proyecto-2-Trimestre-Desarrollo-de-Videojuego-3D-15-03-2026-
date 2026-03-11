using UnityEngine;
using UnityEngine.AI;

public enum EstadoIA
{
    //enumerado para los estados del enemigo
    Patrullando,
    Persiguiendo
}

public class ControlEnemigoMejorado : MonoBehaviour
{
    public EstadoIA estadoActual = EstadoIA.Patrullando;

    [Header("Estadística")]
    public int vidasActual;
    public int vidasMax;

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
        //comprueba si el enemigo no se queda flotando o por debajo del suelo
        if (agente != null && NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            // si no esta en un punto valido se hace tp hasta el punto valido mas cercano
            agente.Warp(hit.position);
        }
    }

    void Update()
    {
        // --- LA LÍNEA MÁGICA: Si el juego está pausado (ej: mirando el inventario), el enemigo deja de pensar y disparar.
        if (ControlJuego.instancia != null && ControlJuego.instancia.juegoPausado) return;

        if (agente == null || !agente.isOnNavMesh) return;

        //calcula la distancia entre el enemigo y el jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.transform.position);

        switch (estadoActual) // comprueba el estado del enemigo
        {
            case EstadoIA.Patrullando:
                ComportamientoPatrulla(); // camina por los puntos de patrullaje

                // si ell jugador entra en el rango de detección el enemigo cambia al estaado de persiguiendo
                if (distanciaAlJugador < radioDeteccion)
                    estadoActual = EstadoIA.Persiguiendo;
                break;

            case EstadoIA.Persiguiendo:
                ComportamientoPersecucion(distanciaAlJugador); // persigue al jugador y dispara si lo tiene a tiro

                //para que el enemigo no se vuelva loco usamos la histéresis que basicamente es que el jugador se tiene que alejar un rango especifico para que el enemigo vuelva a patrullar
                if (distanciaAlJugador > radioDeteccion * 1.2f)
                {
                    agente.isStopped = false; // asegura que el enemigo se vuelva a mover
                    estadoActual = EstadoIA.Patrullando; // el enemigo vuelve al estado inicial
                }
                break;
        }
    }

    void ComportamientoPatrulla()
    {
        if (puntosPatrulla.Length == 0) return;

        agente.isStopped = false; // permite que el enemigo ande

        // calcula la ruta hasta el siguiente punto de patrullaje
        agente.SetDestination(puntosPatrulla[indicePatrulla].position);

        // cuando el enemigo toca el punto de pattrulla pasa al siguiente
        if (!agente.pathPending && agente.remainingDistance < 0.6f)
        {
            indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
        }
    }

    void ComportamientoPersecucion(float distancia)
    {
        // si el jugador esta dentro del rango de ataque el enemigo se para para apuntar y disparar
        if (distancia <= rangoAtaque)
        {
            agente.isStopped = true;

            // calcula la dirección para apuntar al jugador
            Vector3 direccion = (objetivo.transform.position - transform.position).normalized;
            // calcula la rotación para apuntar al jugador
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));
            // hace que el enemigo rote de forma suave
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // si el enemigo puede disparar dispara
            if (arma.PuedeDisparar()) arma.Disparar();
        }
        else // si el jugador esta en el rango de deteccion pero no en el de ataque 
        {
            agente.isStopped = false; // el enemigo sigue persiguiendo al jugador
            agente.SetDestination(objetivo.transform.position);
        }
    }

    public void QuitarVidasEnemigo(int cantidad)
    {
        vidasActual -= cantidad; //resta vida
        if (vidasActual <= 0) // si tiene 0 de vida
        {
            // se resta un enemigo
            ControlJuego.instancia.EnemigoDerrotado();

            // destruye el enemigo
            Destroy(gameObject);
        }
    }
}