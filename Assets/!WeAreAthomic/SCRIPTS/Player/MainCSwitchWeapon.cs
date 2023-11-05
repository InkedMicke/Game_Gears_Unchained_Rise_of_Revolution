using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCSwitchWeapon : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private MainCMovement _mainCMovement;
        private MainCAttack _mainCAttack;

        [SerializeField] private GameObject wrenchObj;
        [SerializeField] private GameObject pistolObj;
    
        public bool isUsingWrench = true;
        public bool isUsingPistol;

        private void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
        
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.SwitchWeapon.performed += SwitchWeapon;
        }


        private void SwitchWeapon(InputAction.CallbackContext context)
        {
            if (CanSwitchWeapon())
            {
                isUsingWrench = !isUsingWrench;
                isUsingPistol = !isUsingPistol;
                wrenchObj.SetActive(!wrenchObj.activeInHierarchy);
                pistolObj.SetActive(!pistolObj.activeInHierarchy);
            }
        }

        private bool CanSwitchWeapon()
        {
            if (_mainCAttack.IsAttacking)
            {
                return false;}

            if (_mainCMovement.IsJumping)
            {
                return false;}

            if (_mainCMovement.IsFalling)
            {
                return false;}
        
            return true;

            return false;
        }
    
    }
}