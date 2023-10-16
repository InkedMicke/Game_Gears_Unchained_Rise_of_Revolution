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
    
        [SerializeField] private GameObject cameraObj;

        [SerializeField] private Transform camAimPosTr;
        [SerializeField] private Transform middlePos;

        [SerializeField] private LayerMask enemyHurtBox;

        private Vector3 _camStartPos;

        private Quaternion _camStartRot;

        [System.NonSerialized] public bool IsAiming;

        [SerializeField] private float sphereDectorSize = 5f;
        public float _camLerpMultiplier;

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
            _camStartPos = cameraObj.transform.position;
            _camStartRot = cameraObj.transform.rotation;
        }

        private void Update()
        {
        }

        private void AutoTargetNear()
        {
            if(IsAiming)
            {
                var colliders = Physics.OverlapSphere(middlePos.position, sphereDectorSize, enemyHurtBox);

                if(colliders.Length > 1)
                {

                }
                else
                {
                    var desiredPos = colliders[0].gameObject.transform.position;
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