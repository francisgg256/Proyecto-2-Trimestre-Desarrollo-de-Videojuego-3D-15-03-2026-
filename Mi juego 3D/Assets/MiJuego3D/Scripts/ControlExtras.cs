using UnityEngine;

public enum TipoExtra
{
    //enumerado para elegir el tipo del coleccionable
    Vida, 
    BalasPistola, 
    BalasRifle 
}

public class ControlExtras : MonoBehaviour 
{
    public TipoExtra tipo; 
    public int cantidad; 

    private void OnTriggerEnter(Collider other)
    {
        // comprueba que el objeto lo haya tocado el jugador
        if (other.CompareTag("Jugador"))
        {
            ControlJugador jugador = other.GetComponent<ControlJugador>();

            //llama al metodo GuardarEnMochila para que se guarden segun el tipo y la cantidad
            jugador.GuardarEnMochila(tipo, cantidad);

            // se destruye el objeto para que no se siga viendo
            Destroy(gameObject);
        }
    }
}
