using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GreenSoliderAttack : MonoBehaviour
{
    private NavMeshAgent _agent;
    private SoldierAnimator _soldierAnim;

    private Coroutine _shootCoroutine;
    private Coroutine _decalCoroutine;

    [SerializeField] private AnimationCurve moveToDecalCurve;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject decalGroupPrefab;
    private GameObject _currentDecal;

    [SerializeField] private Transform muzzle1;
    private Transform _playerTr => GameObject.FindGameObjectWithTag("Player").transform;

    [System.NonSerialized] public bool IsAttacking;
    [System.NonSerialized] public bool IsShooting;
    private bool _hasEndedShootAnim;

    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private float speedShooting = 1.5f;
    public float totalColdown;
    public float damage;
    private float timeToStopShooting;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _soldierAnim = GetComponent<SoldierAnimator>();
    }

    public void StartDecal()
    {
        _agent.enabled = false;
        IsAttacking = true;
        _decalCoroutine = StartCoroutine(DecalSize(.2f));
    }

    private void Update()
    {
        if (IsShooting && _currentDecal != null)
        {
            var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();

            if (!decalHurtBox.IsPlayerInside && !decalHurtBox.HasPlayerLeft)
            {
                timeToStopShooting += Time.deltaTime;
                if (timeToStopShooting > 2.5f)
                {
                    if (IsShooting)
                    {
                        StopCoroutine(_shootCoroutine);
                    }
                    timeToStopShooting = 0f;
                    StopCoroutine(_decalCoroutine);
                    Destroy(_currentDecal);
                    IsShooting = false;
                    IsAttacking = false;
                    totalColdown = Time.time + 4f;
                    _agent.enabled = true;
                }
            }
        }
    }

    private IEnumerator ShootCoroutine()
    {
        IsShooting = true;
        while (!CheckIfPlayer())
        {
            _hasEndedShootAnim = false;
            _soldierAnim.ShootTrigger();
            _soldierAnim.SetAnimatorSpeed(speedShooting);

            while (!_hasEndedShootAnim)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("endedAnim");
        }
    }

    public void SpawnBullet()
    {
        for (int i = 0; i < 5; i++) // Cambia este valor según la cantidad de proyectiles que quieres disparar
        {
            var bulletObj = Instantiate(bullet, muzzle1.position, transform.rotation);
            var rb = bulletObj.GetComponent<Rigidbody2D>();

            // Aplicar una rotación aleatoria al proyectil dentro del ángulo de dispersión
            float spread = Random.Range(-5, 5);
            var spreadRotation = Quaternion.Euler(0f, 0f, spread);
            var spreadDirection = spreadRotation * muzzle1.right;

            // Aplicar fuerza al proyectil con la dirección aleatoria
            rb.AddForce(spreadDirection * bulletObj.GetComponent<GBullet>().bulletForce, ForceMode2D.Impulse); // Ajusta la fuerza según tu escala y necesidades
        }

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


        while (decalGroup.transform.localScale.z < 6)
        {
            var decalScale = decalGroup.transform.localScale;
            decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z + speed);
            decalGroup.transform.localScale = decalScale;
            yield return new WaitForSeconds(.01f);
        }
        _shootCoroutine = StartCoroutine(ShootCoroutine());

    }

    private bool CheckIfPlayer()
    {
        var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();

        if (!decalHurtBox.IsPlayerInside && decalHurtBox.HasPlayerLeft)
        {
            

            StopCoroutine(_decalCoroutine);

            Destroy(_currentDecal);
            IsShooting = false;
            IsAttacking = false;
            totalColdown = Time.time + 4f;
            _agent.enabled = true;

            if (IsShooting)
            {
                StopCoroutine(_shootCoroutine);
                _soldierAnim.SetAnimatorSpeed(1f);
                return true;
            }
        }
        return false;
    }

    public void EndShootAnim()
    {
        _hasEndedShootAnim = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        Debug.DrawRay(muzzle1.transform.position, muzzle1.forward * 3);
    }
}
