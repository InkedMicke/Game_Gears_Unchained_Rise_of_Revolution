using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCMouseController : MonoBehaviour
    {
        protected enum TypeOfAttack
        {
            NormalAttack,
            ChargedAttack
        }

        protected TypeOfAttack _typeOfAttack;

        private PlayerInputActions _playerInputActions;

        protected bool IsLeftMouseDown;
        protected bool IsRightMouseDown;

        protected float _mouseMagnitude;
        protected float timeToCharged = .2f;

        private protected virtual void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += LeftMousePressedDown;
            _playerInputActions.Player.Attack.canceled += LeftMousePressedUp;
            _playerInputActions.Player.SecondaryAttack.performed += RightMousePressedDown;
            _playerInputActions.Player.SecondaryAttack.canceled += RightMousePressedUp;
        }

        private void Start()
        {
            StartCoroutine(nameof(TestCoroutine));
        }

        private IEnumerator TestCoroutine()
        {
            while (true)
            {
                if (IsLeftMouseDown)
                {
                    _mouseMagnitude += Time.deltaTime;

                }

                if (_mouseMagnitude > timeToCharged)
                {
                    _typeOfAttack = TypeOfAttack.ChargedAttack;
                }
                else
                {
                    _typeOfAttack = TypeOfAttack.NormalAttack;
                }
                yield return new WaitForSeconds(.01f);
            }
        }

        private void LeftMousePressedUp(InputAction.CallbackContext context)
        {
            _mouseMagnitude = 0f;
            IsLeftMouseDown = false;
        }

        private void LeftMousePressedDown(InputAction.CallbackContext context)
        {
            IsLeftMouseDown = true;
        }

        private void RightMousePressedUp(InputAction.CallbackContext context)
        {
            IsRightMouseDown = false;
        }

        private void RightMousePressedDown(InputAction.CallbackContext context)
        {
            IsRightMouseDown = true;
        }


    }
}
