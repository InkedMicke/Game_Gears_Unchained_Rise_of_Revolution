using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GreenSoliderMovement : EnemyAI
{
    private CharacterController _cc;
    private GreenSoliderAttack _soldierAttack;
    private float _timeToCancelAttack;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        _soldierAttack = GetComponent<GreenSoliderAttack>();

        originalSpeed = _agent.speed;
    }

    void Update()
    {
        if (!_soldierAttack.IsAttacking && !_soldierAttack.IsShooting)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_playerTr.position);
            Debug.Log("IsAttacking: " + _soldierAttack.IsAttacking);
            Debug.Log("IsShooting: " + _soldierAttack.IsShooting);
        }

        var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

        if (distanceToPlayer < 5f && !_soldierAttack.IsAttacking && Time.time > _soldierAttack.totalColdown)
        {
            _agent.isStopped = true;
            _soldierAttack.StartDecal();
        }
        
    }
}
