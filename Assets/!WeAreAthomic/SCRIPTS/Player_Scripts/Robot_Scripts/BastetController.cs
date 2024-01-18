using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts
{
    public class BastetController : MonoBehaviour
    {
        private CharacterController _cc;
        private MainCBastetRapidFire _mainCBastet;
        private MainCPistol _mainCPistol;
        private MainCRail _mainCRail;

        private Coroutine _shootCoroutine;
        private Coroutine _bastetPosCoroutine;

        private Scene _currentScene;

        [SerializeField] private LayerMask enemyHurtBoxLayer;

        [SerializeField] private GameObject playerObj;
        [SerializeField] private GameObject playerRightArm;
        [SerializeField] private GameObject scannerObj;
        [SerializeField] private GameObject bullet;
        [SerializeField] private GameObject cameraObj;
        private GameObject _savedClosestEnemy;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed = 0.1f;
        [SerializeField] private float attackMoveSpeed = 5f;
        [SerializeField] private float bastetMoveSpeed = 5f;

        [SerializeField] private int shootCount = 6;

        private bool positive;
        private bool negative;
        private bool _isShooting;
        private bool _moveToBastetPos;

        public List<Transform> muzzles;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCBastet = playerObj.GetComponent<MainCBastetRapidFire>();
            _mainCPistol = playerObj.GetComponent<MainCPistol>();
            _mainCRail = playerObj.GetComponent<MainCRail>();
        }

        private void Update()
        {
            if(_moveToBastetPos)
            {
                var leftPos = playerObj.transform.position + Vector3.left;
                var correctPos = new Vector3(leftPos.x, leftPos.y + 1.5f, leftPos.z);
                var difference = correctPos - transform.position;
                var moveDir = 8f * Time.deltaTime * difference.normalized;

                if (Vector3.Distance(transform.position, correctPos) > 0.1f)
                {
                    //_cc.Move(moveDir);
                    _currentScene = SceneManager.GetActiveScene();
                    transform.position = Vector3.MoveTowards(transform.position, correctPos, (_mainCRail.IsSliding ? bastetMoveSpeed * 3 : bastetMoveSpeed) * Time.deltaTime);
                }

                if (_mainCPistol.IsAiming)
                {
                    var ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
                    if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                    {
                        transform.LookAt(hit.point);
                    }
                    else
                    {
                        transform.LookAt(ray.GetPoint(75f));
                    }
                }
            }
        }

        public void InvokeMoveToPlayer()
        {
            StartCoroutine(nameof(MoveToPlayer));
        }

        public void StartMoveToBastetPos()
        {
            _moveToBastetPos = true;
        }

        public void StopMoveToBastetPos()
        {
            _moveToBastetPos = false;
        }

        public void StartMovetoPlayer()
        {
            StartCoroutine(MoveToPlayer());
        }

        private IEnumerator MoveToPlayer()
        {
            _cc.enabled = true;
            HideScanner();
            while (true)
            {
                var direction = playerRightArm.transform.position - transform.position;
                _cc.Move(direction.normalized * moveSpeed * Time.deltaTime);
                transform.LookAt(playerRightArm.transform);

                if (Vector3.Distance(playerRightArm.transform.position, transform.position) < 0.3f)
                {
                    gameObject.SetActive(false);
                    break;
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

        public void StartShootEnemy(int maxShoots)
        {
            StartCoroutine(ShootEnemy(maxShoots));
        }

        private IEnumerator ShootEnemy(int maxShoots)
        {
            var currentShoots = new int();
            while (true)
            {
                var randomMuzzle = Random.Range(0, muzzles.Count);
                var bulletObj = Instantiate(bullet, muzzles[randomMuzzle].position, Quaternion.identity);
                _savedClosestEnemy = _mainCBastet.closestEnemyToShoot;
                var enemyPos = _mainCBastet.closestEnemyToShoot != null ? _mainCBastet.closestEnemyToShoot.transform.position : _savedClosestEnemy.transform.position;
                var lookAtFixed = new Vector3(enemyPos.x, enemyPos.y + .1f, enemyPos.z);
                bulletObj.transform.LookAt(lookAtFixed);
                transform.LookAt(lookAtFixed);
                var difference = lookAtFixed - transform.position;
                Debug.DrawRay(transform.position, difference * 10, Color.red, 2f);
                bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 50f, ForceMode.Impulse);
                currentShoots++;
                if (currentShoots > maxShoots)
                {
                    _mainCBastet.DisableBastetAttacking();
                    StartCoroutine(MoveToPlayer());
                    break;
                }
                yield return new WaitForSeconds(Random.Range(.1f, .3f));
            }
        }

        private IEnumerator WaitForShoot()
        {
            yield return new WaitForSeconds(0.1f);

            //StartCoroutine(ShootEnemy());
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

        public void ShowScanner()
        {
            scannerObj.SetActive(true);
        }
    }
}
