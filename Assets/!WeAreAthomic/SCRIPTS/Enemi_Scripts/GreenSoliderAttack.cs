using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{

    public class GreenSoliderAttack : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private SoldierAnimator _soldierAnim;
        private SoldierHealthManager _healthManager;
        private GreenSoliderMovement _greenMove;

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
        private bool _waitForDisableShoot;

        [SerializeField] private float checkRadius = 5f;
        [SerializeField] private float speedShooting = 1.5f;
        [SerializeField] private float spreadAngle = 2f;
        [SerializeField] private float shootAngle = .6f;
        [SerializeField] private float maxTimeToStopShooting = 2.5f;
        private float _timeToStopShooting;
        public float totalColdown;
        public float damage;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _soldierAnim = GetComponent<SoldierAnimator>();
            _healthManager = GetComponentInChildren<SoldierHealthManager>();
            _greenMove = GetComponent<GreenSoliderMovement>();
        }

        public void StartDecal()
        {
            _agent.enabled = false;
            IsAttacking = true;
            _decalCoroutine = StartCoroutine(DecalSize(.2f));
        }

        private void Update()
        {
            CheckForDisableShoot();
            TimerToDisableShoot();
        }

        private IEnumerator ShootCoroutine()
        {
            IsShooting = true;
            while (IsShooting)
            {
                _hasEndedShootAnim = false;
                _soldierAnim.ShootTrigger();
                _soldierAnim.SetAnimatorSpeed(speedShooting);

                while (!_hasEndedShootAnim)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public void SpawnBullet()
        {
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
            var decal = decalGroup.GetChild(0).transform;
            endDecalTr = decal.GetChild(0).transform;


            while (decalGroup.transform.localScale.z < 6)
            {
                var decalScale = decalGroup.transform.localScale;
                decalScale = new Vector3(decalScale.x, decalScale.y, decalScale.z + speed);
                decalGroup.transform.localScale = decalScale;
                yield return new WaitForSeconds(.01f);
            }
            _shootCoroutine = StartCoroutine(ShootCoroutine());

        }

        private bool CheckForDisableShoot()
        {
            if (_currentDecal != null)
            {
                var decalHurtBox = _currentDecal.GetComponentInChildren<GreenDecalHurtBox>();

                if(decalHurtBox.HasPlayerLeft)
                {
                    return true;
                }
            }

            return false;
        }

        private void TimerToDisableShoot()
        {
             if(_waitForDisableShoot)
            {
                _timeToStopShooting -= Time.deltaTime;
                if(_timeToStopShooting <= 0f)
                {
                    StopCoroutine(_decalCoroutine);
                    Destroy(_currentDecal);
                    _agent.enabled = true;
                    IsAttacking = false;
                    IsShooting = false;
                    _greenMove.SetChasePlayer(true);
                }
            }
        }

        public void EndShootAnim()
        {
            _hasEndedShootAnim = true;
            if (CheckForDisableShoot())
            {
                if(!_waitForDisableShoot)
                {
                    _timeToStopShooting = maxTimeToStopShooting;
                }
                _waitForDisableShoot = true;
            }
            else
                _waitForDisableShoot = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, checkRadius);

            Debug.DrawRay(muzzle1.transform.position, muzzle1.forward * 3);
        }
    }
}
