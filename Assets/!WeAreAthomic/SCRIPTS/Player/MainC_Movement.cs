using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCMovement : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;
    private Animator _anim;
    private MainCLayers _mainCLayers;
    private MainCAttack _mainCAttack;
    private MainCPistol _mainCPistol;

    private GameObject _cameraObj;

    public Transform orientation;
    public Transform checkGrounded;

    public LayerMask groundLayer;

    private Vector2 _moveVectorKeyboard;
    private Vector2 _moveVectorGamepad;

    private Vector3 _moveDir;
    private Vector3 _movement;
    private Vector3 _velocity;

    [System.NonSerialized] public bool isCrouch;
    [System.NonSerialized] public bool _isJumping;
    [System.NonSerialized] public bool _isFalling;
    private bool _isRunningKeyboard;
    private bool _isRunningGamepad;
    private bool _isCrouchWalking;
    private bool isUsingGamepad;
    private bool isUsingKeyboard;
    private bool _canMove;
    private bool _isGrounded;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float timeNextCrouch = 0.5f;
    [SerializeField] private float timeNextJump = 0.5f;
    [SerializeField] private float jumpImpulse = 5f;
    [SerializeField] private float gravity = -9.8f;
    private float _moveSpeed;
    private float _horizontal;
    private float _turnSmoothVelocityKeyboard;
    private float _turnSmoothVelocityGamepad;
    private float _timeGraceCrouchPeriod;
    private float _timeGraceJumpPeriod;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _mainCLayers = GetComponent<MainCLayers>();
        _mainCAttack = GetComponent<MainCAttack>();
        _mainCPistol = GetComponent<MainCPistol>();
        
        _playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");

        _canMove = true;
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Running.performed += RunOn;
        _playerInputActions.Player.Running.canceled += RunOff;
        _playerInputActions.Player.Crouch.performed += StartEndCrouch;
        _playerInputActions.Player.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
        _playerInputActions.Player.Running.performed -= RunOn;
        _playerInputActions.Player.Running.canceled -= RunOff;
        _playerInputActions.Player.Crouch.performed -= StartEndCrouch;
        _playerInputActions.Player.Jump.performed -= Jump;
    }

    private void Update()
    {
        AnimatorController();
        if (_canMove && !_mainCAttack.isFinalAttacking)
        {
            MoveKeyboard();
            MoveGamepad();
        }

        CrouchWalking();
        //ApplyGravity();
        
    }

    private void FixedUpdate()
    {
        if (_isJumping || !IsGrounded())
        {
            //_velocity.y += gravity * Time.deltaTime;
            //_cc.Move(_velocity * Time.deltaTime);
        }
    }


    private void AnimatorController()
    {
        if (!IsGrounded() && _velocity.y < 0 )
        {
            _isFalling = true;
            _mainCLayers.EnableJumpLayer();
            _isJumping = false;
            _anim.SetBool(string.Format("isFalling"), _isFalling); // Activa la animaci�n de ca�da
            _anim.SetBool(string.Format("isJumping"), _isJumping);
        }

        if (_isFalling && IsGrounded())
        {
            _isFalling = false;
            _isJumping = false;
            _anim.SetBool(string.Format("isFalling"), _isFalling); // Activa la animaci�n de ca�da
            _anim.SetBool(string.Format("isJumping"), _isJumping);
            _anim.SetBool(string.Format("isGrounded"), true);
            if (isCrouch)
            {
                _mainCLayers.DisableJumpLayer();
            }
        }
    }

    private void ApplyGravity()
    {
        if (IsGrounded()) return;
        //_cc.Move(_velocity * Time.deltaTime);
    }

    private void MoveKeyboard()
    {
        _moveVectorKeyboard = _playerInputActions.Player.MovementKeyboard.ReadValue<Vector2>();

        if (_moveVectorKeyboard.magnitude > 0.1f)
        {
            isUsingKeyboard = true;
            isUsingGamepad = false;
            if (!_isJumping && !_isFalling && !isCrouch)
            {
                InvokeDisableAllLayers();
            }
        }

        if (isUsingGamepad) return;

        var direction = new Vector3(_moveVectorKeyboard.x, 0f, _moveVectorKeyboard.y).normalized;
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityKeyboard,
            turnSmoothTime);

        //_moveDir = orientation.forward * (Time.deltaTime * _moveSpeed * direction.magnitude);

        _anim.SetFloat(string.Format("moveSpeed"), value: _moveSpeed);

        ApplyGravity();

        
       // _cc.Move(_moveDir);

        if (_moveVectorKeyboard.magnitude > 0.1f || _mainCPistol.IsAiming)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        //arreglar cuando mantienes la W se suma y resta a la vez el moveSpeed
        if (_moveVectorKeyboard.magnitude > 0.1f && !_isRunningKeyboard)
        {
            if (_moveSpeed < walkSpeed)
            {
                _moveSpeed += Time.deltaTime * 24;

                if((Mathf.Abs(_moveSpeed - walkSpeed)) < 0.3f)
                {
                    _moveSpeed = walkSpeed;
                }
            }
            
            if(_moveSpeed > walkSpeed)
            {
                _moveSpeed -= Time.deltaTime * 18;
            }
        }
        else
        {
            if (!_isRunningKeyboard || _isRunningKeyboard && _moveVectorKeyboard.magnitude < 0.1f)
            {
                if (_moveSpeed > 0)
                {
                    _moveSpeed -= Time.deltaTime * 18;
                }
                else
                {
                    if (_moveSpeed.Equals(0)) return;
                    _moveSpeed = 0;
                }
            }
        }

        if (!(_moveVectorKeyboard.magnitude > 0.1 && _isRunningKeyboard)) return;
        if (_moveSpeed < runSpeed)
        {
            _moveSpeed += Time.deltaTime * 24;
        }
        else
        {
            if (_moveSpeed.Equals(runSpeed)) return;
            _moveSpeed = runSpeed;
        }
    }
    private void MoveGamepad()
    {
        _moveVectorGamepad = _playerInputActions.Player.MovementGamepad.ReadValue<Vector2>();

        if (_moveVectorGamepad.magnitude > 0.1f)
        {
            isUsingGamepad = true;
            isUsingKeyboard = false;
            if (!_isJumping && !_isFalling && !isCrouch)
            {
                InvokeDisableAllLayers();
            }
        }
        else
        {
            if(_isRunningGamepad)
            {
                _isRunningGamepad = false;
            }
        }


        if (isUsingKeyboard) return;


        _movement = new Vector3(_moveVectorGamepad.x, 0.0f, _moveVectorGamepad.y);

        var moveSpeed = _isRunningGamepad ? runSpeed : walkSpeed;

        var desiredSpeed = _movement.magnitude * moveSpeed / 2 * 2.0f;
;

       // _cc.Move(_movement * Time.deltaTime);

        // Suaviza la transici�n utilizando Lerp para el float de velocidad en el Animator
        var actualSpeed = _anim.GetFloat(string.Format("moveSpeed"));
        var interpolatedSpeed = Mathf.Lerp(actualSpeed, desiredSpeed, Time.deltaTime * 4.0f);
        _anim.SetFloat(string.Format("moveSpeed"), interpolatedSpeed);

        var direction = new Vector3(_moveVectorGamepad.x, 0f, _moveVectorGamepad.y).normalized;
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityGamepad,
            turnSmoothTime);



        var moveDir = orientation.forward * (Time.deltaTime * _moveSpeed * direction.magnitude);

        if (_moveVectorGamepad.magnitude > 0.1)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void RunOn(InputAction.CallbackContext context)
    {
        _isRunningKeyboard = true;

        _isRunningGamepad = !_isRunningGamepad;

    }

    private void RunOff(InputAction.CallbackContext context)
    {
        _isRunningKeyboard = false;
    }

    private void StartEndCrouch(InputAction.CallbackContext context)
    {
        isCrouch = !isCrouch;

        if (Time.time > _timeGraceCrouchPeriod)
        {
            _anim.SetBool(string.Format("isCrouch"), isCrouch);
            _timeGraceCrouchPeriod = Time.time + timeNextCrouch;

            if (isCrouch)
            {
                DisableMovement();
                InvokeDisableAllLayers();
                _mainCLayers.EnableCrouchLayer();
                Invoke(nameof(EnableMovement), 0.5f);
            }
            else
            {
                DisableMovement();
                Invoke(nameof(InvokeDisableCrouchLayer), 0.5f);
                Invoke(nameof(EnableMovement), 0.5f);
            }
        }
    }

    private void InvokeDisableCrouchLayer()
    {
        _mainCLayers.DisableCrouchLayer();
    }

    public void InvokeDisableAllLayers()
    {
        _mainCLayers.DisableCrouchLayer();
        _mainCLayers.DisableJumpLayer();
    }
    
    
    private void CrouchWalking()
    {
        if (isCrouch && (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f))
        {
            _isCrouchWalking = true;
            _anim.SetBool(string.Format("isCrouchWalking"), true);
        }
        else
        {
            if (_isCrouchWalking)
            {
                _isCrouchWalking = false;
                _anim.SetBool(string.Format("isCrouchWalking"), false);
            }
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            if (Time.time > _timeGraceJumpPeriod)
            {
                _mainCLayers.EnableJumpLayer();
                _timeGraceJumpPeriod = Time.time + timeNextJump;
                _isJumping = true;
                _velocity.y = Mathf.Sqrt(jumpImpulse * -2 * gravity);
                _anim.SetBool(string.Format("isJumping"), _isJumping);
            }
        }
    }

    private void EnableMovement()
    {
        _canMove = true;
    }

    private void DisableMovement()
    {
        _canMove = false;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(checkGrounded.position, .1f, groundLayer);
    }
}