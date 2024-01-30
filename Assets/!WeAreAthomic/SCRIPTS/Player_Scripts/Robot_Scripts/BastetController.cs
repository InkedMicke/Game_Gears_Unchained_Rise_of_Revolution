using DG.Tweening;
using System;
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
        public GameObject playerRightArm;
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

        public void StartMoveToBastetPos()
        {
            _moveToBastetPos = true;
        }

        public void StopMoveToBastetPos()
        {
            _moveToBastetPos = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onFinishedAction">funcion que se va ejecutar si "callOnFinish" es verdadera</param>
        /// <param name="posToGo">Target del movimiento</param>
        /// <param name="duration">Duracion del movimiento</param>
        /// <param name="ease"></param>
        public void GoToDesiredPos(Action onFinishedAction, Vector3 posToGo, float duration, Ease ease)
        {
            transform.DOMove(posToGo, duration).SetEase(ease).OnComplete(() => onFinishedAction());
        }

        private IEnumerator WaitForShoot()
        {
            yield return new WaitForSeconds(0.1f);

            //StartCoroutine(ShootEnemy());
        }

        public void Shoot(GameObject bulletPrefab, float bulletForce, Vector3 posToLook, Vector3 bulletSize, bool ifDestroyInTime,float timeToDestroy)
        {
            var randomMuzzle = Random.Range(0, muzzles.Count);
            var bullet = Instantiate(bulletPrefab, muzzles[randomMuzzle].position, Quaternion.identity);
            bullet.transform.LookAt(posToLook);
            bullet.GetComponent<Bullet>().bulletForce = bulletForce;
            if (ifDestroyInTime)
            {
                Destroy(bullet, timeToDestroy);
            }
        }

        public void PosRightHand()
        {
            transform.position = playerRightArm.transform.position;
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
