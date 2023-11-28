using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class GamePadController : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    private Vector2 vectorGamepad;
    private Vector2 lastMousePosition;

    private bool _isActive;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Update()
    {
        if (GameManagerSingleton.Instance.IsStopMenuEnabled || GameManagerSingleton.Instance.IsSettingsMenuEnabled)
        {
            vectorGamepad = _playerInputActions.Player.MovementGamepad.ReadValue<Vector2>();
            if (vectorGamepad.magnitude > .1f)
            {
                if (!_isActive)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    _isActive = true;
                }

                if(Cursor.visible)
                {
                    Debug.Log("hola1");
                    GameManagerSingleton.Instance.CursorMode(false);
                }
            }
            else
            {
                if(_isActive)
                {
                    _isActive = false;
                }

                var currentMousePosition = Mouse.current.position.ReadValue();

                if (currentMousePosition != lastMousePosition)
                {
                    GameManagerSingleton.Instance.CursorMode(true);
                    // Realiza acciones cuando el ratón se mueve
                }
                else
                {
                    Debug.Log("El ratón está quieto");
                    // Realiza acciones cuando el ratón está quieto
                }

                lastMousePosition = currentMousePosition;
            }
        }
    }
}
