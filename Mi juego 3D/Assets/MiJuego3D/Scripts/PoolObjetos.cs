using System.Collections.Generic;
using UnityEngine;

public class PoolObjetos : MonoBehaviour
{
    public GameObject objetoPrefab; // la bala
    public int numObjetosOnStart; // número de balas que se crean al cargar el nivel

    private List<GameObject> objetosPooled = new List<GameObject>(); // lista donde se guardan las balas

    private void Start()
    {
        //bucle crea el número de balas que se especifica en numObjetosOnStart al empezar a jugar
        for (int x = 0; x < numObjetosOnStart; x++)
        {
            crearNuevoObjeto();
        }
    }

    private GameObject crearNuevoObjeto()
    {
        //se crea la bala
        GameObject objeto = Instantiate(objetoPrefab);
        // se desactiva para que no se vea
        objeto.SetActive(false);
        // se añade a la lista 
        objetosPooled.Add(objeto);

        return objeto;
    }

    public GameObject getObjeto()
    {
        //al dispara busca la primera bala que este desactivada
        GameObject objeto = objetosPooled.Find(x => x.activeInHierarchy == false);
        // si te quedas sin balas en la lista se crea una de "emergencia"
        if (objeto == null)
            objeto = crearNuevoObjeto();
        //se activa la bala para que el arma pueda disparar
        objeto.SetActive(true);
        return objeto;

    }
}
