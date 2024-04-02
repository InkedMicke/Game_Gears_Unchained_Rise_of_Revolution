using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MainCInput : MonoBehaviour
    {
        public PlayerInputActions Input => m_input;
        PlayerInputActions m_input;

        public Action OnLeftMouseDown;
        public Action OnLeftMouseUp;

        private void Awake()
        {
            m_input = new PlayerInputActions();
            m_input.Enable();
            m_input.PlayerPC.Attack.performed += InvokeLeftMouseDown;
            m_input.PlayerPC.Attack.canceled += InvokeLeftMouseUp;
        }

        void InvokeLeftMouseDown(InputAction.CallbackContext x)
        {
            OnLeftMouseDown?.Invoke();
        }

        void InvokeLeftMouseUp(InputAction.CallbackContext x)
        {
            OnLeftMouseUp?.Invoke();
        }

        public Vector2 GetInput()
        {
            return m_input.PlayerGlobal.Movement.ReadValue<Vector2>();
        }        
       
    }
}