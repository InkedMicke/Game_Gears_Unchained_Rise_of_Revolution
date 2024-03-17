using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MainCInput : MonoBehaviour
    {
        public PlayerInputActions Input => m_input;
        PlayerInputActions m_input;

        public Action OnLeftMouse;

        private void Awake()
        {
            m_input = new PlayerInputActions();
            m_input.Enable();
        }

        public Vector2 GetInput()
        {
            return m_input.PlayerGlobal.Movement.ReadValue<Vector2>();
        }        
       
    }
}