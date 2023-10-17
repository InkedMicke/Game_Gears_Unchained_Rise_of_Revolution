using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player.Robot
{
    public class BastetController : MonoBehaviour
    {
        CharacterController _cc;

        private GameObject _playerObj;

        [SerializeField] private float moveSpeed;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
        }

        public void InvokeMoveToPlayer(float value)
        {
            StartCoroutine(MoveToPlayer(value));
        }

        private IEnumerator MoveToPlayer(float value)
        {
            var canEnableLayer = true;

            while (canEnableLayer)
            {
                var direction = _playerObj.transform.position - transform.position;
                _cc.Move(direction.normalized * moveSpeed);

                if (Vector3.Distance(_playerObj.transform.position, transform.position) < 0.1f)
                {
                    canEnableLayer = false;
                    Destroy(this.gameObject);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
