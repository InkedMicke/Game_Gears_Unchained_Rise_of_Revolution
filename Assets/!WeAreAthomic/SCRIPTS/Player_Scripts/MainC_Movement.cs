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
        private MainCDash m_mainCDash;
        private MainCVFX m_mainCVFX;
        private MainCJump m_mainCJump;
        private MainCCrouch m_mainCCrouch;

        public AnimationCurve dashSpeedCurve;

        private RaycastHit _slopeHit;

        [SerializeField] private LayerMask playerBulletLayer;

        [SerializeField] private GameObject cameraBaseObj;
        private GameObject _cameraObj;

        public Transform orientation;
        [SerializeField] private Transform groundCheck;

        public LayerMask groundLayer;

        private Vector2 _moveVectorKeyboard;
        private Vector2 _moveVectorGamepad;

        private Vector3 _moveDir;
        private Vector3 _movement;
        [NonSerialized] public Vector3 Velocity;
        private Vector3[] puntosTrayectoria;
        private Vector3 _localPosGroundCheckOriginal;

        [NonSerialized] public bool IsFalling;
        [NonSerialized] public bool IsFollowingTrajectory;
        [NonSerialized] public bool IsRunningKeyboard;
        [NonSerialized] public bool IsRunningGamepad;
        private bool canApplyGravity = true;
        private bool _canMove;

        private int indexPoint = 2;

        [NonSerialized] public float _turnSmoothVelocityKeyboard;
        public float walkSpeed;
        public float runSpeed;
        [SerializeField] private float gravity = -9.8f;

        [SerializeField] private float aimSpeed;
        [SerializeField] private float maxSlopeAngle = 40f;
        public float turnSmoothTime = 0.1f;
        private float _moveSpeed;
        private float _turnSmoothVelocityGamepad;
        [NonSerialized] public float CurrentWalkSpeed;
        

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
            m_mainCDash = GetComponent<MainCDash>();
            m_mainCVFX = GetComponent<MainCVFX>();
            Trajectory = GetComponent<GTrajectory>();
            m_mainCJump = GetComponent<MainCJump>();
            m_mainCCrouch = GetComponent<MainCCrouch>();
        }

        private void Start()
        {
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");

            _canMove = true;
            CurrentWalkSpeed = walkSpeed;
            _localPosGroundCheckOriginal = groundCheck.localPosition;
            var desiredPos = _localPosGroundCheckOriginal;
            desiredPos.y -= 0.03f;
            GameManagerSingleton.Instance.PauseGame(false);
            GameManagerSingleton.Instance.SetIsSettingsMenuEnabled(false);
            GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
            GameManagerSingleton.Instance.SetIsOnDialogue(false);
        }

        private void OnEnable()
        {
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Running.performed += RunToggle;
            _playerInputActions.PlayerGamepad.Running.performed += RunToggle;

        }

        private void OnDisable()
        {
            _playerInputActions.Disable();
            _playerInputActions.PlayerPC.Running.performed -= RunToggle;
            _playerInputActions.PlayerGamepad.Running.performed -= RunToggle;
        }

        private void Update()
        {
            AnimatorController();
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
                if (!IsGrounded() && Velocity.y < 0 && !_mainCRail.IsOnRail() || !_mainCRail.IsOnRail() && Velocity.y < 0 && !IsGrounded())
                {
                    IsFalling = true;
                    m_mainCJump.SetIsJumping(false);
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(m_mainCJump.IsJumping);
                }

                if (IsFalling && IsGrounded() || IsFalling && _mainCRail.IsOnRail() || IsFalling && IsOnSlope())
                {
                    IsFalling = false;
                    m_mainCJump.SetIsJumping(false);
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(m_mainCJump.IsJumping);
                    _mainCAnimator.SetGrounded(true);
                    if (_moveDir.magnitude > 0 || _moveDir.magnitude > 0)
                    {
                        _mainCLayers.DisableJumpLayer();
                    }
                    m_mainCJump.TimeGraceJumpPeriod = Time.time + m_mainCJump.TimeNextJump;
                }
            }
        }

        private void ApplyGravity()
        {
            if (m_mainCJump.IsJumping || IsFalling || !_cc.isGrounded && !GameManagerSingleton.Instance.IsGodModeEnabled && !_mainCRail.IsSliding && !IsFollowingTrajectory)
            {
                if(canApplyGravity)
                {
                    Velocity += Vector3.up.normalized * (gravity * Time.deltaTime);
                    _cc.Move(Velocity * Time.deltaTime);
                }
            
            }

            if (m_mainCJump.IsJumping && _cc.isGrounded || IsFalling && _cc.isGrounded || IsFalling && _mainCRail.IsOnRail())
            {
                m_mainCJump.SetIsJumping(false);
                _mainCAnimator.SetGrounded(true);
            }

            if (IsFalling && _mainCRail.IsOnRail())
            {
                m_mainCVFX.SetActiveSparks(true);
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
            if (_moveVectorKeyboard.magnitude > 0.1f && !IsRunningKeyboard)
            {
                if (_moveSpeed < CurrentWalkSpeed)
                {
                    _moveSpeed += Time.deltaTime * 24;

                    if ((Mathf.Abs(_moveSpeed - CurrentWalkSpeed)) < 0.3f)
                    {
                        _moveSpeed = CurrentWalkSpeed;
                    }
                }

                if (_moveSpeed > CurrentWalkSpeed)
                {
                    _moveSpeed -= Time.deltaTime * 18;
                }
            }
            else
            {
                if (!IsRunningKeyboard || IsRunningKeyboard && _moveVectorKeyboard.magnitude < 0.1f)
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

            if (!(_moveVectorKeyboard.magnitude > 0.1 && IsRunningKeyboard)) return;
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

            var moveSpeed = IsRunningGamepad ? runSpeed : CurrentWalkSpeed;

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

            if (m_mainCCrouch.IsCrouch)
            {
                if (m_mainCCrouch.CanStandUp())
                {
                    _mainCLayers.DisableCrouchLayer();
                    m_mainCCrouch.SetIsCrouch(false);
                    m_mainCCrouch.ToggleCCSize();
                    _mainCAnimator.SetCrouch(m_mainCCrouch.IsCrouch);


                    IsRunningKeyboard = !IsRunningKeyboard;

                    IsRunningGamepad = !IsRunningGamepad;
                }
            }
            else
            {

                IsRunningKeyboard = !IsRunningKeyboard;

                IsRunningGamepad = !IsRunningGamepad;
            }

        }

        public void InvokeDisableAllLayers()
        {
            _mainCLayers.DisableCrouchLayer();
            _mainCLayers.DisableAttackLayer();
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

        public bool IsOnSlope()
        {
            if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, .08f))
            {
                var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        public void SetCanApplyGravity(bool condition)
        {
            canApplyGravity = condition;
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

            if(GameManagerSingleton.Instance.IsOnDialogue)
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

            if (m_mainCDash.IsDashing)
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