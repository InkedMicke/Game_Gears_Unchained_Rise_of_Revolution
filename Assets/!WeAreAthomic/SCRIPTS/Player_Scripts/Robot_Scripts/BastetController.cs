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

        private Tween _goToPosTween;

        [SerializeField] private LayerMask enemyHurtBoxLayer;

        [SerializeField] private GameObject playerObj;
        [SerializeField] private GameObject scannerObj;
        [SerializeField] private GameObject disc;
        [SerializeField] private GameObject bullet;
        [SerializeField] private GameObject cameraObj;

        Transform m_parent;

        Quaternion m_startRot;

        Vector3 m_startPos;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed = 0.1f;
        [SerializeField] private float attackMoveSpeed = 5f;
        [SerializeField] private float bastetMoveSpeed = 5f;

        [SerializeField] private int shootCount = 6;

        public List<Transform> muzzles;

        private void Awake()
        {
            _mainCPistol = playerObj.GetComponent<MainCPistol>();

            m_startPos = transform.position;
            m_startRot = transform.rotation;
            m_parent = transform.parent;
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
            disc.SetActive(true);
            transform.SetParent(null);
            _goToPosTween = transform.DOMove(posToGo, duration).SetEase(ease).OnComplete(() => onFinishedAction());
        }

        public void GoToRightHandPosUntilReachedPos(float speed, float distanceMagnitude)
        {
            StartCoroutine(RightHandUntilReachedPos(speed, distanceMagnitude));
        }

        private IEnumerator RightHandUntilReachedPos(float speed, float distanceMagnitude)
        {
            while(Vector3.SqrMagnitude(transform.position - m_parent.position) > distanceMagnitude)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_parent.position, speed);
                yield return new WaitForEndOfFrame();
            }
            disc.SetActive(false);
            transform.SetParent(m_parent);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f,0f,0f));
            
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

        public void HideScanner()
        {
            scannerObj.SetActive(false);
        }

        public void ShowScanner()
        {
            scannerObj.SetActive(true);
        }

        public void ToggleDisc()
        {
            disc.SetActive(!disc.activeSelf);
        }
    }
}
