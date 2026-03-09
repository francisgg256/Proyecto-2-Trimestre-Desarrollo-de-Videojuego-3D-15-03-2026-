using NUnit.Framework;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;
using System;
public class ControlEnemigo : MonoBehaviour
{
    [Header("Estadística")]
    public int vidasActual;
    public int vidasMax;
    public int puntuacionEnemigo;

    [Header("Movimiento")]
    public float velocidad;
    public float rangoAtaque;
    private List<Vector3> listaCaminos;
    private ControlArma arma;
    private GameObject objetivo;
    void Start()
    {
        arma = GetComponent<ControlArma>();
        objetivo = GameObject.FindGameObjectWithTag("Jugador");
        // Cada medio segundo repite la creación de la lista de caminos
        InvokeRepeating("ActualizarCaminos", 0.0f, 0.2f);
    }

    void ActualizarCaminos()
    {
        NavMeshPath caminoCalulado = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, objetivo.transform.position, NavMesh.AllAreas, caminoCalulado);
        listaCaminos = caminoCalulado.corners.ToList();
        foreach (var item in listaCaminos)
        {
            Debug.Log(Time.time + " " + item);
        }
    }
    private void Update()
    {
        float distancia = Vector3.Distance(transform.position, objetivo.transform.position);
        if (distancia > rangoAtaque) PerseguirObjetivo();
    }
    private void PerseguirObjetivo()
    {
        if (listaCaminos.Count == 0)
            return;

        Vector3 destino = new Vector3(listaCaminos[0].x, transform.position.y, listaCaminos[0].z);
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);
        if (transform.position == destino) listaCaminos.RemoveAt(0);
    }

    public void QuitarVidasEnemigo(int cantidad)
    {
        vidasActual -= cantidad;

        if (vidasActual <= 0)
            Destroy(gameObject);
    }
}
