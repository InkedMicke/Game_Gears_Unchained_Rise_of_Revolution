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
        private MainCMovement _mainCMove;
        private CharacterController _cc;

        [SerializeField] private GameObject movableFloorPivot;

        [SerializeField] private Transform cameraTr;

        private bool toggled;



        private void Awake()
        {
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();
            _mainCMove = GetComponent<MainCMovement>();
            _cc = GetComponent<CharacterController>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            //_playerInputActions.Player.Test.performed += ToggleTime;
            //_playerInputActions.Player.Test.performed += Die;
            _playerInputActions.PlayerPC.Test.performed += TestPC;




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

        private void TestPC(InputAction.CallbackContext context)
        {
            StartCoroutine(PushWait());
        }

        private IEnumerator PushWait()
        {
            while (!_mainCMove.IsGrounded())
            {
                yield return new WaitForEndOfFrame();
            }

            _cc.enabled = false;
            Instantiate(movableFloorPivot, transform.position - Vector3.right * 1.4f, Quaternion.identity);
            _mainCMove.SetFollowTrajectory(true);
        }
    }
}

