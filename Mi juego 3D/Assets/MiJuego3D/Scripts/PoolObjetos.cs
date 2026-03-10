using System.Collections.Generic; 
using UnityEngine; 

public class PoolObjetos : MonoBehaviour 
{
    public GameObject objetoPrefab;
    public int numObjetosOnStart;
    private List<GameObject> objetosPooled = new List<GameObject>();

    private void Start() 
    {
        // un bucle que se repite tantas veces como ponga en numObjetosOnStart
        for (int x = 0; x < numObjetosOnStart; x++)
        {
            crearNuevoObjeto(); //crear una bala invisible y la mete en la lista
        }
    }

    private GameObject crearNuevoObjeto() 
    {
        // clona el Prefab y lo pone en el mapa
        GameObject objeto = Instantiate(objetoPrefab);

        // desactivamos la bala para que no se vea ni consuma recursos
        objeto.SetActive(false);

        // se añade la bala apagada a la Lista 
        objetosPooled.Add(objeto);

        // devuelve la bala
        return objeto;
    }

    public GameObject getObjeto() 
    {
        // busca en la Lista de balas la primera bala que este desactivada usando una expresión lambda
        GameObject objeto = objetosPooled.Find(x => x.activeInHierarchy == false);

        // si es null todas las balas han sido ya usadas
        if (objeto == null)
        {
            // fabrica una bala nueva
            objeto = crearNuevoObjeto();
        }

        // cuando la bala dse puede usar se activa para poder disparar
        objeto.SetActive(true);

        return objeto;
    }
}
