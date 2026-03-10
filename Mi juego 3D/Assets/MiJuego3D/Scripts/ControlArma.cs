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
        // llama al script poolObjetos
        pool = GetComponent<PoolObjetos>();
    }

    public bool PuedeDisparar() 
    {
        //solo se puede dispara si ya ha pasado el cooldown, si tienes mas de 0 balas o si tienes munición infinita
        return Time.time >= tiempoProximoDisparo && (municionActual > 0 || municionInfinita);
    }

    public void Disparar() 
    {
        // le añade el cooldown al aram
        tiempoProximoDisparo = Time.time + frecuenciaDisparo;

        // si no tiene munición infinita
        if (!municionInfinita)
        {
            municionActual--; // resta una bala 

            if (ControlHUD.instancia != null) 
            {
                // actualiza el número de balas
                ControlHUD.instancia.actualizarBalasTexto(municionActual, municionMax);
            }
        }

        if (pool != null)
        {
            // coge una bala
            GameObject bala = pool.getObjeto();

            if (bala != null) 
            {

                // se crea un rayo desde el centro de la pantalla
                Ray rayoCentroPantalla = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit impacto; 
                Vector3 puntoDestino; 

                // dispara ese láser 100 metros hacia adelante.
                if (Physics.Raycast(rayoCentroPantalla, out impacto, 100f))
                {
                    // si choca con algo se guarda el punto exacto como nuestro destino.
                    puntoDestino = impacto.point;
                }
                else
                {
                    // si no choca con nada se pone como destino un punto a 100 metros.
                    puntoDestino = rayoCentroPantalla.GetPoint(100f);
                }

                //la operación permite saber la dirección hacia donde tiene que ir la bala, normalized reduce la longitud de la flecha para que no se altere la velocidad de la bala
                Vector3 direccionDisparo = (puntoDestino - puntoSalida.position).normalized;

                // pone la bala en el punto de salida
                bala.transform.position = puntoSalida.position;

                // rota la bala para que mire hacia la dirección que calculamos
                bala.transform.rotation = Quaternion.LookRotation(direccionDisparo);

                // activa la bala
                bala.SetActive(true);

                Rigidbody rb = bala.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // pone la velocidad de la bala a 0
                    rb.linearVelocity = Vector3.zero;

                    // empuja la bala a la direccion indicada
                    rb.AddForce(direccionDisparo * velocidadBala, ForceMode.Impulse);
                }
            }
        }
    }
}