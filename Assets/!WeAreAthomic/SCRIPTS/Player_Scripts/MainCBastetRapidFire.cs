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
        private MainCSounds _mainCSounds;
        private PlayerInputActions _playerInputActions;
        private BastetController _bastetController;
        private MainCPlayerInterface _mainCPlayerInterface;
        private MainCPistol _mainCPistol;

        [SerializeField] private PlayerDamageData _playerDamageData;

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
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCPlayerInterface = GetComponent<MainCPlayerInterface>();
            _mainCPistol = GetComponent<MainCPistol>();
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
                _mainCSounds.PlayBastetCall();
                bastetObj.SetActive(true);
                _bastetController.HideScanner();
                _bastetController.PosRightHand();
                StartCoroutine(Attack());
            }
        }

        private IEnumerator Attack()
        {
            StartCoroutine(MoveWithPlayer());
            while (currentShoots < maxShoots && GameManagerSingleton.Instance.bastetEnergy >= 1 )
            {
                currentShoots++;
               

                var colliders = Physics.OverlapSphere(transform.position, radiusCheck, enemyLayer);
                if(colliders.Length == 1)
                {
                    _bastetController.Shoot(bulletPrefab , bulletForce, colliders[0].transform.position + Vector3.up * .3f, bulletPrefab.transform.localScale, false, 10, _playerDamageData);
                }
                else
                {
                    _bastetController.Shoot(bulletPrefab, bulletForce, FindNearestEnemy().position + Vector3.up * .3f, bulletPrefab.transform.localScale, false, 10, _playerDamageData);
                }
                _mainCPlayerInterface.BastetAbylitiesConsume(1);
                yield return new WaitForSeconds(shootTime);
                
            }
            _isBastetAttacking = false;
            _mainCPistol.StartRecoveringEnergy(.1f);



        }

        private IEnumerator MoveWithPlayer()
        {
            while(true)
            {
                var leftPos = transform.position + Vector3.left;
                var correctPos = new Vector3(leftPos.x, leftPos.y + 1.5f, leftPos.z);
                _bastetController.GoToDesiredPos(null, correctPos, .5f, Ease.Linear);
                yield return new WaitForEndOfFrame();
            }
        }

        public void SetCurrentShoots(int value)
        {
            currentShoots = value;
        }

        private Transform FindNearestEnemy()
        {
            var colliders = Physics.OverlapSphere(transform.position, radiusCheck, enemyLayer);
            Transform enemigoMasCercano = null;
            var distanciaMasCercana = Mathf.Infinity;
            var posicionActual = transform.position;
            bool canContinue = false;

            foreach (Collider enemigo in colliders)
            {
                if (enemigo.TryGetComponent(out SoldierHurtBox soldierHurtBox))
                {
                    canContinue = true;
                    if (soldierHurtBox.IsDeath)
                    {
                        continue;
                    }
                }
                else if (!canContinue && enemigo.TryGetComponent(out DummieHurtBox dummieHurtBox))
                {
                    if (dummieHurtBox.isDeath)
                    {
                        continue;
                    }
                }

                float distanciaEnemigo = Vector3.Distance(posicionActual, enemigo.transform.position);

                if (distanciaEnemigo < distanciaMasCercana)
                {
                    distanciaMasCercana = distanciaEnemigo;
                    enemigoMasCercano = enemigo.transform;
                }
            }

            return enemigoMasCercano;
        }
    }
}
