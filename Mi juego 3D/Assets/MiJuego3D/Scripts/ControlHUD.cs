using UnityEngine; 
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ControlHUD : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI balasTexto; 
    public Image barraVidas; 

    [Header("Ventana Pausa")]
    public GameObject ventanaPausa; 

    [Header("Ventana Fin Juego")]
    public GameObject ventanaFinJuego; 
    public TextMeshProUGUI resultadoTexto; 

    [Header("Menu Mochila (Pulsar TAB)")]
    public GameObject panelMochila; 
    public TextMeshProUGUI txtBotiquines; 
    public TextMeshProUGUI txtBalasPistola; 
    public TextMeshProUGUI txtBalasRifle; 
    private bool mochilaAbierta = false; 

    public static ControlHUD instancia; 

    private void Awake()
    {
        instancia = this; // uso del patrón singleton para que todos los scripts puedan acceder a este
    }

    public void AlternarMenuInventario() 
    {
        mochilaAbierta = !mochilaAbierta; // abre el inventario

        if (panelMochila != null) // comprueba que el panel exista
        {
            panelMochila.SetActive(mochilaAbierta); // muestra el ventana del inventario en pantalla

            // permite usa el ratón en el inventario
            Cursor.lockState = mochilaAbierta ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void ActualizarTextosMochila(int botiquines, int balasP, int balasR) 
    {
        // etiquetas para el numeros de botiquines y de balas
        if (txtBotiquines != null) txtBotiquines.text = "Botiquines: " + botiquines;
        if (txtBalasRifle != null) txtBalasRifle.text = "Balas Rifle: " + balasR;
        if (txtBalasPistola != null) txtBalasPistola.text = "Balas Pistola: " + balasP;
    }

    public void actualizaBarraVida(int vidaActual, int vidaMax) 
    {
        // calcula como debe estar la barra roja de llena 
        barraVidas.fillAmount = (float)vidaActual / (float)vidaMax;
    }

    public void actualizarBalasTexto(int numBalasActual, int numBalasMax) 
    {
        //actualiza el número de balas
        balasTexto.text = "Balas : " + numBalasActual + " / " + numBalasMax;
    }

    public void CambiarEstadoVentanaPausa(bool pausa) 
    {
        //activa la pantalla de pausa
        ventanaPausa.SetActive(pausa);
    }

    public void establecerVentanaFinJuego(bool ganado) 
    {
        ventanaFinJuego.SetActive(true); // enciende la pantalla final.
        Cursor.lockState = CursorLockMode.None; // permite usar el ratón
        Time.timeScale = 0.0f; // congela el juego

        // si ganas se muestra un mensaje y si pierdes otro
        resultadoTexto.text = (ganado) ? "HAS GANADO!!" : "HAS PERDIDO!!";
        // si ganas se muestra el mensaje en verde si pierdes se muestra en rojo
        resultadoTexto.color = (ganado) ? Color.green : Color.red;
    }

    public void OnBotonMenu() 
    {
        //carga el menú
        SceneManager.LoadScene("Menu"); 
    }

    public void OnBotonVolver() 
    {
        //quita la pausa y vuelve al juego
        ControlJuego.instancia.cambiarPausa();
    }

    public void OnBotonEmpezar() 
    {
        //empieza el juego
        SceneManager.LoadScene("Nivel1"); 
    }

    public void onBotonSalir() 
    {
        //cierra el juego
        Application.Quit(); 
    }
}
