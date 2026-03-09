using UnityEngine;

public class ControlArma : MonoBehaviour
{
    [Header("Estadísticas de Munición")]
    public int municionActual;
    public int municionMax;
    public bool municionInfinita;

    [Header("Configuración de Disparo")]
    public Transform puntoSalida;
    public float velocidadBala = 40f;
    public float frecuenciaDisparo = 0.5f;
    private float tiempoProximoDisparo = 0f;

    private PoolObjetos pool;

    void Awake()
    {
        pool = GetComponent<PoolObjetos>();
    }

    public bool PuedeDisparar()
    {
        //comprueba que el jugador puede disparar según el tiempo entre disparo dispara mas rapido o mas lento
        return Time.time >= tiempoProximoDisparo && (municionActual > 0 || municionInfinita);
    }

    public void Disparar()
    {
        //evita que se pueda disparar todo el rato
        tiempoProximoDisparo = Time.time + frecuenciaDisparo;

        //comprueba si el arma tiene una munición específica y te resta las balas 
        if (!municionInfinita)
        {
            municionActual--;

            if (ControlHUD.instancia != null)
            {
                ControlHUD.instancia.actualizarBalasTexto(municionActual, municionMax);
            }
        }

        if (pool != null)
        {
            //llama a la bala
            GameObject bala = pool.getObjeto();

            //teletransporta la bala invisible a la punta de tu pistola, la orienta y la enciende.
            if (bala != null)
            {
                bala.transform.position = puntoSalida.position;
                bala.transform.rotation = puntoSalida.rotation;
                bala.SetActive(true);

                //dispara la bala usando las físicas
                Rigidbody rb = bala.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(puntoSalida.forward * velocidadBala, ForceMode.Impulse);
                }
            }
        }
    }
}
