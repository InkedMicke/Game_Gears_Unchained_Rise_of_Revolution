using _WeAreAthomic.SCRIPTS.Debug_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCMovement : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private PlayerInputActions _playerInputActions;
        private Animator _anim;
        private MainCLayers _mainCLayers;
        private MainCAttack _mainCAttack;
        private MainCPistol _mainCPistol;
        private MainCRailGrindSystem _railGrindSystem;
        private MainCHackingSystem _mainCHacking;
        private MainCAnimatorController _mainCAnimator;
        private CharacterController _cc;
        private Godmode _godMode;

        [SerializeField] private LayerMask playerBulletLayer;

        [SerializeField] private GameObject cameraBaseObj;
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
        private bool isMoveWhileAimingWPressed;
        private bool isMoveWhileAimingSPressed;
        private bool isMoveWhileAimingAPressed;
        private bool isMoveWhileAimingDPressed;

        [System.NonSerialized] public float _turnSmoothVelocityKeyboard;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float timeNextCrouch = 0.5f;
        [SerializeField] private float timeNextJump = 0.5f;
        [SerializeField] private float jumpImpulse = 5f;
        [SerializeField] private float jumpImpulseOnRail = 5f;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float aimSpeed;
        public float turnSmoothTime = 0.1f;
        private float _moveSpeed;
        private float _horizontal;
        private float _turnSmoothVelocityGamepad;
        private float _timeGraceCrouchPeriod;
        private float _timeGraceJumpPeriod;
        private float _moveAimingX;
        private float _moveAimingY;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCPistol = GetComponent<MainCPistol>();
            _cc = GetComponent<CharacterController>();
            _godMode = GetComponent<Godmode>();
            _railGrindSystem = GetComponent<MainCRailGrindSystem>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
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
            //_playerInputActions.Player.Test.performed += ReloadScene;
        }

        private void OnDisable()
        {
            _playerInputActions.Disable();
            _playerInputActions.Player.Running.performed -= RunOn;
            _playerInputActions.Player.Running.canceled -= RunOff;
            _playerInputActions.Player.Crouch.performed -= StartEndCrouch;
        }

        private void Update()
        {
            AnimatorController();
            if (_canMove && !GameManagerSingleton.Instance.IsGodModeEnabled && !_railGrindSystem.CanJumpOnRail && !_mainCHacking.isHackingAnim && !_mainCAttack.IsFinalAttacking)
            {
                MoveKeyboard();
                MoveGamepad();
            }

            CrouchWalking();
            ApplyGravity();
        }

        private void ReloadScene(InputAction.CallbackContext context)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        private void AnimatorController()
        {
            if (!GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                if (!IsGrounded() && _velocity.y < 0 && !_railGrindSystem.IsOnRail() || !_railGrindSystem.IsOnRail() && _velocity.y < 0 && !IsGrounded())
                {
                    IsFalling = true;
                    IsJumping = false;
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(IsJumping);
                }

                if (IsFalling && IsGrounded() ||IsFalling && _railGrindSystem.IsOnRail())
                {
                    IsFalling = false;
                    IsJumping = false;
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(IsJumping);
                    _mainCAnimator.SetGrounded(true);
                    if (IsCrouch)
                    {
                    }
                    _timeGraceJumpPeriod = Time.time + timeNextJump;
                }
            }
            _mainCAnimator.SetPistolMoveX(_moveAimingX);
            _mainCAnimator.SetPistolMoveY(_moveAimingY);
        }

        private void ApplyGravity()
        {
            if (IsJumping || !IsGrounded() && !GameManagerSingleton.Instance.IsGodModeEnabled && !_railGrindSystem.IsOnRail())
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
            
            _mainCAnimator.SetMoveSpeed(_moveSpeed);

            ApplyGravity();

            _cc.Move(_moveDir);

            MoveWhileAiming();
            
            _mainCAnimator.SetPistolMoveX(_moveAimingX);
            _mainCAnimator.SetPistolMoveY(_moveAimingY);

            if (_moveVectorKeyboard.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
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
            }
            else
            {
                if (_isRunningGamepad)
                {
                    _isRunningGamepad = false;
                }
            }


            if (_isUsingKeyboard) return;


            _movement = new Vector3(_moveVectorGamepad.x, 0.0f, _moveVectorGamepad.y).normalized;

            var moveSpeed = _isRunningGamepad ? runSpeed : walkSpeed;

            var desiredSpeed = _movement.magnitude * moveSpeed / 2 * 2.0f;
        

            //_cc.Move(_movement * Time.deltaTime);

            var actualSpeed = _mainCAnimator.GetMoveSpeed();
            var interpolatedSpeed = Mathf.Lerp(actualSpeed, desiredSpeed, Time.deltaTime * 8.0f);
            _mainCAnimator.SetMoveSpeed(interpolatedSpeed);

            var direction = new Vector3(_moveVectorGamepad.x, 0f, _moveVectorGamepad.y).normalized;
            var targetAngle = Mathf.Atan2(_movement.x, _movement.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityGamepad,
                turnSmoothTime);


            var moveDir = orientation.forward * (Time.deltaTime * interpolatedSpeed * direction.magnitude);

            _cc.Move(moveDir);

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
                _mainCAnimator.SetCrouch(IsCrouch);
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
            _mainCLayers.DisableAttackLayer();
        }


        private void CrouchWalking()
        {
            if (IsCrouch && (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f))
            {
                _isCrouchWalking = true;
                _mainCAnimator.SetCrouchWalking(_isCrouchWalking);
                
            }
            else
            {
                if (_isCrouchWalking)
                {
                    _isCrouchWalking = false;
                    _mainCAnimator.SetCrouchWalking(_isCrouchWalking);
                }
            }
        }

        public Vector3 PositionOnFloorNotGrounded()
        {
            var ray = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
            {
                return hitInfo.point;
            }
            else
            {
                return ray.GetPoint(2f);
            }
        }

        private void MoveWhileAiming()
        {
            if(_moveVectorKeyboard.x > 0.1f)
            {
                isMoveWhileAimingAPressed = true;
                _moveAimingX += aimSpeed * Time.deltaTime;
                if(_moveAimingX > 1)
                {
                    _moveAimingX = 1;
                }
            }
            else
            {
                if (isMoveWhileAimingAPressed == true)
                {
                    _moveAimingX -= aimSpeed * Time.deltaTime;
                    if (_moveAimingX <= 0)
                    {
                        _moveAimingX = 0;
                        isMoveWhileAimingAPressed = false;
                    }
                }
            }

            if (_moveVectorKeyboard.x < -0.1f)
            {
                isMoveWhileAimingDPressed = true;
                _moveAimingX -= aimSpeed * Time.deltaTime;
                if (_moveAimingX < -1)
                {
                    _moveAimingX = -1;
                }
            }
            else
            {
                if (isMoveWhileAimingDPressed == true)
                {
                    _moveAimingX += aimSpeed * Time.deltaTime;
                    if (_moveAimingX >= 0)
                    {
                        _moveAimingX = 0;
                        isMoveWhileAimingDPressed = false;
                    }
                }
            }

            if (_moveVectorKeyboard.y > 0.1f)
            {
                isMoveWhileAimingWPressed = true;
                _moveAimingY += aimSpeed * Time.deltaTime;
                if (_moveAimingY > 1)
                {
                    _moveAimingY = 1;
                }
            }
            else
            {
                if (isMoveWhileAimingWPressed == true)
                {
                    _moveAimingY -= aimSpeed * Time.deltaTime;
                    if (_moveAimingY <= 0)
                    {
                        _moveAimingY = 0;
                        isMoveWhileAimingWPressed = false;
                    }
                }
            }

            if (_moveVectorKeyboard.y < -0.1f)
            {
                isMoveWhileAimingSPressed = true;
                _moveAimingY -= aimSpeed * Time.deltaTime;
                if (_moveAimingY < -1)
                {
                    _moveAimingY = -1;
                }
            }
            else
            {
                if (isMoveWhileAimingSPressed == true)
                {
                    _moveAimingY += aimSpeed * Time.deltaTime;
                    if (_moveAimingY >= 0)
                    {
                        _moveAimingY = 0;
                        isMoveWhileAimingSPressed = false;
                    }
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