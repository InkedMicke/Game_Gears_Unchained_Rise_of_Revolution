using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
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

        private bool _isLeftMouseDown;
        protected bool _isRightMouseDown;

        private float _mouseMagnitude;

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
                if (_isLeftMouseDown)
                {
                    _mouseMagnitude += Time.deltaTime;

                }

                if (_mouseMagnitude > .5f)
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
            _isLeftMouseDown = false;
        }

        private void LeftMousePressedDown(InputAction.CallbackContext context)
        {
            _isLeftMouseDown = true;
        }

        private void RightMousePressedUp(InputAction.CallbackContext context)
        {
            _isRightMouseDown = false;
        }

        private void RightMousePressedDown(InputAction.CallbackContext context)
        {
            _isRightMouseDown = true;
        }
    }
}
