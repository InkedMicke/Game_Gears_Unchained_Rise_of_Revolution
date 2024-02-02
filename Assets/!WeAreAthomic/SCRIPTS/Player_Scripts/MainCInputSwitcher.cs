using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCInputSwitcher : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    private Vector2 vectorGamepad;
    private Vector2 lastMousePosition;
    private Vector2 currentMousePosition;

    public bool isUsingMouse;
    public bool isUsingGamepad;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void Update()
    {
        UpdateInputValues();

        if (vectorGamepad.magnitude > 1)
        {
            isUsingMouse = true;
        }

        if (currentMousePosition.magnitude > 0.1f)
        {
            isUsingMouse = true;
        }
    }

    private void UpdateInputValues()
    {
        vectorGamepad = _playerInputActions.PlayerGamepad.MovementGamepad.ReadValue<Vector2>();

        currentMousePosition = Mouse.current.position.ReadValue();

        lastMousePosition = currentMousePosition;
    }
}
