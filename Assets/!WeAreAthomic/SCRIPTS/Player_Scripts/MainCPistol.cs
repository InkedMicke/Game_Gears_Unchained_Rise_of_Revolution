using System;
using UnityEngine;
using UnityEngine.InputSystem;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts;
using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCPistol : MainCMouseController
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

        private TypeOfAim _typeOfAim;

        private Camera _mainCamera;

        private Coroutine _fovCoroutine;

        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        [SerializeField] private GameObject bastetObj;
        [SerializeField] private GameObject shootParticles;
        [SerializeField] private GameObject bigBullet;

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
        private bool _isShooting;
        private bool _isFovCorutineRunning;

        [SerializeField] private float sphereDectorSize = 5f;
        [SerializeField] private float cameraTransitionSpeed = 5f;
        [SerializeField] private float shootingCooldown = 0.5f;
        private float _closestDistance = Mathf.Infinity;
        private float _initialFov;
        private float _totalCooldown;

        private protected override void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCSwitch = GetComponent<MainCSwitchWeapon>();;
            _mainCRailGrind = GetComponent<MainCRailGrindSystem>();
            _mainCAnim = GetComponent<MainCAnimatorController>();
            _camFollower = cameraBaseObj.GetComponent<CameraFollower>();
            _bastetController = bastetObj.GetComponent<BastetController>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += Shoot;
            _playerInputActions.Player.SecondaryAttack.performed += RightMouseDown;
            _playerInputActions.Player.SecondaryAttack.canceled += RightMouseUp;

            _mainCamera = cameraObj.GetComponent<Camera>();
            _initialFov = _mainCamera.fieldOfView;
            base.Awake();
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
            _camFollower.cameraFollow = camAimPosTr;
            _bastetController.HideScanner();
            bastetObj.SetActive(true);
            _bastetController.SetAbilityBastetAttack(true);
            _bastetController.SetMoveToBastetPos(true);

        }

        private void RightMouseUp(InputAction.CallbackContext context)
        {
            _camFollower.cameraFollow = cameraFollow;
            _bastetController.SetAbilityBastetAttack(false);
            _bastetController.SetMoveToBastetPos(false);
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
        private void Shoot(InputAction.CallbackContext context)
        {
            if (Time.time > _totalCooldown && IsAiming)
            {
                var ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
                if (Physics.Raycast(ray, out var hit, 20f))
                {
                    _totalCooldown = Time.time + shootingCooldown;
                    _bastetController.StartShoot(hit.collider.gameObject.transform.position, shootParticles, bigBullet, true);
                }
                else
                {
                    _totalCooldown = Time.time + shootingCooldown;
                    _bastetController.StartShoot(ray.GetPoint(20f), shootParticles, bigBullet, false);
                }
            }
        }

        private void AimingOnRail()
        {
            if(IsRightMouseDown)
            {
                if(!_isAnimEnabled)
                {
                    _mainCLayers.EnablePistolLayer();
                    _mainCAnim.SetAimOnRail(true);
                    _isAnimEnabled = true;
                    IsAiming = true;
                }

            }
            else
            {
                if (_isAnimEnabled)
                {
                    _mainCLayers.DisablePistolLayer();
                    _mainCAnim.SetAimOnRail(false);
                    _isAnimEnabled = false;
                    IsAiming = false;
                }
                
            }
        }

        private IEnumerator FovCamera(string opera)
        {
            if(opera == "+")
            {
                _isFovCorutineRunning = true;
                while (true)
                {
                    if(_mainCamera.fieldOfView <= _initialFov - 5)
                    {
                        _mainCamera.fieldOfView = _initialFov - 10;
                        _isFovCorutineRunning = false;
                        break;
                    }
                    _mainCamera.fieldOfView -= Time.deltaTime * 50f;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else if(opera == "-") 
            {
                _isFovCorutineRunning = true;
                while (true)
                {
                    if (_mainCamera.fieldOfView >= _initialFov)
                    {
                        _isFovCorutineRunning = false;
                        _mainCamera.fieldOfView = _initialFov;
                        break;
                    }
                    _mainCamera.fieldOfView += Time.deltaTime * 50f;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        private void DisableShooting()
        {
            _isShooting = false;
        }

    }
}