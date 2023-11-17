using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GreenSoliderAttack : EnemyAI
{
    private CharacterController _cc;
    private GreenSoliderHurtBox _greenSoliderHurtBox;

    private Coroutine _shootCoroutine;

    [SerializeField] private AnimationCurve moveToDecalCurve;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject decalGroup;
    private GameObject _currentDecal;

    [SerializeField] private Transform muzzle1;
    [SerializeField] private Transform muzzle2;

    private Vector3 _staticPlayerPos;

    [System.NonSerialized] public bool IsAttacking;
    [System.NonSerialized] public bool IsShooting;

    [SerializeField] private float checkRadius = 5f;
    private float _startDecalSize;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _greenSoliderHurtBox = GetComponentInChildren<GreenSoliderHurtBox>();
        _agent = GetComponent<NavMeshAgent>();
        _startDecalSize = decalGroup.transform.localScale.z;
    }

    public void StartDecal()
    {
        IsAttacking = true;
        var decalScale = decalGroup.transform.localScale;
        decalScale = new Vector3(decalScale.x, decalScale.z, _startDecalSize);
        decalGroup.transform.localScale = decalScale;
        decalGroup.SetActive(true);
        StartCoroutine(DecalSize("+", .08f));
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            SpawnBullet(muzzle2);

            yield return new WaitForSeconds(.2f);
            SpawnBullet(muzzle1);

            yield return new WaitForSeconds(.4f);
            SpawnBullet(muzzle1);
            SpawnBullet(muzzle2);

            yield return new WaitForSeconds(.5f);

            StartCoroutine(CheckIfPlayer());
        }
    }

    private void SpawnBullet(Transform muzzle)
    {
        Instantiate(bullet, muzzle.position, transform.rotation);
    }

    private IEnumerator DecalSize(string symbol, float speed)
    {
        _currentDecal = Instantiate(decalGroup, transform.position, Quaternion.identity);
        var desiredPos = new Vector3(_playerTr.position.x, transform.position.y, _playerTr.position.z);
        _currentDecal.transform.LookAt(desiredPos);

        if (symbol == "+")
        {
            while (_currentDecal.transform.localScale.z < 3)
            {
                var decalScale = _currentDecal.transform.localScale;
                decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z + speed);
                _currentDecal.transform.localScale = decalScale;
                yield return new WaitForSeconds(.01f);
            }
        }

        if (symbol == "-")
        {
            while (_currentDecal.transform.localScale.z >= 0.001)
            {
                var decalScale = _currentDecal.transform.localScale;
                decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z - speed);
                _currentDecal.transform.localScale = decalScale;
                yield return new WaitForSeconds(.01f);
            }
        }

        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator CheckIfPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();
            if (!decalHurtBox.IsPlayerInside)
            {
                StopAllCoroutines();
                StartCoroutine(DecalSize("-", .5f));
                IsShooting = false;
                IsAttacking = false;
                break;
            }
        }
    }

    /*private IEnumerator MoveToDecal(Transform endDecal)
    {
        _agent.enabled = false;
        _cc.enabled = true;
        while (Vector3.Distance(transform.position, endDecal.position) > 0.1f)
        {
            var difference = endDecal.position - transform.position;
            var speed = new float();
            var moveDir = difference.normalized * (Time.deltaTime * 5f);
            _cc.Move(moveDir);
            yield return new WaitForSeconds(.01f);
        }
        
        StartShooting(_greenSoliderHurtBox.CheckIfPlayerIsNear(checkRadius));
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        Debug.DrawRay(muzzle1.transform.position, muzzle1.forward * 3);
    }
}
