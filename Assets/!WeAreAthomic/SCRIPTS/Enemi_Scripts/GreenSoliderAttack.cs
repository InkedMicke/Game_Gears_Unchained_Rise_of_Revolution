using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GreenSoliderAttack : MonoBehaviour {
    private NavMeshAgent _agent;
    private SoldierAnimator _soldierAnim;

    private Coroutine _shootCoroutine;
    private Coroutine _decalCoroutine;

    [SerializeField] private AnimationCurve moveToDecalCurve;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject decalGroupPrefab;
    private GameObject _currentDecal;

    [SerializeField] private Transform muzzle1;
    private Transform endDecalTr;
    private Transform _playerTr => GameObject.FindGameObjectWithTag("Player").transform;

    [System.NonSerialized] public bool IsAttacking;
    [System.NonSerialized] public bool IsShooting;
    private bool _hasEndedShootAnim;

    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private float speedShooting = 1.5f;
    [SerializeField] private float spreadAngle = 2f;
    [SerializeField] private float shootAngle = .6f;
    public float totalColdown;
    public float damage;
    private float timeToStopShooting;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _soldierAnim = GetComponent<SoldierAnimator>();
    }

    public void StartDecal() {
        _agent.enabled = false;
        IsAttacking = true;
        _decalCoroutine = StartCoroutine(DecalSize(.2f));
    }

    private void Update() {
        if (IsShooting && _currentDecal != null) {
            var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();

            if (!decalHurtBox.IsPlayerInside && !decalHurtBox.HasPlayerLeft) {
                timeToStopShooting += Time.deltaTime;
                if (timeToStopShooting > 2.5f) {
                    if (IsShooting) {
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

    private IEnumerator ShootCoroutine() {
        IsShooting = true;
        while (!CheckIfPlayer()) {
            _hasEndedShootAnim = false;
            _soldierAnim.ShootTrigger();
            _soldierAnim.SetAnimatorSpeed(speedShooting);

            while (!_hasEndedShootAnim) {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void SpawnBullet() {
        for (int i = 0; i < 5; i++) // Cambia este valor según la cantidad de proyectiles que quieres disparar
        {
            if (endDecalTr != null) 
            {
                var desiredPos = new Vector3(
                    endDecalTr.position.x + Random.Range(-spreadAngle, spreadAngle),
                    (muzzle1.position.y - shootAngle) + Random.Range(-spreadAngle, spreadAngle),
                    endDecalTr.position.z + Random.Range(-spreadAngle, spreadAngle)
                    );
                var bulletObj = Instantiate(bullet, muzzle1.position, transform.rotation);
                bulletObj.transform.LookAt(desiredPos);
                var dir = desiredPos - bulletObj.transform.position;

                bulletObj.GetComponent<Rigidbody>().AddForce(dir.normalized * bulletObj.GetComponent<GBullet>().bulletForce, ForceMode.Impulse);

                Destroy(bulletObj, 5f);
            }
        }

    }

    private IEnumerator DecalSize(float speed) {
        yield return new WaitForSeconds(.2f);
        if (_currentDecal == null) {
            _currentDecal = Instantiate(decalGroupPrefab, transform.position, Quaternion.identity);
        }
        var desiredPos = new Vector3(_playerTr.position.x, transform.position.y, _playerTr.position.z);
        transform.LookAt(desiredPos);
        _currentDecal.transform.LookAt(desiredPos);
        var decalGroup = _currentDecal.transform.GetChild(0).transform;
        var decal = decalGroup.GetChild(0).transform;
        endDecalTr = decal.GetChild(0).transform;


        while (decalGroup.transform.localScale.z < 6) {
            var decalScale = decalGroup.transform.localScale;
            decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z + speed);
            decalGroup.transform.localScale = decalScale;
            yield return new WaitForSeconds(.01f);
        }
        _shootCoroutine = StartCoroutine(ShootCoroutine());

    }

    private bool CheckIfPlayer() {
        if (_currentDecal != null) {
            var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();


            if (!decalHurtBox.IsPlayerInside && decalHurtBox.HasPlayerLeft) {


                StopCoroutine(_decalCoroutine);

                Destroy(_currentDecal);
                IsShooting = false;
                IsAttacking = false;
                totalColdown = Time.time + 4f;
                _agent.enabled = true;

                if (IsShooting) {
                    StopCoroutine(_shootCoroutine);
                    _soldierAnim.SetAnimatorSpeed(1f);
                    return true;
                }
            }
        }
        return false;
    }

    public void EndShootAnim() {
        _hasEndedShootAnim = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        Debug.DrawRay(muzzle1.transform.position, muzzle1.forward * 3);
    }
}
