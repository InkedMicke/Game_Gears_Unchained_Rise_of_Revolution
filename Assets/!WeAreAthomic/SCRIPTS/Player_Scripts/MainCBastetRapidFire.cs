using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie;
using System.Linq;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts
{
    public class MainCBastetRapidFire : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private BastetController _bastetController;

        [SerializeField] private LayerMask enemyLayer;

        [SerializeField] private GameObject bastetObj;
        [SerializeField] private GameObject bulletPrefab;
        public GameObject closestEnemyToShoot;

        private bool _isBastetAttacking;

        [SerializeField] private int maxShoots = 4;
        public int currentShoots;

        [SerializeField] private float radiusCheck = 10f;
        [SerializeField] private float attackCooldown = 10f;
        [SerializeField] private float shootTime = .2f;
        [SerializeField] private float bulletForce = 20f;
        private float _totalCooldown = 0f;
        private float _closestDistance = Mathf.Infinity;

        private void Awake()
        {
            _bastetController = bastetObj.GetComponent<BastetController>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.BastetAttack.performed += InputPC;
            _playerInputActions.PlayerGamepad.BastetAttack.performed += InputGamepad;
        }

        private void InputPC(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.RapidFire)
                StartAttacking();
        }

        private void InputGamepad(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.RapidFire)
                StartAttacking();
        }

        public void DisableBastetAttacking()
        {
            _isBastetAttacking = false;
            _totalCooldown = attackCooldown + Time.time;
        }

        private void StartAttacking()
        {
            var colliders = new Collider[1];
            var colliderCount = Physics.OverlapSphereNonAlloc(transform.position, radiusCheck, colliders, enemyLayer);
            if (!_isBastetAttacking && Time.time > _totalCooldown && colliderCount > 0)
            {

                bastetObj.SetActive(true);
                _bastetController.HideScanner();
                _bastetController.PosRightHand();
                StartCoroutine(Attack());
            }
        }

        private IEnumerator Attack()
        {
            StartCoroutine(MoveWithPlayer());
            while (currentShoots < maxShoots)
            {
                currentShoots++;

                var colliders = Physics.OverlapSphere(transform.position, radiusCheck, enemyLayer);
                if(colliders.Length == 1)
                {
                    _bastetController.Shoot(bulletPrefab , bulletForce, colliders[0].transform.position, bulletPrefab.transform.localScale, false, 10);
                }
                else
                {
                    foreach (var col in colliders)
                    {
                        closestEnemyToShoot = col.gameObject;
                        if (closestEnemyToShoot.TryGetComponent(out SoldierHurtBox _))
                        {
                            _closestDistance = Mathf.Infinity;
                        }
                        else if (closestEnemyToShoot.TryGetComponent(out DummieHurtBox _))
                        {
                            _closestDistance = Mathf.Infinity;
                        }

                        var distance = Vector3.Distance(transform.position, col.transform.position);
                        if (distance < _closestDistance)
                        {
                            _closestDistance = distance;
                            closestEnemyToShoot = col.gameObject;
                        }
                    }

                    _bastetController.Shoot(bulletPrefab, bulletForce, closestEnemyToShoot.transform.position, bulletPrefab.transform.localScale, false, 10);
                }

                yield return new WaitForSeconds(shootTime);
            }
            _isBastetAttacking = false;

        }

        private IEnumerator MoveWithPlayer()
        {
            while(_isBastetAttacking)
            {
                var leftPos = transform.position + Vector3.left;
                var correctPos = new Vector3(leftPos.x, leftPos.y + 1.5f, leftPos.z);
                _bastetController.GoToDesiredPos(null, correctPos, 1f, Ease.Linear);
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radiusCheck);
        }

        public void SetCurrentShoots(int value)
        {
            currentShoots = value;
        }
    }
}
