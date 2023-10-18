using System;
using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player.Robot
{
    public class BastetController : MonoBehaviour
    {
        CharacterController _cc;

        private GameObject _playerObj;
        [SerializeField] private GameObject playerRightArm;

        [SerializeField] private float moveSpeed;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            Debug.Log("Active? "+gameObject.activeInHierarchy);
        }

        public void InvokeMoveToPlayer()
        {
            StartCoroutine(nameof(MoveToPlayer));
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
    }
}
