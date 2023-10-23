using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace _WeAreAthomic.SCRIPTS.Player.Robot
{
    public class BastetController : MonoBehaviour
    {
        CharacterController _cc;

        private GameObject _playerObj;
        [SerializeField] private GameObject playerRightArm;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed = 0.1f;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(nameof(NegativeRotationX));
        }

        public void InvokeMoveToPlayer()
        {
            StartCoroutine(nameof(MoveToPlayer));
        }

        private void Update()
        {
            Debug.Log(transform.localEulerAngles.x);
        }

        private IEnumerator MoveToPlayer()
        {
            var canEnableLayer = true;

            while (canEnableLayer)
            {
                var direction = playerRightArm.transform.position - transform.position;
                _cc.Move(direction.normalized * moveSpeed * Time.deltaTime);

                if (Vector3.Distance(playerRightArm.transform.position, transform.position) < 0.3f)
                {
                    canEnableLayer = false;
                    this.gameObject.SetActive(false);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public IEnumerator NegativeRotationX()
        {
            var enable = true;

            while (enable)
            {
                var eulerAngles = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(eulerAngles.x -= rotateSpeed, eulerAngles.y, eulerAngles.z);

                if (eulerAngles.x <= 320)
                {
                    enable = false;
                    StartCoroutine(nameof(PositiveRotationX));
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

                if (eulerAngles.x >= 359)
                {
                    enable = false;
                    StartCoroutine(nameof(NegativeRotationX));
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
            }
}
