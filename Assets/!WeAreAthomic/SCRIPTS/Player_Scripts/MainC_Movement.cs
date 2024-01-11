using _WeAreAthomic.SCRIPTS.Debug_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCMovement : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private MainCAttack _mainCAttack;
        private MainCPistol _mainCPistol;
        private MainCRailGrindSystem _mainCRail;
        private MainCHackingSystem _mainCHacking;
        private MainCAnimatorController _mainCAnimator;
        private CharacterController _cc;
        private Godmode _godMode;
        private MainCHealthManager _mainCHealth;
        private Rigidbody _rb;
        private GTrajectory _trajectory;
        private MainCSounds _mainCSounds;
        private MainCDash _mainCDash;

        private Scene _currentScene;

        public AnimationCurve dashSpeedCurve;

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
        private Vector3[] puntosTrayectoria;

        [System.NonSerialized] public bool IsCrouch;
        [System.NonSerialized] public bool IsJumping;
        [System.NonSerialized] public bool IsFalling;
        [System.NonSerialized] public bool IsFollowingTrajectory;
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
        private bool _isPushOnAir;

        private int indexPoint = 2;

        [System.NonSerialized] public float _turnSmoothVelocityKeyboard;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float timeNextCrouch = 0.5f;
        [SerializeField] private float timeNextJump = 0.5f;
        [SerializeField] private float jumpImpulse = 5f;
        [SerializeField] private float jumpImpulseOnRail = 5f;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float aimSpeed;
        [SerializeField] private float pushJumpForce = 10f;
        [SerializeField] private float pushMaxHeight = 5f;
        [SerializeField] private float pushJumpDuration = 2f;
        public float turnSmoothTime = 0.1f;
        private float _moveSpeed;
        private float _turnSmoothVelocityGamepad;
        private float _timeGraceCrouchPeriod;
        private float _timeGraceJumpPeriod;
        private float _moveAimingX;
        private float _moveAimingY;
        private float _currentWalkSpeed;


        private void Awake()
        {
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCPistol = GetComponent<MainCPistol>();
            _cc = GetComponent<CharacterController>();
            _godMode = GetComponent<Godmode>();
            _mainCRail = GetComponent<MainCRailGrindSystem>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _playerInputActions = new PlayerInputActions();
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCDash = GetComponent<MainCDash>();

            _currentScene = SceneManager.GetActiveScene();
            if (_currentScene.name == "S2_LABTUTORIAL" || _currentScene.name == "TESTING")
            {
                _trajectory = GetComponent<GTrajectory>();
            }
        }

        private void Start()
        {
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");

            _canMove = true;
            _currentWalkSpeed = walkSpeed;
            GameManagerSingleton.Instance.PauseGame(false);
            GameManagerSingleton.Instance.SetIsSettingsMenuEnabled(false);
            GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
        }

        private void OnEnable()
        {
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Running.performed += RunOn;
            _playerInputActions.PlayerPC.Running.canceled += RunOff;
            _playerInputActions.PlayerPC.Crouch.performed += StartEndCrouch;
            _playerInputActions.PlayerPC.Jump.performed += StartJumpPC;
            _playerInputActions.PlayerGamepad.Running.performed += RunOn;
            _playerInputActions.PlayerGamepad.Running.canceled += RunOff;
            _playerInputActions.PlayerGamepad.Crouch.performed += StartEndCrouch;
            _playerInputActions.PlayerGamepad.Jump.performed += StartJumpGamepad;
        }

        private void OnDisable()
        {
            _playerInputActions.Disable();
            _playerInputActions.PlayerPC.Running.performed -= RunOn;
            _playerInputActions.PlayerPC.Running.canceled -= RunOff;
            _playerInputActions.PlayerPC.Crouch.performed -= StartEndCrouch;
            _playerInputActions.PlayerPC.Jump.performed -= StartJumpPC;
            _playerInputActions.PlayerGamepad.Running.performed -= RunOn;
            _playerInputActions.PlayerGamepad.Running.canceled -= RunOff;
            _playerInputActions.PlayerGamepad.Crouch.performed -= StartEndCrouch;
            _playerInputActions.PlayerGamepad.Jump.performed -= StartJumpGamepad;
        }

        private void Update()
        {
            AnimatorController();


            CrouchWalking();
            ApplyGravity();
            FollowTrajectory();
        }

        private void FixedUpdate()
        {
            if (CanMove())
            {
                if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
                {
                    MoveKeyboard();
                }
                else
                    MoveGamepad();
            }
        }


        private void AnimatorController()
        {
            if (!GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                if (!IsGrounded() && _velocity.y < 0 && !_mainCRail.IsOnRail() || !_mainCRail.IsOnRail() && _velocity.y < 0 && !IsGrounded())
                {
                    IsFalling = true;
                    IsJumping = false;
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(IsJumping);
                }

                if (IsFalling && IsGrounded() || IsFalling && _mainCRail.IsOnRail())
                {
                    IsFalling = false;
                    IsJumping = false;
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(IsJumping);
                    _mainCAnimator.SetGrounded(true);
                    _mainCLayers.DisableJumpLayer();
                    _timeGraceJumpPeriod = Time.time + timeNextJump;
                }
            }
        }

        private void ApplyGravity()
        {
            if (IsJumping || !IsGrounded() && !GameManagerSingleton.Instance.IsGodModeEnabled && !_mainCRail.IsOnRail() && !IsFollowingTrajectory)
            {
                _velocity += transform.up.normalized * (gravity * Time.deltaTime);
                _velocity.z = 0f;
                _cc.Move(_velocity * Time.deltaTime);
            }

            if(IsJumping && IsGrounded() || IsFalling && IsGrounded())
            {
                IsJumping = false;
                _mainCAnimator.SetGrounded(true);
            }
        }

        private void MoveKeyboard()
        {
            _moveVectorKeyboard = _playerInputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();

            var direction = CalculateMovementDirection(_moveVectorKeyboard);

            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityKeyboard, turnSmoothTime);

            _moveDir = orientation.forward * (Time.deltaTime * _moveSpeed * direction.magnitude);

            _mainCAnimator.SetMoveSpeed(_moveSpeed);

            ApplyGravity();

            _cc.Move(_moveDir);

            if (_moveVectorKeyboard.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }




            //arreglar cuando mantienes la W se suma y resta a la vez el moveSpeed
            if (_moveVectorKeyboard.magnitude > 0.1f && !_isRunningKeyboard)
            {
                if (_moveSpeed < _currentWalkSpeed)
                {
                    _moveSpeed += Time.deltaTime * 24;

                    if ((Mathf.Abs(_moveSpeed - _currentWalkSpeed)) < 0.3f)
                    {
                        _moveSpeed = _currentWalkSpeed;
                    }
                }

                if (_moveSpeed > _currentWalkSpeed)
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
            _moveVectorGamepad = _playerInputActions.PlayerGamepad.MovementGamepad.ReadValue<Vector2>();

            _movement = new Vector3(_moveVectorGamepad.x, 0.0f, _moveVectorGamepad.y).normalized;

            var moveSpeed = _isRunningGamepad ? runSpeed : _currentWalkSpeed;

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

        private Vector3 CalculateMovementDirection(Vector2 inputDirection)
        {
            var direction = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

            // Detectar la inclinación de la superficie usando Raycast
            RaycastHit hit;
            if (Physics.Raycast(checkGrounded.position, Vector3.down, out hit, .5f))
            {
                // Calcular la dirección de la rampa proyectando la normal en el plano horizontal
                Vector3 rampRightDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

                Vector3 rampForwardDir = Vector3.Cross(rampRightDirection, Vector3.up);

                //direction = rampForwardDir * 
            }
            return direction;
        }

        public void SetFollowTrajectory(bool condition)
        {
            IsFollowingTrajectory = condition;
            if (IsFollowingTrajectory)
            {
                indexPoint = 2;
                DisableMovement();
                puntosTrayectoria = _trajectory.CalcularPuntosTrayectoria();
            }
        }

        private void FollowTrajectory()
        {

            if (IsFollowingTrajectory)
            {
                var difference = puntosTrayectoria[indexPoint] - transform.position;
                transform.position = Vector3.MoveTowards(transform.position, puntosTrayectoria[indexPoint], 20f * Time.deltaTime);
                if (Vector3.Distance(transform.position, puntosTrayectoria[indexPoint]) < 0.1f)
                {
                    indexPoint++;
                }

                if (IsGrounded())
                {
                    EnableMovement();
                    _cc.enabled = true;
                    Debug.Log("hola1");
                    IsFollowingTrajectory = false;
                }
            }
        } 


        private void RunOn(InputAction.CallbackContext context)
        {
            if (!IsCrouch)
            {
                _isRunningKeyboard = true;

                _isRunningGamepad = !_isRunningGamepad;
            }
        }

        private void RunOff(InputAction.CallbackContext context)
        {
            _isRunningKeyboard = false;
        }

        private void StartEndCrouch(InputAction.CallbackContext context)
        {

            if (Time.time > _timeGraceCrouchPeriod)
            {
                IsCrouch = !IsCrouch;
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
                _currentWalkSpeed = crouchSpeed;

            }
            else
            {
                if (_isCrouchWalking)
                {
                    _isCrouchWalking = false;
                    _mainCAnimator.SetCrouchWalking(_isCrouchWalking);
                    _currentWalkSpeed = walkSpeed;
                }
            }
        }

        private void StartJumpPC(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                Jump();
            }
        }

        private void StartJumpGamepad(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {
                Jump();
            }
        }

        public void Jump()
        {
            if (CanJump())
            {
                IsJumping = true;
                _mainCLayers.EnableJumpLayer();
                _mainCAnimator.SetJumping(true);
                _velocity.y = jumpImpulse;
                _timeGraceJumpPeriod = Time.time + timeNextJump;
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

        public void EnableMovement()
        {
            _canMove = true;
        }

        public void DisableMovement()
        {
            _canMove = false;
        }

        private void OnDrawGizmos()
        {
            if (IsFollowingTrajectory)
            {
                foreach (var t in puntosTrayectoria)
                {
                    Gizmos.DrawWireSphere(t, 0.1f);
                }
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkGrounded.position, .1f);
        }



        private bool CanJump()
        {
            if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                return false;
            }

            if (!IsGrounded())
            {
                return false;
            }

            if(IsJumping)
            {
                return false;
            }

            if(Time.time < _timeGraceJumpPeriod)
            {
                return false;
            }

            if (_mainCRail.IsOnRail())
            {
                return false;
            }

            return true;
        }

        private bool CanMove()
        {
            if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                return false;
            }

            if(GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                return false;
            }

            if(_mainCHacking.isHackingAnim)
            {
                return false;
            }

            if(_mainCAttack.IsFinalAttacking)
            {
                return false;
            }

            if(_mainCHealth.IsDeath)
            {
                return false;
            }

            if(_mainCRail.IsSliding)
            {
                return false;
            }

            if(!_canMove)
            {
                return false;
            }

            if(_mainCDash.IsDashing)
            {
                return false;
            }

            if (_mainCRail.IsOnRail())
            {
                return false;
            }

            return true;
        }


        public bool IsGrounded()
        {
            return Physics.CheckSphere(checkGrounded.position, .1f, groundLayer);
        }
    }
}