using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GreenSoliderController : EnemyAI
{

    private Coroutine _shootCoroutine;

    [SerializeField] private GameObject bullet;

    [SerializeField] private Transform muzzle1;
    [SerializeField] private Transform muzzle2;

    private float timeToCancelAttack;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        originalSpeed = _agent.speed;
    }

    void Update()
    {
        _agent.SetDestination(_playerTr.position);

        var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

        if(distanceToPlayer < distanceToSlow && distanceToPlayer > distanceToAttack)
        {
            _agent.speed = originalSpeed * 0.7f;
            var number += Time.deltaTime;
            StopCoroutine(ShootCoroutine());
            StopCoroutine(_shootCoroutine);
        }

        if (distanceToPlayer < distanceToAttack)
        {
            _agent.speed = originalSpeed * 0.2f;

            if(!_isShooting)
            {
                _shootCoroutine = StartCoroutine(ShootCoroutine());
                _isShooting = true;
            }
        }
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(.3f);
            SpawnBullet(muzzle2);

            yield return new WaitForSeconds(.3f);
            SpawnBullet(muzzle1);

            yield return new WaitForSeconds(.5f);
            SpawnBullet(muzzle1);
            SpawnBullet(muzzle2);

            yield return new WaitForSeconds(2f);
        }
    }

    private void SpawnBullet(Transform muzzle)
    {
        var bulletObj = Instantiate(bullet, muzzle.position, Quaternion.identity);
        var desiredPos = new Vector3(_playerTr.position.x, _playerTr.position.y + 1f, _playerTr.position.z);

        bulletObj.transform.LookAt(desiredPos);
    }

}
