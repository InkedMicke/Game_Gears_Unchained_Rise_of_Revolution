using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCPistol : MonoBehaviour
    {
        private MainCAttack _mainCAttack;
        private MainCMovement _mainCMovement;
        private MainCLayers _mainCLayers;
        private MainCSwitchWeapon _mainCSwitch;
        private PlayerInputActions _playerInputActions;
        private CameraFollower _camFollower;
    
        [SerializeField] private GameObject cameraObj;
        [SerializeField] private GameObject cameraBaseObj;
        private Transform _closestTransform = null;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform orientation;

        [SerializeField] private LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAiming;
        [System.NonSerialized] public bool IsAutoTargeting;

        [SerializeField] private float sphereDectorSize = 5f;
        public float _camLerpMultiplier;
        private float _closestDistance = Mathf.Infinity;

        private void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCSwitch = GetComponent<MainCSwitchWeapon>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.SecondaryAttack.performed += EnableAim;
            _playerInputActions.Player.SecondaryAttack.canceled += DisableAim;
        }
    
        private void Start()
        {
            _camFollower = cameraBaseObj.GetComponent<CameraFollower>();
        }

        private void Update()
        {
            AutoTargetNear();
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

        private void EnableAim(InputAction.CallbackContext context)
        {
            if (CanAim())
            {
                IsAiming = true;
                _mainCLayers.EnablePistolLayer();
            }
        }

        private void DisableAim(InputAction.CallbackContext context)
        {
            if (IsAiming)
            {
                _mainCLayers.DisablePistolLayer();
                IsAiming = false;
            }
        }

        private bool CanAim()
        {
            if (!_mainCSwitch.isUsingPistol)
            {
                return false;
            }

            return true;
        }
    }
}