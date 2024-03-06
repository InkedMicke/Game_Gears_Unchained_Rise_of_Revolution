using UnityEngine;
using UnityEngine.InputSystem;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts;
using System.Collections;
using Player.Bastet;
using UnityEngine.Events;
using Cinemachine;
using Enemy;
using Enemy.Dummie;
using Generics.Camera;

namespace Player
{
    public class MainCPistol : MonoBehaviour
    {
        private enum TypeOfAim
        {
            GroundAim,
            RailAim
        }

        private MainCAttack _mainCAttack;
        private MainCMovement _mainCMovement;
        private CameraFollower _camFollower;
        private MainCAnimatorController _mainCAnim;
        private PlayerInputActions _playerInputActions;
        private BastetController _bastetController;
        private MainCPlayerInterface _mainCInterface;

        private CinemachineVirtualCamera _mainCamera;
        
        private Coroutine _recoverEnergyCoroutine;


        [SerializeField] private PlayerDamageData _pistolAttackData;

        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        [SerializeField] private GameObject bastetObj;
        [SerializeField] private GameObject shootParticles;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject crosshair;
        [SerializeField] private GameObject hitChispasPrefab;
        [SerializeField] private GameObject destroyBulletChispasPrefab;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform lookAtAim;
        [SerializeField] private Transform cameraFollow;

        [SerializeField] private LayerMask rayLayers;

        [System.NonSerialized] public bool IsAiming;
        [System.NonSerialized] public bool IsAutoTargeting;
        private bool _isRecoveringShoot;
        private bool _isWaitingForRecoveringShoot;
        private bool _isShooting;

        [SerializeField] private float sphereDectorSize = 5f;
        [SerializeField] private float cameraTransitionSpeed = 5f;
        [SerializeField] private float shootingHoldTime = 5f;
        [SerializeField] private float bulletSpeed = 60f;
        [SerializeField] private float bulletSize = 3f;
        [SerializeField] private float energySpend = 20f;
        [SerializeField] private float shootCooldown = 1f;
        [SerializeField] private float shootDistance = 10f;
        [SerializeField] private float bastetToAimPosSpeed = .4f;
        private float _closestDistance = Mathf.Infinity;
        private float _totalCooldown;

        [SerializeField] private AudioSource shootSoundClip;
        [SerializeField] private UnityEvent OnShoot;
        private void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCAnim = GetComponent<MainCAnimatorController>();
            _camFollower = cameraBaseObj.GetComponent<CameraFollower>();
            _bastetController = bastetObj.GetComponent<BastetController>();
            _mainCInterface = GetComponent<MainCPlayerInterface>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.BastetAimAttack.performed += LeftMouseDown;
            _playerInputActions.PlayerPC.SecondaryAttack.performed += RightMouseDown;
            _playerInputActions.PlayerPC.SecondaryAttack.canceled += RightMouseUp;
            _playerInputActions.PlayerGamepad.BastetAimAttack.performed += LeftGamepadDown;
            _playerInputActions.PlayerGamepad.SecondaryAttack.performed += RightGamepadDown;
            _playerInputActions.PlayerGamepad.SecondaryAttack.canceled += RightGamepadUp;

            _mainCamera = cameraObj.GetComponent<CinemachineVirtualCamera>();


            if (GameManagerSingleton.Instance.bastetEnergy < 100 && !_isRecoveringShoot && !_isWaitingForRecoveringShoot)
            {
                StartRecoveringEnergy(5f);
            }
        }

        private void Update()
        {
            if (IsAiming)
            {
                var leftPos = transform.position + Vector3.left;
                var correctPos = new Vector3(leftPos.x, leftPos.y + 1.5f, leftPos.z);
                _bastetController.GoToDesiredPosMoveTowardsWithoutUpdate(correctPos, bastetToAimPosSpeed);
            }
        }

        private void RightMouseDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
                RightControlDown();
        }

        private void RightMouseUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
                RightControlUp();
        }

        private void LeftMouseDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
                LeftControlDown();
        }

        private void RightGamepadDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                RightControlDown();
        }

        private void RightGamepadUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                RightControlUp();
        }

        private void LeftGamepadDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                LeftControlDown();
        }

        private void LeftControlDown()
        {
            if (CanShoot())
            {
                Shoot();
            }
        }

        private void RightControlUp()
        {
            if (IsAiming)
            {
                IsAiming = false;
                crosshair.SetActive(false);
                _camFollower.cameraFollow = cameraFollow;
                _bastetController.GoToRightHandPosUntilReachedPos(true, () => bastetObj.SetActive(false), .5f, .1f);
            }

        }

        private void RightControlDown()
        {
            if (CanAim())
            {
                IsAiming = true;
                _bastetController.KillGoToDesiredPos();
                crosshair.SetActive(true);
                _camFollower.cameraFollow = camAimPosTr;
                _bastetController.HideScanner();
                bastetObj.SetActive(true);
                _bastetController.PosRightHand();
            }

        }

        private IEnumerator RecoverEnergy(float waitTime)
        {

            yield return new WaitForSeconds(waitTime);

            _isRecoveringShoot = true;
            _isWaitingForRecoveringShoot = false;

            while (_isRecoveringShoot && !_isShooting)
            {
                _mainCInterface.ChargeEnergy(1);
               
                if (GameManagerSingleton.Instance.bastetEnergy > 100f)
                {
                    GameManagerSingleton.Instance.bastetEnergy = 100f;
                    _isRecoveringShoot = false;
                }
                if(_isShooting)
                {
                    break;
                }

                yield return new WaitForSeconds(.1f);

            }
        }

        public void StartRecoveringEnergy(float waitTimeEnergy)
        {
            _recoverEnergyCoroutine = StartCoroutine(RecoverEnergy(waitTimeEnergy));
            _isWaitingForRecoveringShoot = true;
        }

        public void StopRecoveringEnergy()
        {
            if (_isRecoveringShoot)
            {
                StopCoroutine(_recoverEnergyCoroutine);
                _isRecoveringShoot = false;
            }
        }

        private void Shoot()
        {
            if (IsAiming)
            {
                if(_isWaitingForRecoveringShoot)
                {
                    StopCoroutine(_recoverEnergyCoroutine);
                }
                _isShooting = true;
                _isWaitingForRecoveringShoot = false;
                _isRecoveringShoot = false;
                shootSoundClip.Play();
                OnShoot.Invoke();
                
                var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                GCameraShake.Instance.ShakeCamera(1f, 1f, .1f);
                if (Physics.Raycast(ray, out RaycastHit hit, shootDistance, rayLayers))
                {
                    Vector3 direction = bastetObj.transform.position - hit.collider.transform.position;
                    var distance = direction.magnitude;
                    var timeToDestroy = distance / (bulletSpeed * direction.normalized.magnitude);
                    _bastetController.Shoot(bulletPrefab, bulletSpeed, hit.point, Vector3.one * bulletSize, true, timeToDestroy, _pistolAttackData);
                    Instantiate(hitChispasPrefab, hit.point, Quaternion.identity);

                    if (hit.collider.TryGetComponent(out SoldierHurtBox hurtbox))
                    {
                        hurtbox.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(_pistolAttackData, hit.collider.gameObject));
                    }
                    else if (hit.collider.TryGetComponent(out DummieHurtBox dummieHurtbox))
                    {
                        dummieHurtbox.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(_pistolAttackData, hit.collider.gameObject));
                    }

                }
                else
                {
                    Vector3 direction = bastetObj.transform.position - ray.GetPoint(shootDistance);
                    var distance = direction.magnitude;
                    var timeToDestroy = distance / (bulletSpeed * direction.normalized.magnitude);
                    _bastetController.Shoot(bulletPrefab, bulletSpeed, ray.GetPoint(shootDistance), Vector3.one * bulletSize, true, timeToDestroy, _pistolAttackData);

                    Instantiate(destroyBulletChispasPrefab, ray.GetPoint(shootDistance), Quaternion.identity);
                }


                _totalCooldown = Time.time + shootCooldown;
                if (!GameManagerSingleton.Instance.IsUnlimitedEnergy)
                {
                    _mainCInterface.TakeEnergy(energySpend);
                    _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
                    
                }
                _isShooting = false;
                if (GameManagerSingleton.Instance.bastetEnergy < 100 && !_isRecoveringShoot && !_isWaitingForRecoveringShoot)
                {
                    StartRecoveringEnergy(5f);
                    _isWaitingForRecoveringShoot = true;
                }

                
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward * shootDistance, Color.red);
        }

        private bool CanAim()
        {
            if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                return false;
            }

            if (!GameManagerSingleton.Instance.HasUnlockedBastetAttack)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsOnDialogue)
            {
                return false;
            }

            return true;
        }

        private bool CanShoot()
        {
            if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.bastetEnergy < 20)
            {
                return false;
            }

            if (!GameManagerSingleton.Instance.HasUnlockedBastetAttack)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsOnDialogue)
            {
                return false;
            }

            if (Time.time < _totalCooldown)
            {
                return false;
            }

            return true;
        }

    }
}