using System;
using UnityEngine;
using UnityEngine.InputSystem;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts;
using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;
using static UnityEngine.ParticleSystem;

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
        private MainCPlayerInterface _mainCInterface;

        private TypeOfAim _typeOfAim;

        private Camera _mainCamera;

        private Coroutine _fovCoroutine;

        private ParticleSystem _ps;

        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        [SerializeField] private GameObject bastetObj;
        [SerializeField] private GameObject shootParticles;
        [SerializeField] private GameObject bigBullet;
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
        private bool _isShooting;
        private bool _isChargingShoot;
        private bool _isRecoveringShoot;

        [SerializeField] private float sphereDectorSize = 5f;
        [SerializeField] private float cameraTransitionSpeed = 5f;
        [SerializeField] private float shootingHoldTime = 5f;
        private float _closestDistance = Mathf.Infinity;
        private float _shootingTime;
        private float energySpent;

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
            _mainCInterface = GetComponent<MainCPlayerInterface>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += LeftMouseDown;
            _playerInputActions.Player.Attack.canceled += LeftMouseUp;
            _playerInputActions.Player.SecondaryAttack.performed += RightMouseDown;
            _playerInputActions.Player.SecondaryAttack.canceled += RightMouseUp;

            _mainCamera = cameraObj.GetComponent<Camera>();
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

            if (GameManagerSingleton.Instance.bastetEnergy <= 0 && !_isRecoveringShoot)
            {
                _isRecoveringShoot = true;
                _isChargingShoot = false;
                GameManagerSingleton.Instance.bastetEnergy = 0;
                StartCoroutine(RecoverEnergy());
            }

            if (_isLeftMouseDown && !_isShooting && IsAiming && !_isRecoveringShoot && GameManagerSingleton.Instance.bastetEnergy > 0)
            {
                _shootingTime += Time.deltaTime;
                if (_shootingTime > shootingHoldTime)
                {
                    energySpent = GameManagerSingleton.Instance.bastetEnergy;
                    Shoot(_shootingTime / 2.5f, _shootingTime * 20f);
                }
                else
                {
                    energySpent = GameManagerSingleton.Instance.bastetEnergy * (_shootingTime / shootingHoldTime) * Time.deltaTime;
                }

                GameManagerSingleton.Instance.bastetEnergy -= energySpent;
                _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);

                if (!_isChargingShoot && !_isRecoveringShoot)
                {
                    if (IsAiming)
                    {
                        _isChargingShoot = true;
                        _shootingTime = 0;
                        _currentParticle = Instantiate(shootParticles, _bastetController.muzzles[0].transform.position, Quaternion.identity);
                        _ps = _currentParticle.GetComponentInChildren<ParticleSystem>();
                        _ps.Stop();
                        var main = _ps.main;
                        main.duration = shootingHoldTime;
                        _ps.Play();
                        _currentParticle.transform.parent = _bastetController.muzzles[0].transform;
                    }
                }
            }

        }

        private IEnumerator RecoverEnergy()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                GameManagerSingleton.Instance.bastetEnergy += 5f;
                _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);

                if (GameManagerSingleton.Instance.bastetEnergy > 100f)
                {
                    GameManagerSingleton.Instance.bastetEnergy = 100f;
                    _isRecoveringShoot = false;
                    break;
                }

            }
        }

        private void RightMouseDown(InputAction.CallbackContext context)
        {
            _camFollower.cameraFollow = camAimPosTr;
            _bastetController.HideScanner();
            bastetObj.SetActive(true);
            _bastetController.PosRightHand();
            _bastetController.SetAbilityBastetAttack(true);
            _bastetController.SetMoveToBastetPos(true);

        }

        private void RightMouseUp(InputAction.CallbackContext context)
        {
            _camFollower.cameraFollow = cameraFollow;
            _bastetController.SetAbilityBastetAttack(false);
            _bastetController.SetMoveToBastetPos(false);
        }

        private void LeftMouseDown(InputAction.CallbackContext context)
        {
            _isLeftMouseDown = true;
        }

        private void LeftMouseUp(InputAction.CallbackContext context)
        {
            if(_currentParticle != null && IsAiming)
            {
                Destroy(_currentParticle);
            }

            if(IsAiming && !_isShooting && !_isRecoveringShoot)
            {
                Shoot(_shootingTime / 2.5f, _shootingTime * 15f);
                _mainCInterface.localEnergy = GameManagerSingleton.Instance.bastetEnergy;
            }
            _isLeftMouseDown = false;
            _isChargingShoot = false;
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
        private void Shoot(float sizeBullet, float bulletSpeed)
        {
            if (IsAiming)
            {
                _isShooting = true;
                var ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
                if (Physics.Raycast(ray, out var hit, 20f))
                {
                    _bastetController.Shoot(hit.collider.gameObject.transform.position, bigBullet, true, sizeBullet, bulletSpeed);
                }
                else
                {
                    _bastetController.Shoot(ray.GetPoint(20f), bigBullet, false, sizeBullet, bulletSpeed);
                }
            }
        }

        public void DisableShooting()
        {
            _isShooting = false;
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

    }
}