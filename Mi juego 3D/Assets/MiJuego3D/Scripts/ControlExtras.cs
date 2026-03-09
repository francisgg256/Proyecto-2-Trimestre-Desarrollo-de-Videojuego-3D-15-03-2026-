using UnityEngine;

public enum TipoExtra
{
    //enumerado para elegir el tipo de coleccionable 
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
        //comprueba si el jugador a chocado con el coleccionable
        if (other.CompareTag("Jugador"))
        {
            ControlJugador jugador = other.GetComponent<ControlJugador>();

            // mete el objeto en la mochila con su tipo y la cantidad
            jugador.GuardarEnMochila(tipo, cantidad);
            //destruye el objeto
            Destroy(gameObject);
        }
    }
}
