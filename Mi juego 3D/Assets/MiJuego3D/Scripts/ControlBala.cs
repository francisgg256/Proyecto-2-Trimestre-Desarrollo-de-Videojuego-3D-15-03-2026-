using UnityEngine;

public class ControlBala : MonoBehaviour
{
    public GameObject particulasExplosion;
    public int cantidadVida;
    public float tiempoActivo;
    private float tiempoDisparo;

    public void OnEnable()
    {
        //cada vez que se dispara el tiempo se reinicia a 0
        tiempoDisparo = Time.time;
    }

    private void Update()
    {
        //comprueba si la bala esta mas tiempo del permitido y la desactiva si es asi
        if (Time.time - tiempoDisparo >= tiempoActivo) gameObject.SetActive(false);
    }
    public void OnTriggerEnter(Collider other)
    {
        //diferencia si ha disparado al jugador o al enemigo
        if (other.CompareTag("Jugador"))
        {
            //le quita vidas al jugador
            other.GetComponent<ControlJugador>().QuitarVidasJugador(cantidadVida);
        }
        else if (other.CompareTag("Enemigo"))
        {
            //le quita vidas al enemigo
            ControlEnemigoMejorado enemigo = other.GetComponent<ControlEnemigoMejorado>();
            if (enemigo != null)
            {
                enemigo.QuitarVidasEnemigo(cantidadVida);
            }
        }

        // crea las partículas de choque de la bala y las destruye después de un segundo
        GameObject particulas = Instantiate(particulasExplosion, transform.position, Quaternion.identity);
        Destroy(particulas, 1f);

        gameObject.SetActive(false);
    }
}
