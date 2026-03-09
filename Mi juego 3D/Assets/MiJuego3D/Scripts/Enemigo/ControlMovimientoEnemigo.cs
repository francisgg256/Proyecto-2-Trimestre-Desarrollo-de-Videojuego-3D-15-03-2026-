using UnityEngine;
using UnityEngine.AI;
public class ControlMovimientoEnemigo : MonoBehaviour
{
    [SerializeField] Transform objetivo;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(objetivo.position);
    }
}
