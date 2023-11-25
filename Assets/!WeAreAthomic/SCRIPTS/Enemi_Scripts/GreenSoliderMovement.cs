using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GreenSoliderMovement : EnemyAI
{
    private CharacterController _cc;
    private GreenSoliderAttack _soldierAttack;

    private GameManagerSingleton.TypeOfEnemy _typeOfEnemy;

    private float _timeToCancelAttack;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        _soldierAttack = GetComponent<GreenSoliderAttack>();

        originalSpeed = _agent.speed;

        _typeOfEnemy = GameManagerSingleton.TypeOfEnemy.GreenSoldier;
    }

    void Update()
    {
        if (!_soldierAttack.IsAttacking && !_soldierAttack.IsShooting)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_playerTr.position);
        }

        var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

        if (distanceToPlayer < 5f && !_soldierAttack.IsAttacking && Time.time > _soldierAttack.totalColdown)
        {
            _agent.isStopped = true;
            _soldierAttack.StartDecal();
        }
        
    }
}
