using UnityEngine; 

public class ControlJugador : MonoBehaviour 
{
    [Header("Movimiento")] 
    public float velocidad; 
    public float velocidadSprint = 10f; 
    public float fuerzaSalto = 8f; 

    [Header("Camara")]
    public float sensibilidadRaton; 
    public float maxVistaX; 
    public float minVistaX; 
    private float rotacionX; 

    [Header("Vidas")]
    public int vidasActual; 
    public int vidasMax; 

    [Tooltip("Porcentaje de vida")] 
    [Range(0f, 1f)] 
    public float umbralAutoCura = 0.1f; 

    [Header("Inventario de Armas")]
    public ControlArma[] inventarioArmas; 
    private int indiceArmaActual = 0; 
    private ControlArma armaActual; 

    [Header("Inventario")]
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
        Time.timeScale = 1.0f; // se asegura de que el tiempo no este congelado

        if (inventarioArmas.Length > 0) EquiparArma(0); // muestra el primer arma

        ControlHUD.instancia.actualizaBarraVida(vidasActual, vidasMax); // pinta la barra de vida
        ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas); // pone los contadores a 0
    }

    private void Update() 
    {
        if (ControlJuego.instancia.juegoPausado) return;

        Movimiento(); // llama a la funcion de movimiento
        VistaCamara(); // llama a la función de movimiento de la cámara

        if (Input.GetButtonDown("Jump")) Salto(); // si se pulsa el espacio salta

        if (Input.GetKeyDown(KeyCode.Tab)) // si se pulsa el tab se abre el inventario
        {
            ControlHUD.instancia.AlternarMenuInventario(); 
        }

        if (Input.GetKeyDown(KeyCode.H)) UsarBotiquin(); // si se pulsa la h se usa un botiquín
        if (Input.GetKeyDown(KeyCode.R)) RecargarArma(); // si se pulsa la r recarga el arma

        // si se pulsa el 1 saca la primera arma
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventarioArmas.Length > 0) EquiparArma(0);
        // si se pulsa el 2 saca la segunda arma
        if (Input.GetKeyDown(KeyCode.Alpha2) && inventarioArmas.Length > 1) EquiparArma(1);

        if (Input.GetButton("Fire1")) // si se pulsa el clic izquierdo y se tiene un arma equipada y se pueda disparar
        {
            if (armaActual != null && armaActual.PuedeDisparar())
            {
                armaActual.Disparar(); //dispara
            }
            // si el cargador está a 0 
            else if (armaActual != null && armaActual.municionActual <= 0)
            {
                RecargarArma(); // hace una recarga automática
            }
        }
    }

    public void GuardarEnMochila(TipoExtra tipo, int cantidad) 
    {
        switch (tipo) // un case que distingue el tipo de coleccionable que se recoge
        {
            case TipoExtra.Vida: botiquinesGuardados++; break; 
            case TipoExtra.BalasPistola: balasPistolaGuardadas += cantidad; break; 
            case TipoExtra.BalasRifle: balasRifleGuardadas += cantidad; break; 
        }
        // actualiza el texto
        ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas);
    }

    public void UsarBotiquin()
    {
        // solo se puede usar si se tiene un botiquín y si la vida no esta al maximo
        if (botiquinesGuardados > 0 && vidasActual < vidasMax)
        {
            botiquinesGuardados--; // gasta el botiquín
            IncrementaVida(5); // incrementa la vida
            ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas); // actualiza la barra de vida
        }
    }

    public void RecargarArma()
    {
        // si no se puede recargar corta la recargar
        if (armaActual == null || armaActual.municionActual == armaActual.municionMax) return;

        // calcula las balas que se necesita para llenar el cargador al máximo
        int balasNecesarias = armaActual.municionMax - armaActual.municionActual;

        if (indiceArmaActual == 0 && balasRifleGuardadas > 0) // si tienes el rifle 
        {
            // coge el minimo entre las balas que necesitas y las que tienes
            int recarga = Mathf.Min(balasNecesarias, balasRifleGuardadas);
            armaActual.municionActual += recarga; // mete las balas en el cargador
            balasRifleGuardadas -= recarga; // resta las balas de la mochila
        }
        else if (indiceArmaActual == 1 && balasPistolaGuardadas > 0) // lo mismo pero para la pistola
        {
            int recarga = Mathf.Min(balasNecesarias, balasPistolaGuardadas);
            armaActual.municionActual += recarga;
            balasPistolaGuardadas -= recarga;
        }

        // actualiza el texto
        ControlHUD.instancia.actualizarBalasTexto(armaActual.municionActual, armaActual.municionMax);
        ControlHUD.instancia.ActualizarTextosMochila(botiquinesGuardados, balasPistolaGuardadas, balasRifleGuardadas);
    }

    private void EquiparArma(int indice) 
    {
        // cambia de arma
        for (int i = 0; i < inventarioArmas.Length; i++)
        {
            // apaga el arma que no se ha elegido presionando la tecla correspondiente
            inventarioArmas[i].gameObject.SetActive(i == indice);
        }
        indiceArmaActual = indice; //guarda el arma actual
        armaActual = inventarioArmas[indice]; // conecta la variable al script del arma concreta.
        ControlHUD.instancia.actualizarBalasTexto(armaActual.municionActual, armaActual.municionMax); // actualiza el hud
    }

    private void Salto()
    {
        Ray rayo = new Ray(transform.position, Vector3.down); // crea un láser desde los pies apuntando hacia abajo.

        // si el laser toca el suelo se puede saltar
        if (Physics.Raycast(rayo, 1.1f))
        {
            // pone la y a cero para que no salga volando
            fisica.linearVelocity = new Vector3(fisica.linearVelocity.x, 0, fisica.linearVelocity.z);
            // empuja al personaje hacia arriba
            fisica.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }

    private void VistaCamara()
    {
        // multiplica el movimiento horizontal del ratón por la sensibilidad.
        float y = Input.GetAxis("Mouse X") * sensibilidadRaton;

        // suma el movimiento vertical del ratón
        rotacionX += Input.GetAxis("Mouse Y") * sensibilidadRaton;
        // limita el máximo y el mínimo para que no se pueda mover la camara demasiado
        rotacionX = Mathf.Clamp(rotacionX, minVistaX, maxVistaX);

        // rota la cámara hacia arriba o hacia abajo 
        camara.transform.localRotation = Quaternion.Euler(-rotacionX, 0, 0);
        // rota el jugador hacia la izquierda o la derecha
        transform.eulerAngles += Vector3.up * y;
    }

    private void Movimiento()
    {
        float velocidadActual = velocidad; 

        // si se pulsa el shift izquierdo
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadActual = velocidadSprint; // el personaje sprinta
        }

        // lee la tecla pulsada y multiplica por la velocidad
        float x = Input.GetAxis("Horizontal") * velocidadActual;
        float z = Input.GetAxis("Vertical") * velocidadActual;

        // calcula la dirección 
        Vector3 direccion = transform.right * x + transform.forward * z;

        // asigna la velocidad al Rigidbody
        fisica.linearVelocity = new Vector3(direccion.x, fisica.linearVelocity.y, direccion.z);
    }

    internal void QuitarVidasJugador(int cantidadVida) 
    {
        vidasActual -= cantidadVida; // resta vida

        // calcula el numero de puntos de vida que representan el umbral 
        float puntosParaCurar = vidasMax * umbralAutoCura;

        // si la vida esta por debajo de ese umbral se usa el botiquin
        if (vidasActual <= puntosParaCurar && vidasActual > 0 && botiquinesGuardados > 0)
        {
            UsarBotiquin(); 
        }

        ControlHUD.instancia.actualizaBarraVida(vidasActual, vidasMax); // actualiza la barra de vida

        if (vidasActual <= 0) TerminaJugador(); // si se llega a 0 mueres
    }

    private void TerminaJugador()
    {
        // llama a la pantalla final
        ControlHUD.instancia.establecerVentanaFinJuego(false);
    }

    internal void IncrementaVida(int cantidad)
    {
        // suma la vida
        vidasActual = Mathf.Clamp(vidasActual + cantidad, 0, vidasMax);
        ControlHUD.instancia.actualizaBarraVida(vidasActual, vidasMax); // actualiza el HUD
    }
}