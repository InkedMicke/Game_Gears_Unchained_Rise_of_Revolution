using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadController : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    [SerializeField] private GameObject gamepadPointer;

    private Vector2 vectorGamepad;

    private bool _isActive;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Update()
    {
        if(GameManagerSingleton.Instance.IsStopMenuEnabled)
        {
            vectorGamepad = _playerInputActions.Player.MovementGamepad.ReadValue<Vector2>();
            if (vectorGamepad.magnitude > .1f)
            {
                if(!_isActive)
                {
                    gamepadPointer.SetActive(true);
                    _isActive = true;
                }
            }
        }

        if(_isActive)
        {
            PointerPositionOnScreen();
        }
    }

    private void PointerPositionOnScreen()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var speed = 5f;
        Vector2 newPosition = mousePosition + vectorGamepad * speed * Time.deltaTime;
        Mouse.current.WarpCursorPosition(newPosition);
    }
}
