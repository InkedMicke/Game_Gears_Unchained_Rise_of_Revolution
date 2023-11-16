using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    protected NavMeshAgent _agent;

    protected Transform _playerTr;

    protected bool _isShooting;

    protected float distanceToSlow = 6f;
    protected float distanceToAttack = 5f;
    protected float originalSpeed;

    void Start()
    {
        _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }
}

    
