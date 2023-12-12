using System;
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
        private MainCLayers _mainCLayers;
        private MainCSwitchWeapon _mainCSwitch;
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
        private GameObject _currentParticle;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform lookAtAim;
        [SerializeField] private Transform cameraFollow;
        private Transform _closestTransform = null;

        [SerializeField] private LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAiming;
        [System.NonSerialized] public bool IsAutoTargeting;
        private bool _isAnimEnabled;
        private bool _isLeftMouseDown;
        private bool _isRightMouseDown;
        private bool _isShooting;
        private bool _isChargingShoot;
        private bool _isRecoveringShoot;
        private bool _isWaitingForRecoveringShoot;

        [SerializeField] private float sphereDectorSize = 5f;
        [SerializeField] private float cameraTransitionSpeed = 5f;
        [SerializeField] private float shootingHoldTime = 5f;
        [SerializeField] private float bulletSpeed = 60f;
        [SerializeField] private float bulletSize = 3f;
        [SerializeField] private float energySpend = 20f;
        [SerializeField] private float shootCooldown = 1f;
        private float _closestDistance = Mathf.Infinity;
        private float _totalCooldown;

        private void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCSwitch = GetComponent<MainCSwitchWeapon>();;
            _mainCRailGrind = GetComponent<MainCRailGrindSystem>();
            _mainCAnim = GetComponent<MainCAnimatorController>();
            _camFollower = cameraBaseObj.GetComponent<CameraFollower>();
            _bastetController = bastetObj.GetComponent<BastetController>();
            _mainCInterface = GetComponent<MainCPlayerInterface>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();           
            _playerInputActions.Player.BastetAimAttack.performed += LeftMouseDown;
            _playerInputActions.Player.BastetAimAttack.canceled += LeftMouseUp;
            _playerInputActions.Player.SecondaryAttack.performed += RightMouseDown;
            _playerInputActions.Player.SecondaryAttack.canceled += RightMouseUp;

            _mainCamera = cameraObj.GetComponent<Camera>();


            if (GameManagerSingleton.Instance.bastetEnergy < 100 && !_isRecoveringShoot && !_isWaitingForRecoveringShoot)
            {
                StartRecoveringEnergy(5f);
            }
        }

        private protected void Update()
        {

            if(_mainCMovement.IsGrounded())
            {
                _typeOfAim = TypeOfAim.GroundAim;
            }

            if(_mainCRailGrind.IsOnRail())
            {
                _typeOfAim = TypeOfAim.RailAim;
            }

            Aim();

        }

        private void RightMouseDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.HasUnlockedBastetAttack && !GameManagerSingleton.Instance.IsSettingsMenuEnabled && !GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                crosshair.SetActive(true);
                _camFollower.cameraFollow = camAimPosTr;
                _bastetController.HideScanner();
                bastetObj.SetActive(true);
                _bastetController.PosRightHand();
                _bastetController.StartMoveToBastetPos();
            }
            _isRightMouseDown = true;
        }

        private void RightMouseUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.HasUnlockedBastetAttack && !GameManagerSingleton.Instance.IsSettingsMenuEnabled && !GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                crosshair.SetActive(false);
                _camFollower.cameraFollow = cameraFollow;
                _bastetController.StopMoveToBastetPos();
                _bastetController.InvokeMoveToPlayer();
            }
            _isRightMouseDown = false;
        }

        private void LeftMouseDown(InputAction.CallbackContext context)
        {
            _isLeftMouseDown = true;
            if(GameManagerSingleton.Instance.bastetEnergy >= 20 && Time.time > _totalCooldown)
            {
                Shoot();
            }
        }

        private void LeftMouseUp(InputAction.CallbackContext context)
        {
            _isLeftMouseDown = false;
        }

        private IEnumerator RecoverEnergy(float waitTime)
        {

            yield return new WaitForSeconds(waitTime);

            _isRecoveringShoot = true;
            _isWaitingForRecoveringShoot = false;

            while (true)
            {
                if(!_isRecoveringShoot)
                {
                    break;
                }
                GameManagerSingleton.Instance.bastetEnergy += .5f;
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



        private void Aim()
        {
            switch(_typeOfAim)
            {
                case TypeOfAim.GroundAim:
                    AimingOnGround();
                    break;
                case TypeOfAim.RailAim:
                    AimingOnRail();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AimingOnGround()
        {
            AimingOnRail();
        }

        public void StartRecoveringEnergy(float waitTimeEnergy)
        {
            _recoverEnergyCoroutine = StartCoroutine(RecoverEnergy(waitTimeEnergy));
            _isWaitingForRecoveringShoot = true;
        }

        public void StopRecoveringEnergy()
        {
            StopCoroutine(_recoverEnergyCoroutine);
            _isRecoveringShoot = false;
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
                var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                RaycastHit hit;

                Vector3 targetPoint;

                Transform t = null;

                if (Physics.Raycast(ray, out hit)) {
                    targetPoint = hit.point;
                    if(hit.collider.gameObject.layer == enemyHurtBox)
                    t = hit.transform;

                }
                else {
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

        private void AimingOnRail()
        {
            if(_isRightMouseDown)
            {
                if(!_isAnimEnabled)
                {
                    _mainCAnim.SetAimOnRail(true);
                    _isAnimEnabled = true;
                    IsAiming = true;
                }

            }
            else
            {
                if (_isAnimEnabled)
                {
                    _mainCAnim.SetAimOnRail(false);
                    _isAnimEnabled = false;
                    IsAiming = false;
                }
                
            }
        }

    }
}