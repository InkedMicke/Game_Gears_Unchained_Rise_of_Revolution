using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GODMODE : MonoBehaviour
{
    private CharacterController _cc;
    private PlayerInputActions _playerInputActions;
    private MainCMovement _mainCMove;

    private GameObject _cameraObj;
    
    public bool isGodModeEnabled;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _mainCMove = GetComponent<MainCMovement>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.GodMode.performed += EnableGodMode;
    }

    private void Start()
    {
        _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        GodMode();
    }

    private void EnableGodMode(InputAction.CallbackContext context)
    {
        isGodModeEnabled = !isGodModeEnabled;
    }

    private void GodMode()
    {
        if(isGodModeEnabled)
        {
            var _moveVectorKeyboard = _playerInputActions.Player.MovementKeyboard.ReadValue<Vector2>();

            var direction = new Vector3(_moveVectorKeyboard.x, 0f, _moveVectorKeyboard.y).normalized;
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _mainCMove._turnSmoothVelocityKeyboard,
                _mainCMove.turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            var _moveDir = _cameraObj.transform.forward * (Time.deltaTime * 5f * direction.magnitude);

            _cc.Move(_moveDir);
        }
    }
}
