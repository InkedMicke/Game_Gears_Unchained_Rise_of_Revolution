using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.TEST
{
    public class Test : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;

        [SerializeField] private GameObject stopMenu;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Escape.performed += OpenStopMenu;
        }

        private void OpenStopMenu(InputAction.CallbackContext context)
        {
            stopMenu.SetActive(true);
        }
    }
}

