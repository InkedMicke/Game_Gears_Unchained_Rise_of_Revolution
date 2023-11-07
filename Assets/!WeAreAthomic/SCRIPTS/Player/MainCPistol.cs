using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCPistol : MainCMouseController
    {
        private enum typeOfAim
        {
            groundAim,
            railAim
        }

        private MainCAttack _mainCAttack;
        private MainCMovement _mainCMovement;
        private MainCLayers _mainCLayers;
        private MainCSwitchWeapon _mainCSwitch;
        private MainCRailGrindSystem _mainCRailGrind;
        private CameraFollower _camFollower;
        private MainCAnimatorController _mainCAnim;

        private typeOfAim _typeOfAim;

        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        private Transform _closestTransform = null;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform orientation;

        [SerializeField] private LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAiming;
        [System.NonSerialized] public bool IsAutoTargeting;
        private bool _isAnimEnabled;

        [SerializeField] private float sphereDectorSize = 5f;
        public float _camLerpMultiplier;
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
                _typeOfAim = typeOfAim.groundAim;
            }

            if(_mainCRailGrind.IsOnRail())
            {
                _typeOfAim = typeOfAim.railAim;
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
                    foreach (Collider col in colliders)
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
                case typeOfAim.groundAim:
                    AimingOnGround();
                    break;
                case typeOfAim.railAim:
                    AimingOnRail();
                    break;
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
                }

            }
            else
            {
                if (_isAnimEnabled)
                {
                    _mainCLayers.DisablePistolLayer();
                    _mainCAnim.SetAimOnRail(false);
                    _isAnimEnabled = false;
                }
            }
        }

    }
}