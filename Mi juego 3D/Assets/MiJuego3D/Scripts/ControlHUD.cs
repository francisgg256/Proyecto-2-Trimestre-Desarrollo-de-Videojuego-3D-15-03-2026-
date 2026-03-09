using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ControlHUD : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI puntuacionTexto;
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
        //patrón singleton para que cualquier script pueda llamarlo
        instancia = this;
    }

    public void AlternarMenuInventario()
    {
        mochilaAbierta = !mochilaAbierta;
        //encienda la pantalla del inventario
        if (panelMochila != null)
        {
            panelMochila.SetActive(mochilaAbierta);
            //activa el raton para usar el inventario y lo descativa cuando sale del inventario
            Cursor.lockState = mochilaAbierta ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void ActualizarTextosMochila(int botiquines, int balasP, int balasR)
    {
        //actualiza el texto del inventario dependiendo de la cantidad de coleccionables que tengas
        if (txtBotiquines != null) txtBotiquines.text = "Botiquines: " + botiquines;
        if (txtBalasRifle != null) txtBalasRifle.text = "Balas Rifle: " + balasR;
        if (txtBalasPistola != null) txtBalasPistola.text = "Balas Pistola: " + balasP;
    }

    public void actualizaBarraVida(int vidaActual, int vidaMax)
    {
        //divide la vida actual y la vida máxima para rellenar la mitad de la vida
        barraVidas.fillAmount = (float)vidaActual / (float)vidaMax;
    }

    public void actualizarBalasTexto(int numBalasActual, int numBalasMax)
    {
        //actualiza el texto de la munición
        balasTexto.text = "Balas : " + numBalasActual + " / " + numBalasMax;
    }

    public void actualizarPuntuacion(int puntuacion)
    {
        //actualiza el texto de la puntuación
        puntuacionTexto.text = puntuacion.ToString("00000");
    }

    public void CambiarEstadoVentanaPausa(bool pausa)
    {
        //cambia a la ventana de pausa
        ventanaPausa.SetActive(pausa);
    }

    public void establecerVentanaFinJuego(bool ganado)
    {
        //muestra la ventana fin de juego con un texto distinto dependiendo de si has perdido o ha ganado
        ventanaFinJuego.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0.0f;

        resultadoTexto.text = (ganado) ? "HAS GANADO!!" : "HAS PERDIDO!!";
        resultadoTexto.color = (ganado) ? Color.green : Color.red;
    }

    public void OnBotonMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnBotonVolver()
    {
        ControlJuego.instancia.cambiarPausa();
    }

    public void OnBotonEmpezar()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void onBotonSalir()
    {
        Application.Quit();
    }
}
