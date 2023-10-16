using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCMovement : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private PlayerInputActions _playerInputActions;
        private Animator _anim;
        private MainCLayers _mainCLayers;
        private MainCAttack _mainCAttack;
        private MainCPistol _mainCPistol;
        private RailGrindSystem _railGrindSystem;
        private CharacterController _cc;
        private GODMODE _godMode;

        private GameObject _cameraObj;

        public Transform orientation;
        public Transform checkGrounded;

        public LayerMask groundLayer;

        private Vector2 _moveVectorKeyboard;
        private Vector2 _moveVectorGamepad;

        private Vector3 _moveDir;
        private Vector3 _movement;
        private Vector3 _velocity;

        [System.NonSerialized] public bool IsCrouch;
        [System.NonSerialized] public bool IsJumping;
        [System.NonSerialized] public bool IsFalling;
        private bool _isRunningKeyboard;
        private bool _isRunningGamepad;
        private bool _isCrouchWalking;
        private bool _isUsingGamepad;
        private bool _isUsingKeyboard;
        private bool _canMove;
        private bool _isGrounded;

        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        public float turnSmoothTime = 0.1f;
        [SerializeField] private float timeNextCrouch = 0.5f;
        [SerializeField] private float timeNextJump = 0.5f;
        [SerializeField] private float jumpImpulse = 5f;
        [SerializeField] private float jumpImpulseOnRail = 5f;
        [SerializeField] private float gravity = -9.8f;
        private float _moveSpeed;
        private float _horizontal;
        [System.NonSerialized] public float _turnSmoothVelocityKeyboard;
        private float _turnSmoothVelocityGamepad;
        private float _timeGraceCrouchPeriod;
        private float _timeGraceJumpPeriod;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCPistol = GetComponent<MainCPistol>();
            _cc = GetComponent<CharacterController>();
            _godMode = GetComponent<GODMODE>();
            _railGrindSystem = GetComponent<RailGrindSystem>();

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
            if (_canMove && !_godMode.isGodModeEnabled && !_railGrindSystem.CanJumpOnRail)
            {
                MoveKeyboard();
                MoveGamepad();
            }

            CrouchWalking();
            ApplyGravity();
        }


        private void AnimatorController()
        {
            if (!_godMode.isGodModeEnabled)
            {
                if (!IsGrounded() && _velocity.y < 0 && !_railGrindSystem.IsOnRail() || !_railGrindSystem.IsOnRail() && _velocity.y < 0 && !IsGrounded())
                {
                    IsFalling = true;
                    IsJumping = false;
                    _anim.SetBool(string.Format("isFalling"), IsFalling); // Activa la animaci�n de ca�da
                    _anim.SetBool(string.Format("isJumping"), IsJumping);
                }

                if (IsFalling && IsGrounded() ||IsFalling && _railGrindSystem.IsOnRail())
                {
                    IsFalling = false;
                    IsJumping = false;
                    _anim.SetBool(string.Format("isFalling"), IsFalling); // Activa la animaci�n de ca�da
                    _anim.SetBool(string.Format("isJumping"), IsJumping);
                    _anim.SetBool(string.Format("isGrounded"), true);
                    if (IsCrouch)
                    {
                        _mainCLayers.DisableJumpLayer();
                    }
                    _timeGraceJumpPeriod = Time.time + timeNextJump;
                }
            }
        }

        private void ApplyGravity()
        {
            if (IsJumping || !IsGrounded() && !_godMode.isGodModeEnabled && !_railGrindSystem.IsOnRail())
            {
                _velocity += transform.up.normalized * (gravity * Time.deltaTime);
                _velocity.z = 0f;
                _cc.Move(_velocity * Time.deltaTime);
            }
        }

        private void MoveKeyboard()
        {
            _moveVectorKeyboard = _playerInputActions.Player.MovementKeyboard.ReadValue<Vector2>();

            if (_moveVectorKeyboard.magnitude > 0.1f)
            {
                _isUsingKeyboard = true;
                _isUsingGamepad = false;
            }
 
            if (_isUsingGamepad) return;

            var direction = new Vector3(_moveVectorKeyboard.x, 0f, _moveVectorKeyboard.y).normalized;
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityKeyboard,
                turnSmoothTime);

            _moveDir = orientation.forward * (Time.deltaTime * _moveSpeed * direction.magnitude);

            _anim.SetFloat(string.Format("moveSpeed"), value: _moveSpeed);

            ApplyGravity();

            _cc.Move(_moveDir);

            if (_moveVectorKeyboard.magnitude > 0.1f && !_mainCPistol.IsAiming)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            if (_mainCPistol.IsAiming)
            {
                transform.rotation = Quaternion.Euler(0f, _cameraObj.transform.eulerAngles.y, 0f);
            }

            //arreglar cuando mantienes la W se suma y resta a la vez el moveSpeed
            if (_moveVectorKeyboard.magnitude > 0.1f && !_isRunningKeyboard)
            {
                if (_moveSpeed < walkSpeed)
                {
                    _moveSpeed += Time.deltaTime * 24;

                    if ((Mathf.Abs(_moveSpeed - walkSpeed)) < 0.3f)
                    {
                        _moveSpeed = walkSpeed;
                    }
                }

                if (_moveSpeed > walkSpeed)
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
                _isUsingGamepad = true;
                _isUsingKeyboard = false;
                if (!IsJumping && !IsFalling && !IsCrouch && !_mainCAttack.isAttacking)
                {
                    InvokeDisableAllLayers();
                    _mainCAttack.timeGraceAttackPeriod = Time.time + .2f;
                }
            }
            else
            {
                if (_isRunningGamepad)
                {
                    _isRunningGamepad = false;
                }
            }


            if (_isUsingKeyboard) return;


            _movement = new Vector3(_moveVectorGamepad.x, 0.0f, _moveVectorGamepad.y);

            var moveSpeed = _isRunningGamepad ? runSpeed : walkSpeed;

            var desiredSpeed = _movement.magnitude * moveSpeed / 2 * 2.0f;
        

            _cc.Move(_movement * Time.deltaTime);
        
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
            IsCrouch = !IsCrouch;

            if (Time.time > _timeGraceCrouchPeriod)
            {
                _anim.SetBool(string.Format("isCrouch"), IsCrouch);
                _timeGraceCrouchPeriod = Time.time + timeNextCrouch;

                if (IsCrouch)
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
            _mainCLayers.DisableAttackLayer();
            _mainCLayers.DisablePistolLayer();
        }


        private void CrouchWalking()
        {
            if (IsCrouch && (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f))
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
            if (IsGrounded() && !_railGrindSystem.IsOnRail())
            {
                if (Time.time > _timeGraceJumpPeriod)
                {
                    _mainCLayers.EnableJumpLayer();
                    IsJumping = true;
                    _velocity.y = Mathf.Sqrt(jumpImpulse * -2 * gravity);
                    _anim.SetBool(string.Format("isJumping"), IsJumping);
                    _anim.SetBool(string.Format("isGrounded"), false);
                }
            }

            if (!IsGrounded() && _railGrindSystem.IsOnRail())
            {
                if (Time.time > _timeGraceJumpPeriod)
                {
                    _mainCLayers.EnableJumpLayer();
                    IsJumping = true;
                    _velocity.y = Mathf.Sqrt(jumpImpulseOnRail * -2 * gravity);
                    _anim.SetBool(string.Format("isJumping"), IsJumping);
                    _anim.SetBool(string.Format("isGrounded"), false);
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
}