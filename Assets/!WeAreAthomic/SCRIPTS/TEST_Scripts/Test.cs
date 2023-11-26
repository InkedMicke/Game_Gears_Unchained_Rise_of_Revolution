using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.TEST
{
    public class Test : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;

        private bool toggled;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Test.performed += ToggleTime;
        }

        private void ToggleTime(InputAction.CallbackContext context)
        {
            toggled = !toggled;

            Time.timeScale = toggled ? 0.05f : 1f;
        }

    }
}
