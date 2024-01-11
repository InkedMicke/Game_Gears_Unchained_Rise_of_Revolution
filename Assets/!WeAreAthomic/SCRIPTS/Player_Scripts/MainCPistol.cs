using UnityEngine;
using UnityEngine.InputSystem;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts;
using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
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
        private MainCRailGrindSystem _mainCRailGrind;
        private CameraFollower _camFollower;
        private MainCAnimatorController _mainCAnim;
        private PlayerInputActions _playerInputActions;
        private BastetController _bastetController;
        private MainCPlayerInterface _mainCInterface;

        private TypeOfAim _typeOfAim;

        private Camera _mainCamera;

        private Coroutine _recoverEnergyCoroutine;

        private ParticleSystem _ps;

        [SerializeField] private PlayerDamageData _pistolAttackData;

        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        [SerializeField] private GameObject bastetObj;
        [SerializeField] private GameObject shootParticles;
        [SerializeField] private GameObject bigBullet;
        [SerializeField] private GameObject crosshair;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform lookAtAim;
        [SerializeField] private Transform cameraFollow;

        [SerializeField] private LayerMask enemyHurtBox;

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
        private float _closestDistance = Mathf.Infinity;
        private float _totalCooldown;

        [SerializeField] private AudioSource shootSoundClip;
        private void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCRailGrind = GetComponent<MainCRailGrindSystem>();
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

            _mainCamera = cameraObj.GetComponent<Camera>();


            if (GameManagerSingleton.Instance.bastetEnergy < 100 && !_isRecoveringShoot && !_isWaitingForRecoveringShoot)
            {
                StartRecoveringEnergy(5f);
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
                crosshair.SetActive(false);
                _camFollower.cameraFollow = cameraFollow;
                _bastetController.StopMoveToBastetPos();
                _bastetController.InvokeMoveToPlayer();
            }

        }

        private void RightControlDown()
        {
            if (CanAim())
            {
                IsAiming = true;
                crosshair.SetActive(true);
                _camFollower.cameraFollow = camAimPosTr;
                _bastetController.HideScanner();
                bastetObj.SetActive(true);
                _bastetController.PosRightHand();
                _bastetController.StartMoveToBastetPos();
            }

        }

        private IEnumerator RecoverEnergy(float waitTime)
        {

            yield return new WaitForSeconds(waitTime);

            _isRecoveringShoot = true;
            _isWaitingForRecoveringShoot = false;

            while (true)
            {
                if (!_isRecoveringShoot)
                {
                    break;
                }
                GameManagerSingleton.Instance.bastetEnergy += 1f;
                _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
                _mainCAttack.SetRunOutEnergy(false);
                if (GameManagerSingleton.Instance.bastetEnergy > 100f)
                {
                    GameManagerSingleton.Instance.bastetEnergy = 100f;
                    _isRecoveringShoot = false;
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
                if (_isRecoveringShoot)
                {
                    StopRecoveringEnergy();
                }
                _isWaitingForRecoveringShoot = false;
                _isRecoveringShoot = false;
                _isShooting = true;
                shootSoundClip.Play();
                var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                Vector3 targetPoint;
                Transform t = null;
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyHurtBox))
                {
                    targetPoint = hit.point;

                    t = hit.transform;

                }
                else
                {
                    targetPoint = ray.GetPoint(75f);
                }
                _bastetController.Shoot(targetPoint, t, bigBullet, bulletSize, bulletSpeed, _pistolAttackData);
                _totalCooldown = Time.time + shootCooldown;
                if (!GameManagerSingleton.Instance.IsUnlimitedEnergy)
                {
                    GameManagerSingleton.Instance.TakeEnergy(energySpend);
                    _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
                }

                if (GameManagerSingleton.Instance.bastetEnergy < 100 && !_isRecoveringShoot && !_isWaitingForRecoveringShoot)
                {
                    StartRecoveringEnergy(5f);
                    _isWaitingForRecoveringShoot = true;
                }
            }
        }

        public void DisableShooting()
        {
            _isShooting = false;
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

            if(GameManagerSingleton.Instance.bastetEnergy < 20)
            {
                return false;
            }

            if(!GameManagerSingleton.Instance.HasUnlockedBastetAttack)
            {
                return false;
            }

            if(Time.time < _totalCooldown)
            {
                return false;
            }

            return true;
        }

    }
}