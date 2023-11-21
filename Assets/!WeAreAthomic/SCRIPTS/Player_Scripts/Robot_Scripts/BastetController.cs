using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts
{
    public class BastetController : MonoBehaviour
    {
        private CharacterController _cc;
        private MainCBastetAttack _mainCBastet;
        private MainCPistol _mainCPistol;

        private Coroutine _shootCoroutine;

        [SerializeField] private LayerMask enemyHurtBoxLayer;

        [SerializeField] private GameObject playerObj;
        [SerializeField] private GameObject playerRightArm;
        [SerializeField] private GameObject scannerObj;
        [SerializeField] private GameObject bullet;
        [SerializeField] private GameObject cameraObj;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed = 0.1f;
        [SerializeField] private float attackMoveSpeed = 5f;
        [SerializeField] private float bastetMoveSpeed = 5f;

        [SerializeField] private int shootCount = 6;

        private bool positive;
        private bool negative;
        private bool _isShooting;
        private bool _moveToBastetPos;
        private bool _isAbilityAttacking;

        public List<Transform> muzzles;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCBastet = playerObj.GetComponent<MainCBastetAttack>();
            _mainCPistol = playerObj.GetComponent<MainCPistol>();
        }

        public void InvokeMoveToPlayer()
        {
            StartCoroutine(nameof(MoveToPlayer));
        }

        public void InvokeRotation()
        {
            ShowScanner();
        }

        private void Update()
        {
            if (_moveToBastetPos)
            {
                var leftPos = playerObj.transform.position + Vector3.left;
                var correctPos = new Vector3(leftPos.x, leftPos.y + 1.5f, leftPos.z);
                var difference = correctPos - transform.position;
                var moveDir = 5f * Time.deltaTime * difference.normalized;

                if (Vector3.Distance(transform.position, correctPos) > 0.1f)
                {
                    _cc.Move(moveDir);
                }

                if (_isAbilityAttacking)
                {
                    var ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
                    if(Physics.Raycast(ray, out var hit,20f, enemyHurtBoxLayer))
                    {
                        transform.LookAt(hit.collider.gameObject.transform.position);   
                    }
                    else
                    {
                        transform.LookAt(ray.GetPoint(20f));
                    }
                }
            }
            else
            {
                var direction = playerRightArm.transform.position - transform.position;
                _cc.Move(direction.normalized * moveSpeed * Time.deltaTime);
                transform.LookAt(playerRightArm.transform);

                if (Vector3.Distance(playerRightArm.transform.position, transform.position) < 0.3f)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void SetMoveToBastetPos(bool condition)
        {
            _moveToBastetPos = condition;
        }

        public void SetAbilityBastetAttack(bool condition)
        {
            _isAbilityAttacking = condition;
        }

        public void StartMovetoPlayer()
        {
            StartCoroutine(MoveToPlayer());
        }

        private IEnumerator MoveToPlayer()
        {
            var canEnableLayer = true;
            HideScanner();
            while (canEnableLayer)
            {
                var direction = playerRightArm.transform.position - transform.position;
                _cc.Move(direction.normalized * moveSpeed * Time.deltaTime);
                transform.LookAt(playerRightArm.transform);

                if (Vector3.Distance(playerRightArm.transform.position, transform.position) < 0.3f)
                {
                    canEnableLayer = false;
                    gameObject.SetActive(false);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator NegativeRotationX()
        {
            var enable = true;

            while (enable)
            {
                var eulerAngles = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(eulerAngles.x -= rotateSpeed, eulerAngles.y, eulerAngles.z);
                negative = true;
                if (transform.localRotation.x >= 0.2f)
                {
                    enable = false; ;
                    StartCoroutine(nameof(WaitRotation));
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator PositiveRotationX()
        {
            var enable = true;

            while (enable)
            {
                var eulerAngles = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(eulerAngles.x += rotateSpeed, eulerAngles.y, eulerAngles.z);
                positive = true;
                if (transform.localRotation.x >= 0.2f)
                {
                    enable = false;
                    StartCoroutine(nameof(WaitRotation));
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator WaitRotation()
        {
            var enable = true;
            var time = 0.1f;

            while (enable)
            {
                time -= Time.deltaTime;

                if (time <= 0f)
                {
                    if (positive)
                    {
                        enable = false;
                        positive = false;
                        StartCoroutine(nameof(NegativeRotationX));
                    }

                    else if (negative)
                    {
                        enable = false;
                        negative = false;
                        StartCoroutine(nameof(PositiveRotationX));
                    }
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public void StartMoveToBastetPos()
        {
            _moveToBastetPos = true;
            StartCoroutine(ShootEnemy());
        }

        private IEnumerator ShootEnemy()
        {
            var currentShoots = new int();
            while (true)
            {
                var randomMuzzle = Random.Range(0, muzzles.Count);
                var bulletObj = Instantiate(bullet, muzzles[randomMuzzle].position, Quaternion.identity);
                var enemyPos = _mainCBastet.closestEnemyToShoot.transform.position;
                var lookAtFixed = new Vector3(enemyPos.x, enemyPos.y + .5f, enemyPos.z);
                bulletObj.transform.LookAt(lookAtFixed);
                transform.LookAt(lookAtFixed);
                currentShoots++;
                if (currentShoots > 25)
                {
                    _moveToBastetPos = false;
                    _mainCBastet.DisableBastetAttacking();
                    StartCoroutine(MoveToPlayer());
                    break;
                }
                yield return new WaitForSeconds(Random.Range(.1f, .3f));
            }
        }

        public void Shoot(Vector3 enemy, GameObject bullet, bool condition, float sizeBullet, float bulletSpeed)
        {
            var randomMuzzle = Random.Range(0, muzzles.Count);
            var bulletObj = Instantiate(bullet, muzzles[randomMuzzle].position, Quaternion.identity);
            bulletObj.GetComponent<GBullet>().bulletForce = bulletSpeed;
            bulletObj.transform.localScale = new Vector3(sizeBullet, sizeBullet, sizeBullet);
            var enemyPos = enemy;
            if (condition)
            {
                var lookAtFixed = new Vector3(enemyPos.x, enemyPos.y + .5f, enemyPos.z);
                bulletObj.transform.LookAt(lookAtFixed);
            }
            else
            {
                bulletObj.transform.LookAt(enemyPos);
            }

            _mainCPistol.DisableShooting();
        }

        private IEnumerator WaitForShoot()
        {
            yield return new WaitForSeconds(0.1f);

            StartCoroutine(ShootEnemy());
        }

        public void PosRightHand()
        {
            _cc.enabled = false;
            transform.position = playerRightArm.transform.position;
            _cc.enabled = true;
        }

        public void HideScanner()
        {
            scannerObj.SetActive(false);
        }

        private void ShowScanner()
        {
            scannerObj.SetActive(true);
        }
    }
}
