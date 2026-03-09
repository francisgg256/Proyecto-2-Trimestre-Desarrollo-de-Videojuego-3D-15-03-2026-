using UnityEngine;

public class ControlJuego : MonoBehaviour
{
    public int puntuacionActual;
    public bool juegoPausado;
    public static ControlJuego instancia;

    private int enemigosRestantes;

    public void Awake()
    {
        //patrón singleton para que cualquier script pueda llamarlo
        if (instancia == null) instancia = this;
    }

    private void Start()
    {
        //se guarda el número de enemigos buscandolos por etiqueta
        enemigosRestantes = GameObject.FindGameObjectsWithTag("Enemigo").Length;
    }

    private void Update()
    {
        //escucha si se pulsa el botón de escape para poner el menú de pausa
        if (Input.GetButtonDown("Cancel"))
        {
            cambiarPausa();
        }
    }

    public void cambiarPausa()
    {
        juegoPausado = !juegoPausado;
        //para el juego para que no siga funcionando mientras esta en pausa
        Time.timeScale = (juegoPausado) ? 0.0f : 1.0f;
        //desbloquea el ratón para poder usarlo en el menú
        Cursor.lockState = (juegoPausado) ? CursorLockMode.None : CursorLockMode.Locked;

        //avisa al hud para poner el menú de pausa
        ControlHUD.instancia.CambiarEstadoVentanaPausa(juegoPausado);
    }

    public void PonerPuntuacion(int puntuacion)
    {
        //suma la puntuación
        puntuacionActual += puntuacion;
        //avisa al hud para que cambie la puntuación
        ControlHUD.instancia.actualizarPuntuacion(puntuacionActual);
    }

    public void EnemigoDerrotado()
    {
        //resta uno a los enemigos restantes
        enemigosRestantes--; 
        Debug.Log("Enemigo eliminado. Quedan: " + enemigosRestantes);

        //si los enemigos llegan a 0 se llama a ganar juego
        if (enemigosRestantes <= 0)
        {
            ganarJuego();
        }
    }

    public void ganarJuego()
    {
        //llama a la pantalla de victoria 
        ControlHUD.instancia.establecerVentanaFinJuego(true);
        //congela el tiempo
        Time.timeScale = 0f; 
        //activa el ratón
        Cursor.lockState = CursorLockMode.None; 
    }
}

