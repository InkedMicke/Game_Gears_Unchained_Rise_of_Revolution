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
            normalAttack,
            chargedAttack
        }

        protected TypeOfAttack _typeOfAttack;
        private PlayerInputActions _playerInputActions;

        protected private bool _isMouseDown;

        protected private float _mouseMagnitude;

        protected virtual private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += MousePressedDown;
            _playerInputActions.Player.Attack.canceled += MousePressedUp;
        }

        protected virtual private void Update()
        {
            if (_isMouseDown)
            {
                _mouseMagnitude += Time.deltaTime;
            }

            if (_mouseMagnitude > .5f)
            {
                _typeOfAttack = TypeOfAttack.chargedAttack;
            }
            else
            {
                _typeOfAttack = TypeOfAttack.normalAttack;
            }
        }

        private void MousePressedUp(InputAction.CallbackContext context)
        {
            _isMouseDown = false;
        }

        private void MousePressedDown(InputAction.CallbackContext context)
        {
            _mouseMagnitude = 0f;
            _isMouseDown = true;
        }
    }
}
