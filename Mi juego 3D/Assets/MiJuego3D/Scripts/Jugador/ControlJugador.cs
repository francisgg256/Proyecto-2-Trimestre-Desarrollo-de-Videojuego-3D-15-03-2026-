using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;
    public float fuerzaSalto = 8f;

    [Header("Camara")]
    public float sensibilidadRaton;
    public float maxVistaX;
    public float minVistaX;
    private float rotacionX;

    [Header("Vidas")]
    public int vidasActual;
    public int vidasMax;
    [Tooltip("Porcentaje de vida (0.1 = 10%)")]
    [Range(0f, 1f)]
    public float umbralAutoCura = 0.1f;

    [Header("Inventario de Armas")]
    public ControlArma[] inventarioArmas;
    private int indiceArmaActual = 0;
    private ControlArma armaActual;

    [Header("La Mochila")]
    public int botiquinesGuardados = 0;
    public int balasPistolaGuardadas = 0;
    public int balasRifleGuardadas = 0;

    private Camera camara;
    private Rigidbody fisica;

    private void Awake()
    {
        camara = Camera.main;
        fisica = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Start()
    {
        Time.timeScale = 1.0f;
        if (inventarioArmas.Length > 0) EquiparArma(0);
        ControlHUD.instancia.actualizaBarraVida(vidasActual, vidasMax);
        ControlHUD.instancia.actualizarPuntuacion(0);

        ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas);
    }

    private void Update()
    {
        //controla todas las teclas que se pulsan
        if (ControlJuego.instancia.juegoPausado) return;

        Movimiento();
        VistaCamara();

        //salta si se presiona el espacio
        if (Input.GetButtonDown("Jump")) Salto();

        // abre el inventario si se le da al tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ControlHUD.instancia.AlternarMenuInventario();
        }

        //usa un botiquin si le das a la h
        if (Input.GetKeyDown(KeyCode.H)) UsarBotiquin();
        //recarga munición si le das a la r
        if (Input.GetKeyDown(KeyCode.R)) RecargarArma();

        //cambia de arma con las teclas 1 y 2
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventarioArmas.Length > 0) EquiparArma(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && inventarioArmas.Length > 1) EquiparArma(1);

        // ddispara el aram con clic derecho y si no tiene balas la recarga automaticamente
        if (Input.GetButton("Fire1"))
        {
            if (armaActual != null && armaActual.PuedeDisparar())
            {
                armaActual.Disparar();
            }
            else if (armaActual != null && armaActual.municionActual <= 0)
            {
                RecargarArma();
            }
        }
    }

    public void GuardarEnMochila(TipoExtra tipo, int cantidad)
    {
        //controla la cantidad de coleccionables que tienes en la mochila
        switch (tipo)
        {
            case TipoExtra.Vida: botiquinesGuardados++; break;
            case TipoExtra.BalasPistola: balasPistolaGuardadas += cantidad; break;
            case TipoExtra.BalasRifle: balasRifleGuardadas += cantidad; break;
        }
        ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas);
    }

    public void UsarBotiquin()
    {
        if (botiquinesGuardados > 0 && vidasActual < vidasMax)
        {
            botiquinesGuardados--;
            IncrementaVida(5);
            ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas);
        }
    }

    public void RecargarArma()
    {
        //recarga el arma dependiendo de cual seaa y de el tipo de munición que tengas
        if (armaActual == null || armaActual.municionActual == armaActual.municionMax) return;

        int balasNecesarias = armaActual.municionMax - armaActual.municionActual;

        if (indiceArmaActual == 0 && balasRifleGuardadas > 0)
        {
            int recarga = Mathf.Min(balasNecesarias, balasRifleGuardadas);
            armaActual.municionActual += recarga;
            balasRifleGuardadas -= recarga;
        }

        else if (indiceArmaActual == 1 && balasPistolaGuardadas > 0)
        {
            int recarga = Mathf.Min(balasNecesarias, balasPistolaGuardadas);
            armaActual.municionActual += recarga;
            balasPistolaGuardadas -= recarga;
        }

        ControlHUD.instancia.actualizarBalasTexto(armaActual.municionActual, armaActual.municionMax);
        ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas);
    }

    private void EquiparArma(int indice)
    {
        for (int i = 0; i < inventarioArmas.Length; i++)
        {
            inventarioArmas[i].gameObject.SetActive(i == indice);
        }
        indiceArmaActual = indice;
        armaActual = inventarioArmas[indice];
        ControlHUD.instancia.actualizarBalasTexto(armaActual.municionActual, armaActual.municionMax);
    }

    private void Salto()
    {
        //permite saltar si se esta tocando el suelo
        Ray rayo = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(rayo, 1.1f))
        {
            fisica.linearVelocity = new Vector3(fisica.linearVelocity.x, 0, fisica.linearVelocity.z);
            fisica.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }

    private void VistaCamara()
    {
        // hace que el ratón dirija la cámara sin que pueda mirar ni muy arriba ni muy abajo
        float y = Input.GetAxis("Mouse X") * sensibilidadRaton;
        rotacionX += Input.GetAxis("Mouse Y") * sensibilidadRaton;
        rotacionX = Mathf.Clamp(rotacionX, minVistaX, maxVistaX);
        camara.transform.localRotation = Quaternion.Euler(-rotacionX, 0, 0);
        transform.eulerAngles += Vector3.up * y;
    }

    private void Movimiento()
    {
        //hace que el personaje se mueva a donde se esta mirando
        float x = Input.GetAxis("Horizontal") * velocidad;
        float z = Input.GetAxis("Vertical") * velocidad;
        Vector3 direccion = transform.right * x + transform.forward * z;
        fisica.linearVelocity = new Vector3(direccion.x, fisica.linearVelocity.y, direccion.z);
    }

    internal void QuitarVidasJugador(int cantidadVida)
    {
        //le quita vida al jugador
        vidasActual -= cantidadVida;

        // Calculamos cuántos puntos de vida reales son ese porcentaje
        // Si tienes 100 de vida máxima y el umbral es 0.1, el resultado será 10 puntos.
        //si estas a poca vida usa un botiquin de forma automática
        float puntosParaCurar = vidasMax * umbralAutoCura;

        // Si tu vida cae por debajo de ese límite, y sigues vivo, te curas.
        if (vidasActual <= puntosParaCurar && vidasActual > 0 && botiquinesGuardados > 0)
        {
            UsarBotiquin();
        }

        // muestra el cambio de vida en pantalla
        ControlHUD.instancia.actualizaBarraVida(vidasActual, vidasMax);

        if (vidasActual <= 0) TerminaJugador();
    }

    private void TerminaJugador()
    {
        ControlHUD.instancia.establecerVentanaFinJuego(false);
    }

    internal void IncrementaVida(int cantidad)
    {
        vidasActual = Mathf.Clamp(vidasActual + cantidad, 0, vidasMax);
        ControlHUD.instancia.actualizaBarraVida(vidasActual, vidasMax);
    }
}
