using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAgent : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void StopTotallyAgent()
    {
        _agent.isStopped = true;
        _agent.speed = 0;
        Debug.Log("hola");
    }
}
