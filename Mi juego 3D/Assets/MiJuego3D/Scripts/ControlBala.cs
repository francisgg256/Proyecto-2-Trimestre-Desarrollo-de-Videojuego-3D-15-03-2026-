using UnityEngine; 

public class ControlBala : MonoBehaviour 
{
    public GameObject particulasExplosion; 
    public int cantidadVida; 
    public float tiempoActivo;
    private float tiempoDisparo; 
 
    public void OnEnable()
    {
        // guarda el tiempo en el que se disparo la bala
        tiempoDisparo = Time.time;
    }

    private void Update() 
    {
        // comprueba si la bala lleva mucho tiempo en el mapa sin tocar ningun objetivo
        if (Time.time - tiempoDisparo >= tiempoActivo)
        {
            // si esto se cumple desactiva la bala
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // comprueba si le ha dado al jugador
        if (other.CompareTag("Jugador"))
        {
            // le quita vida al jugador
            other.GetComponent<ControlJugador>().QuitarVidasJugador(cantidadVida);
        }
        // comprueba si le ha dado a un enemigo
        else if (other.CompareTag("Enemigo"))
        {
            //le quita vida al enemigo
            ControlEnemigoMejorado enemigo = other.GetComponent<ControlEnemigoMejorado>();
            if (enemigo != null)
            {
                enemigo.QuitarVidasEnemigo(cantidadVida);
            }
        }

        // muestra el efecto justo donde la bala colisiona
        GameObject particulas = Instantiate(particulasExplosion, transform.position, Quaternion.identity);

        // destruye las partículas 
        Destroy(particulas, 1f);

        // se desactiva la bala
        gameObject.SetActive(false);
    }
}
