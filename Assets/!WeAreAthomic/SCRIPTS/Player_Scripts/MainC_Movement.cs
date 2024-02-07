using _WeAreAthomic.SCRIPTS.Debug_Scripts;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCMovement : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private MainCAttack _mainCAttack;
        private MainCPistol _mainCPistol;
        private MainCRail _mainCRail;
        private MainCHackingSystem _mainCHacking;
        private MainCAnimatorController _mainCAnimator;
        private CharacterController _cc;
        private MainCGodmode _godMode;
        private MainCHealthManager _mainCHealth;
        [NonSerialized] public GTrajectory Trajectory;
        private MainCSounds _mainCSounds;
        private MainCDash _mainCDash;
        private MainCVFX _mainCVFX;

        public AnimationCurve dashSpeedCurve;

        private RaycastHit _slopeHit;

        [SerializeField] private LayerMask playerBulletLayer;

        [SerializeField] private CapsuleCollider _hurtBoxCC;

        [SerializeField] private GameObject cameraBaseObj;
        private GameObject _cameraObj;

        public Transform orientation;
        [SerializeField] private Transform groundCheck;

        public LayerMask groundLayer;

        private Vector2 _moveVectorKeyboard;
        private Vector2 _moveVectorGamepad;

        private Vector3 _moveDir;
        private Vector3 _movement;
        private Vector3 _velocity;
        private Vector3[] puntosTrayectoria;
        private Vector3 _currentGroundNormal;
        private Vector3 _localPosGroundCheckOriginal;
        private Vector3 _localPosGroundCheckChanged;

        [NonSerialized] public bool IsCrouch;
        [NonSerialized] public bool IsJumping;
        [NonSerialized] public bool IsFalling;
        [NonSerialized] public bool IsFollowingTrajectory;
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
        private bool _gotGroundHitRay;

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
        [SerializeField] private float maxSlopeAngle = 40f;
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
            _godMode = GetComponent<MainCGodmode>();
            _mainCRail = GetComponent<MainCRail>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _playerInputActions = new PlayerInputActions();
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCDash = GetComponent<MainCDash>();
            _mainCVFX = GetComponent<MainCVFX>();
            Trajectory = GetComponent<GTrajectory>();
        }

        private void Start()
        {
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");

            _canMove = true;
            _currentWalkSpeed = walkSpeed;
            _localPosGroundCheckOriginal = groundCheck.localPosition;
            var desiredPos = _localPosGroundCheckOriginal;
            desiredPos.y -= 0.03f;
            _localPosGroundCheckChanged = desiredPos;
            GameManagerSingleton.Instance.PauseGame(false);
            GameManagerSingleton.Instance.SetIsSettingsMenuEnabled(false);
            GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
        }

        private void OnEnable()
        {
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Running.performed += RunToggle;
            _playerInputActions.PlayerPC.Crouch.performed += StartEndCrouch;
            _playerInputActions.PlayerPC.Jump.performed += StartJumpPC;
            _playerInputActions.PlayerGamepad.Running.performed += RunToggle;
            _playerInputActions.PlayerGamepad.Crouch.performed += StartEndCrouch;
            _playerInputActions.PlayerGamepad.Jump.performed += StartJumpGamepad;
        }

        private void OnDisable()
        {
            _playerInputActions.Disable();
            _playerInputActions.PlayerPC.Running.performed -= RunToggle;
            _playerInputActions.PlayerPC.Crouch.performed -= StartEndCrouch;
            _playerInputActions.PlayerPC.Jump.performed -= StartJumpPC;
            _playerInputActions.PlayerGamepad.Running.performed -= RunToggle;
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

                if (IsFalling && IsGrounded() || IsFalling && _mainCRail.IsOnRail() || IsFalling && IsOnSlope())
                {
                    IsFalling = false;
                    IsJumping = false;
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(IsJumping);
                    _mainCAnimator.SetGrounded(true);
                    if (_moveDir.magnitude > 0 || _moveDir.magnitude > 0)
                    {
                        _mainCLayers.DisableJumpLayer();
                    }
                    _timeGraceJumpPeriod = Time.time + timeNextJump;
                }
            }
        }

        private void ApplyGravity()
        {
            if (IsJumping || IsFalling || !IsGrounded() && !GameManagerSingleton.Instance.IsGodModeEnabled && !_mainCRail.IsSliding && !IsFollowingTrajectory)
            {
                _velocity += transform.up.normalized * (gravity * Time.deltaTime);
                _cc.Move(_velocity * Time.deltaTime);
            }

            if (IsJumping && IsGrounded() || IsFalling && IsGrounded() || IsFalling && _mainCRail.IsOnRail())
            {
                IsJumping = false;
                _mainCAnimator.SetGrounded(true);
            }

            if (IsFalling && _mainCRail.IsOnRail())
            {
                _mainCVFX.SetActiveSparks(true);
            }
        }

        private void MoveKeyboard()
        {
            _moveVectorKeyboard = _playerInputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();

            var direction = new Vector3(_moveVectorKeyboard.x, 0f, _moveVectorKeyboard.y).normalized;

            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityKeyboard, turnSmoothTime);

            _moveDir = orientation.forward * (Time.deltaTime * _moveSpeed * direction.magnitude);

            if (IsOnSlope())
            {
                _cc.Move(GetSlopeMoveDirection() * (Time.deltaTime * _moveSpeed));
            }
            else
            {
                _cc.Move(_moveDir);
            }
            _mainCAnimator.SetMoveSpeed(_moveSpeed);

            ApplyGravity();

            

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


            _moveDir = orientation.forward * (Time.deltaTime * interpolatedSpeed * direction.magnitude);

            _cc.Move(_moveDir);

            if (_moveVectorGamepad.magnitude > 0.1)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        }

        public void SetFollowTrajectory(bool condition)
        {
            IsFollowingTrajectory = condition;
            if (IsFollowingTrajectory)
            {
                indexPoint = 2;
                puntosTrayectoria = Trajectory.CalcularPuntosTrayectoria();
            }
        }

        private void FollowTrajectory()
        {

            if (IsFollowingTrajectory)
            {
                transform.position = Vector3.MoveTowards(transform.position, puntosTrayectoria[indexPoint], 20f * Time.deltaTime);
                if (Vector3.Distance(transform.position, puntosTrayectoria[indexPoint]) < 0.1f)
                {
                    indexPoint++;
                }

                if (IsGrounded())
                {
                    EnableMovement();
                    _cc.enabled = true;
                    IsFollowingTrajectory = false;
                }
            }
        }


        private void RunToggle(InputAction.CallbackContext context)
        {

            if (IsCrouch)
            {
                _mainCLayers.DisableCrouchLayer();
                IsCrouch = false;
                ToggleCCSize();
                _mainCAnimator.SetCrouch(IsCrouch);
            }

            _isRunningKeyboard = !_isRunningKeyboard;

            _isRunningGamepad = !_isRunningGamepad;

        }

        private void StartEndCrouch(InputAction.CallbackContext context)
        {

            if (Time.time > _timeGraceCrouchPeriod)
            {
                IsCrouch = !IsCrouch;
                ToggleCCSize();
                if (!IsCrouch && _isCrouchWalking)
                {
                    _mainCAnimator.SetCrouchWalking(false);
                    _isCrouchWalking = false;
                    _currentWalkSpeed = walkSpeed;
                    Invoke(nameof(InvokeDisableCrouchLayer), 0.1f);
                }

                if (_mainCRail.IsSliding)
                {
                    _mainCAnimator.SetSlidingCrouch(IsCrouch);
                }
                else
                {
                    _mainCAnimator.SetCrouch(IsCrouch);
                }

                _timeGraceCrouchPeriod = Time.time + timeNextCrouch;

                if (IsCrouch)
                {
                    InvokeDisableAllLayers();
                    _mainCLayers.EnableCrouchLayer();
                    if ((_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f))
                    {
                        _isCrouchWalking = true;
                        _mainCAnimator.SetCrouchWalking(_isCrouchWalking);
                    }
                }
                else
                {
                    Invoke(nameof(InvokeDisableCrouchLayer), 0.5f);
                }
            }
        }

        /// <summary>
        /// Cambia el tamaño de la capsula del Character Controller
        /// </summary>
        private void ToggleCCSize()
        {
            if (IsCrouch)
            {
                //Character Controller
                var crouchPosCc = _cc.center;
                crouchPosCc.y = .75f;
                _cc.center = crouchPosCc;
                _cc.height = 1.48f;
                //HurtBox
                var crouchPosCC = _hurtBoxCC.center;
                crouchPosCC.y = .73f;
                _hurtBoxCC.center = crouchPosCC;
                _hurtBoxCC.height = 1.43f;
            }
            else
            {
                //Character Controller
                var originalPosCc = _cc.center;
                originalPosCc.y = .87f;
                _cc.center = originalPosCc;
                _cc.height = 1.69f;
                //HurtBox
                var originalPosCC = _hurtBoxCC.center;
                originalPosCC.y = .91f;
                _hurtBoxCC.center = originalPosCC;
                _hurtBoxCC.height = 1.79f;
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
                if (_isRunningKeyboard || _isRunningGamepad)
                {
                    _isRunningKeyboard = !_isRunningKeyboard;
                    _isRunningGamepad = !_isRunningGamepad;
                }

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

        private void Jump()
        {
            if (CanJumpGround() && (IsGrounded() || IsOnSlope()))
            {
                _mainCSounds.PlayJumpSound();
                IsJumping = true;
                _mainCAnimator.SetGrounded(false);
                _mainCLayers.EnableJumpLayer();
                _mainCAnimator.SetJumping(true);
                _velocity = transform.up.normalized * jumpImpulse;
                _timeGraceJumpPeriod = Time.time + timeNextJump;
            }

            if (CanJumpRail())
            {
                if (IsCrouch)
                {
                    _mainCAnimator.SetSlidingCrouch(false);
                    IsCrouch = false;

                }
                _mainCSounds.PlayJumpSound();
                IsJumping = true;
                _mainCAnimator.SetGrounded(false);
                _mainCLayers.EnableJumpLayer();
                _mainCAnimator.SetJumping(true);
                _velocity = transform.up.normalized * (_mainCRail.HigherJumpDueToInclination() ? jumpImpulseOnRail * 2.6f : jumpImpulseOnRail);
                _timeGraceJumpPeriod = Time.time + timeNextJump;

                _mainCVFX.SetActiveSparks(false);
            }
        }

        public void EndJump()
        {
            if (!IsJumping)
            {
                _mainCLayers.DisableJumpLayer();
            }
        }

        public Vector3 PositionOnFloorNotGrounded()
        {
            var ray = new Ray(transform.position, -transform.up);
            return Physics.Raycast(ray, out var hitInfo, Mathf.Infinity) ? hitInfo.point : ray.GetPoint(2f);
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(_moveDir, _slopeHit.normal).normalized;
        }

        public void EnableMovement()
        {
            _canMove = true;
        }

        public void DisableMovement()
        {
            _canMove = false;
        }

        public void SetJumping(bool isJumping)
        {
            IsJumping = isJumping;
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
            Gizmos.DrawWireSphere(groundCheck.position, .1f);
        }

        private bool IsOnSlope()
        {
            if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, .08f))
            {
                var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        private bool CanJumpGround()
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

            if (IsJumping)
            {
                return false;
            }

            if (Time.time < _timeGraceJumpPeriod)
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

            if (GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                return false;
            }

            if (_mainCHacking.isHackingAnim)
            {
                return false;
            }

            if (_mainCAttack.IsFinalAttacking)
            {
                return false;
            }

            if (_mainCHealth.IsDeath)
            {
                return false;
            }

            if (_mainCRail.IsSliding)
            {
                return false;
            }

            if (!_canMove)
            {
                return false;
            }

            if (_mainCDash.IsDashing)
            {
                return false;
            }

            if (_mainCRail.IsOnRail())
            {
                return false;
            }

            return true;
        }

        private bool CanJumpRail()
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

            if (!_mainCRail.IsOnRail())
            {
                return false;
            }

            if (IsJumping)
            {
                return false;
            }

            if (Time.time < _timeGraceJumpPeriod)
            {
                return false;
            }

            return true;
        }

        public bool IsGrounded()
        {
            var ground = Physics.CheckSphere(groundCheck.position, .1f, groundLayer);
            if (ground)
            {
                return true;
            }
            else
            {
                var ray = new Ray(transform.position, Vector3.down);
                var gotHit = Physics.Raycast(ray, 0.05f, groundLayer);
                if(gotHit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}