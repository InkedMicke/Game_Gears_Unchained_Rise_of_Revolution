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
        StartCoroutine(DecalSize());
    }

    private IEnumerator ShootCoroutine(Transform shootPos)
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            SpawnBullet(muzzle2, shootPos);

            yield return new WaitForSeconds(.2f);
            SpawnBullet(muzzle1, shootPos);

            yield return new WaitForSeconds(.4f);
            SpawnBullet(muzzle1, shootPos);
            SpawnBullet(muzzle2, shootPos);

            yield return new WaitForSeconds(.5f);
        }
    }

    private void SpawnBullet(Transform muzzle, Transform shootPos)
    {
        var bulletObj = Instantiate(bullet, muzzle.position, transform.rotation);
    }

    private IEnumerator DecalSize()
    {
        var decal = Instantiate(decalGroup, transform.position, Quaternion.identity);
        var desiredPos = new Vector3(_playerTr.position.x, transform.position.y, _playerTr.position.z);
        decal.transform.LookAt(desiredPos);

        
        while (decal.transform.localScale.z < 3)
        {
            var decalScale = decal.transform.localScale;
            decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z + .08f);
            decal.transform.localScale = decalScale;
            yield return new WaitForSeconds(.01f);
        }

        var lasChild = decal.transform.GetChild(decal.transform.childCount - 1);

        StartCoroutine(ShootCoroutine(lasChild));
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
