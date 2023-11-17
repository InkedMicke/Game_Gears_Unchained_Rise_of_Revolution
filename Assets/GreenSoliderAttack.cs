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
    private Coroutine _decalCoroutine;

    [SerializeField] private AnimationCurve moveToDecalCurve;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject decalGroupPrefab;
    private GameObject _currentDecal;

    [SerializeField] private Transform muzzle1;
    [SerializeField] private Transform muzzle2;

    private Vector3 _staticPlayerPos;

    [System.NonSerialized] public bool IsAttacking;
    [System.NonSerialized] public bool IsShooting;

    [SerializeField] private float checkRadius = 5f;
    public float totalColdown;
    private float _startDecalSize;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _greenSoliderHurtBox = GetComponentInChildren<GreenSoliderHurtBox>();
        _agent = GetComponent<NavMeshAgent>();
        _startDecalSize = decalGroupPrefab.transform.localScale.z;
    }

    public void StartDecal()
    {
        _agent.enabled = false;
        IsAttacking = true;
        var decalScale = decalGroupPrefab.transform.localScale;
        decalScale = new Vector3(decalScale.x, decalScale.z, _startDecalSize);
        decalGroupPrefab.transform.localScale = decalScale;
        decalGroupPrefab.SetActive(true);
        _decalCoroutine= StartCoroutine(DecalSize(.04f));
    }

    private IEnumerator ShootCoroutine()
    {
        IsShooting = true;

        while (true)
        {
            CheckIfPlayer();
            yield return new WaitForSeconds(.2f);
            SpawnBullet(muzzle2);

            yield return new WaitForSeconds(.2f);
            SpawnBullet(muzzle1);

            yield return new WaitForSeconds(.4f);
            SpawnBullet(muzzle1);
            SpawnBullet(muzzle2);

            yield return new WaitForSeconds(.5f);

            if (!IsShooting)
            {
                break;
            }
        }
    }

    private void SpawnBullet(Transform muzzle)
    {
        Instantiate(bullet, muzzle.position, transform.rotation);
    }

    private IEnumerator DecalSize(float speed)
    {
        yield return new WaitForSeconds(.2f);
        if (_currentDecal == null)
        {
            _currentDecal = Instantiate(decalGroupPrefab, transform.position, Quaternion.identity);
        }
        var desiredPos = new Vector3(_playerTr.position.x, transform.position.y, _playerTr.position.z);
        transform.LookAt(desiredPos);
        _currentDecal.transform.LookAt(desiredPos);
        var decalGroup = _currentDecal.transform.GetChild(0).transform;


        while (decalGroup.transform.localScale.z < 3)
        {
            var decalScale = decalGroup.transform.localScale;
            decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z + speed);
            decalGroup.transform.localScale = decalScale;
            CheckIfPlayer();
            yield return new WaitForSeconds(.01f);
        }
        _shootCoroutine = StartCoroutine(ShootCoroutine());

    }

    private void CheckIfPlayer()
    {
        var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();

        if (!decalHurtBox.IsPlayerInside && decalHurtBox.HasPlayerLeft)
        {
            if (IsShooting)
            {
                StopCoroutine(_shootCoroutine);
            }

            StopCoroutine(_decalCoroutine);

            Destroy(_currentDecal);
            IsShooting = false;
            IsAttacking = false;
            totalColdown = Time.time + 4f;
            _agent.enabled = true;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        Debug.DrawRay(muzzle1.transform.position, muzzle1.forward * 3);
    }
}
