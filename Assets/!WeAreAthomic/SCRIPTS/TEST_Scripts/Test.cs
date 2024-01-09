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
        private MainCHealthManager _mainCHealth;

        [SerializeField] private Transform cameraTr;

        private bool toggled;



        private void Awake()
        {
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            //_playerInputActions.Player.Test.performed += ToggleTime;
            //_playerInputActions.Player.Test.performed += Die;

            


        }

        private void Update()
        {
 
        }


        private void ToggleTime(InputAction.CallbackContext context)
        {
            toggled = !toggled;

            Time.timeScale = toggled ? 0.05f : 1f;
        }

        private void Die(InputAction.CallbackContext context)
        {
            _mainCHealth.Damage(_mainCHealth.currentHealth);
        }

        public void DebugText(string text)
        {
            Debug.Log(text);
        }

    }
}
