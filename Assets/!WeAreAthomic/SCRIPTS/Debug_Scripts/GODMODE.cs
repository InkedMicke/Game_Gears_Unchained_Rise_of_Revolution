using System;
using System.Collections;
using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class GODMODE : MonoBehaviour
{
    private CharacterController _cc;
    private PlayerInputActions _playerInputActions;
    private MainCMovement _mainCMove;

    private GameObject _cameraObj;

    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask ignoreLayer;

    public bool isGodModeEnabled;
    private bool _isSpeedingUp;
    private bool _isSpeedingDown;

    private float _moveSpeed;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _mainCMove = GetComponent<MainCMovement>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.GodMode.performed += EnableGodMode;
        _playerInputActions.Player.Running.performed += ShiftDown;
        _playerInputActions.Player.Running.canceled += ShiftUp;
        _playerInputActions.Player.Slow.performed += CtrlDown;
        _playerInputActions.Player.Slow.canceled += CtrlUp;
    }

    private void Start()
    {
        _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");

        var gameObj = transform.parent.gameObject;
    }

    private void Update()
    {
        GodMode();
        AdjustSpeed();
        var layerValue = isGodModeEnabled ? 14 : 0;
        var validLayer = Mathf.Clamp(layerValue, 0, 31);
        this.gameObject.layer = validLayer;
    }

    private void EnableGodMode(InputAction.CallbackContext context)
    {
        isGodModeEnabled = !isGodModeEnabled;
    }

    private void GodMode()
    {
        if(isGodModeEnabled)
        {
            var moveVectorKeyboard = _playerInputActions.Player.MovementKeyboard.ReadValue<Vector2>();

            var direction = new Vector3(moveVectorKeyboard.x, 0f, moveVectorKeyboard.y).normalized;
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _mainCMove._turnSmoothVelocityKeyboard,
                _mainCMove.turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            var moveDir = _cameraObj.transform.forward * (Time.deltaTime * 5f * direction.magnitude * _moveSpeed);

            _cc.Move(moveDir);
        }
    }

    private void AdjustSpeed()
    {
        if (_isSpeedingUp)
        {
            _moveSpeed += Time.deltaTime * 4f;
        }

        if (_isSpeedingDown)
        {
            if (_moveSpeed > 0)
            {
                _moveSpeed -= Time.deltaTime * 4f;
            }
        }
    }

    private void ShiftUp(InputAction.CallbackContext context)
    {
        if (isGodModeEnabled)
        {
            _isSpeedingUp = false;
        }
    }
    private void ShiftDown(InputAction.CallbackContext context)
    {
        if (isGodModeEnabled)
        {
            _isSpeedingUp = true;
        }
    }

    private void CtrlUp(InputAction.CallbackContext context)
    {
        if (isGodModeEnabled)
        {
            _isSpeedingDown = false;
        }
    }

    private void CtrlDown(InputAction.CallbackContext context)
    {
        if (isGodModeEnabled)
        {
            _isSpeedingDown = true;
        }
    }
    
}
