using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace Player.Bastet
{
    public class BastetController : MonoBehaviour
    {

        private MainCPistol _mainCPistol;
        private MainCRail _mainCRail;

        private Tween _goToPosTween;

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

        public List<Transform> muzzles;

        private void Awake()
        {
            _mainCPistol = playerObj.GetComponent<MainCPistol>();
            _mainCRail = playerObj.GetComponent<MainCRail>();
        }

        private void Update()
        {
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onFinishedAction">funcion que se va ejecutar si "callOnFinish" es verdadera</param>
        /// <param name="posToGo">Target del movimiento</param>
        /// <param name="duration">Duracion del movimiento</param>
        /// <param name="ease"></param>
        public void GoToDesiredPos(Action onFinishedAction, Vector3 posToGo, float duration, Ease ease)
        {
            _goToPosTween = transform.DOMove(posToGo, duration).SetEase(ease).OnComplete(() => onFinishedAction());
        }

        public void GoToRightHandPosUntilReachedPos(bool callFunctionOnComplete, Action onFinishedAction, float speed, float distanceMagnitude)
        {
            StartCoroutine(RightHandUntilReachedPos(callFunctionOnComplete, onFinishedAction, speed, distanceMagnitude));
        }

        private IEnumerator RightHandUntilReachedPos(bool callFunctionOnComplete, Action onFinishedAction,float speed, float distanceMagnitude)
        {
            while(Vector3.Distance(transform.position, playerRightArm.transform.position) > distanceMagnitude)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerRightArm.transform.position, speed);

                yield return new WaitForEndOfFrame();
            }
            if (callFunctionOnComplete)
            {
                onFinishedAction();
            }
        }

        public void GoToDesiredPosMoveTowardsWithoutUpdate(Vector3 posToGo, float speed)
        {
            transform.position = Vector3.MoveTowards(transform.position, posToGo, speed);
        }        

        public void KillGoToDesiredPos()
        {
            _goToPosTween.Kill();
        }

        public void Shoot(GameObject bulletPrefab, float bulletForce, Vector3 posToLook, Vector3 bulletSize, bool ifDestroyInTime, float timeToDestroy, PlayerDamageData damageData)
        {
            var randomMuzzle = Random.Range(0, muzzles.Count);
            var bullet = Instantiate(bulletPrefab, muzzles[randomMuzzle].position, Quaternion.identity);
            bullet.GetComponent<BastetBullet>().damageData = damageData;
            bullet.transform.LookAt(posToLook);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletForce;
            bullet.transform.localScale = bulletSize;
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
