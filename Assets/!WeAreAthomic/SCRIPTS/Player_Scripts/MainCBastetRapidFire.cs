using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts
{
    public class MainCBastetRapidFire : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private BastetController _bastetController;

        [SerializeField] private LayerMask enemyLayer;

        [SerializeField] private GameObject bastetObj;
        public GameObject closestEnemyToShoot;

        private bool _isBastetAttacking;

        [SerializeField] private int maxShoots = 4;
        public int currentShoots;
        private int _randomPos;
        private int _lastRandomPos;

        [SerializeField] private float radiusCheck = 10f;
        [SerializeField] private float attackCooldown = 10f;
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

        private void Update()
        {
            if (_isBastetAttacking)
            {
                var colliders = Physics.OverlapSphere(transform.position, radiusCheck, enemyLayer);
                foreach (var col in colliders)
                {
                    closestEnemyToShoot = col.gameObject;

                    if(closestEnemyToShoot.GetComponent<SoldierHurtBox>().IsDeath)
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
            }
        }


        private void InputPC(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
                CheckEnemy();
        }

        private void InputGamepad(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (!_isBastetAttacking && Time.time > _totalCooldown)
            {
                var collider = Physics.OverlapSphere(transform.position, radiusCheck, enemyLayer);
                if (collider.Length > 0)
                {
                    closestEnemyToShoot = collider[0].gameObject;
                    _isBastetAttacking = true;
                    StartAttacking();
                }
            }

        }

        public void DisableBastetAttacking()
        {
            _isBastetAttacking = false;
            _totalCooldown = attackCooldown + Time.time;
        }

        private void StartAttacking()
        {
            bastetObj.SetActive(true);
            _bastetController.HideScanner();
            _bastetController.PosRightHand();
            _bastetController.StartMoveToBastetPos();
            _bastetController.StartShootEnemy(maxShoots);
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
