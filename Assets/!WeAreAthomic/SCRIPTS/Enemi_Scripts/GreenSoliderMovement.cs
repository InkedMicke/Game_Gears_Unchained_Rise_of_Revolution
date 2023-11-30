using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GreenSoliderMovement : EnemyAI
{
    private GreenSoliderAttack _soldierAttack;
    private SoldierAnimator _soldierAnim;

    private GameManagerSingleton.TypeOfEnemy _typeOfEnemy;

    private float _timeToCancelAttack;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _soldierAttack = GetComponent<GreenSoliderAttack>();
        _soldierAnim = GetComponent<SoldierAnimator>();

        originalSpeed = _agent.speed;

        _typeOfEnemy = GameManagerSingleton.TypeOfEnemy.GreenSoldier;
    }

    void Update()
    {
        if (!_soldierAttack.IsAttacking && !_soldierAttack.IsShooting)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_playerTr.position);
            _soldierAnim.SetWalking(true);
        }

        var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

        if (distanceToPlayer < 5f && !_soldierAttack.IsAttacking && Time.time > _soldierAttack.totalColdown)
        {
            _agent.isStopped = true;
            _soldierAnim.SetWalking(false);
            _soldierAttack.StartDecal();
        }
        
    }
}
