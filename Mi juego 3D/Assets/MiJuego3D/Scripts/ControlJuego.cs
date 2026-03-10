using UnityEngine; 

public class ControlJuego : MonoBehaviour
{
    public bool juegoPausado; 
    public static ControlJuego instancia;

    private int enemigosRestantes; 

    public void Awake() 
    {
        // uso del patrón singleton para que todos los scripts puedan acceder a este
        if (instancia == null) instancia = this;
    }

    private void Start() 
    {
        // busca los enemigos en el mapa guardando el número usando la etiqueta enemigo
        enemigosRestantes = GameObject.FindGameObjectsWithTag("Enemigo").Length;
    }

    private void Update()
    {
        // si se pulsa el escape se pausa el juego
        if (Input.GetButtonDown("Cancel"))
        {
            cambiarPausa();
        }
    }

    public void cambiarPausa() 
    {
        juegoPausado = !juegoPausado; // pone el juego en pausa

        // congela el tiempo o lo descongela dependiendo de si el juego esta pausado o no
        Time.timeScale = (juegoPausado) ? 0.0f : 1.0f;

        // si el juego esta pausado permite usar el ratón para poder navegar por el menu de pausa
        Cursor.lockState = (juegoPausado) ? CursorLockMode.None : CursorLockMode.Locked;

        // llama a ControlHUD para que muestre el menu de pausa
        ControlHUD.instancia.CambiarEstadoVentanaPausa(juegoPausado);
    }

    public void EnemigoDerrotado()
    {
        enemigosRestantes--; // resta un enemigo a los enemigos restantes
        Debug.Log("Enemigo eliminado. Quedan: " + enemigosRestantes); // muestra un mensaje de los enemigos que quedan

        // comprueba si quedan 0 enemigos
        if (enemigosRestantes <= 0)
        {
            ganarJuego(); // si no hay enemigos se gana el juego
        }
    }

    public void ganarJuego()
    {
        // llama a ControlHUD para que muestre la ventana de fin de juego
        ControlHUD.instancia.establecerVentanaFinJuego(true);

        Time.timeScale = 0f; // congela el juego

        Cursor.lockState = CursorLockMode.None; // permite usar el ratón para poder 
    }
}