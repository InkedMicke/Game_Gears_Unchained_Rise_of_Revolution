using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
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

        private TypeOfAim _typeOfAim;

        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        [SerializeField] private GameObject aimCameraObj;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform lookAtAim;
        private Transform _closestTransform = null;

        [SerializeField] private LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAiming;
        [System.NonSerialized] public bool IsAutoTargeting;
        private bool _isAnimEnabled;

        [SerializeField] private float sphereDectorSize = 5f;
        [SerializeField] private float cameraTransitionSpeed = 5f;
        private float _closestDistance = Mathf.Infinity;

        private protected override void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCSwitch = GetComponent<MainCSwitchWeapon>();;
            _mainCRailGrind = GetComponent<MainCRailGrindSystem>();
            _mainCAnim = GetComponent<MainCAnimatorController>();
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

        private void AutoTargetNear()
        {
            if(IsAiming)
            {
                var colliders = Physics.OverlapSphere(middlePos.position, sphereDectorSize, enemyHurtBox);
                Debug.Log("hola1");

                if(colliders.Length > 1)
                {
                    foreach (var col in colliders)
                    {
                        var distance = Vector3.Distance(transform.position, col.transform.position);

                        if (distance < _closestDistance)
                        {
                            _closestDistance = distance;
                            _closestTransform = col.gameObject.transform;
                        }
                    }
                }
                else
                {
                    if (colliders.Length > 0)
                    {
                        IsAutoTargeting = true;
                        _closestTransform = colliders[0].gameObject.transform;
                    }
                    else
                    {
                        if(IsAutoTargeting)
                        {
                            IsAutoTargeting = false;
                        }
                    }
                }

                if (colliders.Length > 0)
                {
                    var desiredPos = _closestTransform.transform.position;
                    desiredPos = new Vector3(desiredPos.x, transform.position.y, desiredPos.z);
                    transform.LookAt(desiredPos);
                }
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
            
        }

        private void AimingOnRail()
        {
            if(_isRightMouseDown)
            {
                if(!_isAnimEnabled)
                {
                    _mainCLayers.EnablePistolLayer();
                    _mainCAnim.SetAimOnRail(true);
                    _isAnimEnabled = true;
                    IsAiming = true;
                    aimCameraObj.SetActive(true);
                }

                var desiredRot = new Vector3(lookAtAim.position.x, transform.position.y, lookAtAim.position.z);
                transform.LookAt(desiredRot);

            }
            else
            {
                if (_isAnimEnabled)
                {
                    _mainCLayers.DisablePistolLayer();
                    _mainCAnim.SetAimOnRail(false);
                    _isAnimEnabled = false;
                    IsAiming = false;
                    cameraObj.SetActive(true);
                }
                
            }
        }

    }
}