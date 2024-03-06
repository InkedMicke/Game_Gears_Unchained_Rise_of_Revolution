using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MainCMovement : MonoBehaviour
    {
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

        private Vector3 m_moveDir;
        private Vector3 _movement;
        private Vector3[] puntosTrayectoria;
        private Vector3 _localPosGroundCheckOriginal;
        private Vector3 m_direction;

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
        [SerializeField] private float gravityMultiplier = -3f;

        [SerializeField] private float aimSpeed;
        [SerializeField] private float maxSlopeAngle = 40f;
        public float turnSmoothTime = 0.1f;
        private float _moveSpeed;
        private float _turnSmoothVelocityGamepad;
        [NonSerialized] public float Velocity;
        [NonSerialized] public float CurrentSpeed;


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
            CurrentSpeed = walkSpeed;
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
            _moveVectorKeyboard = _playerInputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();

            m_direction = new Vector3(_moveVectorKeyboard.x, 0f, _moveVectorKeyboard.y).normalized;
            Vector3 cameraForward = _cameraObj.transform.forward;
            Vector3 cameraRight = _cameraObj.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            m_moveDir = cameraForward * _moveVectorKeyboard.y + cameraRight * _moveVectorKeyboard.x;


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
                if (!IsGrounded() && Velocity < 0.0f && !_mainCRail.IsOnRail())
                {
                    IsFalling = true;
                    m_mainCJump.SetIsJumping(false);
                    _mainCAnimator.SetGrounded(false);
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(m_mainCJump.IsJumping);
                }

                if (IsFalling && _cc.isGrounded || IsFalling && IsOnSlope())
                {
                    IsFalling = false;
                    m_mainCJump.SetIsJumping(false);
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(m_mainCJump.IsJumping);
                    _mainCAnimator.SetGrounded(true);
                    m_mainCJump.TimeGraceJumpPeriod = Time.time + m_mainCJump.TimeNextJump;
                }
            }
        }

        private void ApplyGravity()
        {
            if (_cc.isGrounded && Velocity < 0.0f)
            {
                Velocity = -1.0f;
            }
            else
            {
                Velocity += -Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }

            m_moveDir.y = Velocity;

            if (m_moveDir.y == 0 && m_mainCJump.IsJumping && IsGrounded())
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
            var targetAngle = Mathf.Atan2(m_direction.x, m_direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityKeyboard, turnSmoothTime);

            CurrentSpeed = IsRunningKeyboard ? runSpeed : walkSpeed;

            if (IsOnSlope())
            {
                _cc.Move(CurrentSpeed * Time.deltaTime * m_moveDir);
            }
            else
            {
                _cc.Move(CurrentSpeed * Time.deltaTime * m_moveDir);
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
                if (_moveSpeed < CurrentSpeed)
                {
                    _moveSpeed += Time.deltaTime * 24;

                    if ((Mathf.Abs(_moveSpeed - CurrentSpeed)) < 0.3f)
                    {
                        _moveSpeed = CurrentSpeed;
                    }
                }

                if (_moveSpeed > CurrentSpeed)
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
                        _moveSpeed -= Time.deltaTime * 24;
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

            var moveSpeed = IsRunningGamepad ? runSpeed : CurrentSpeed;

            var desiredSpeed = _movement.magnitude * moveSpeed / 2 * 2.0f;


            //_cc.Move(_movement * Time.deltaTime);

            var actualSpeed = _mainCAnimator.GetMoveSpeed();
            var interpolatedSpeed = Mathf.Lerp(actualSpeed, desiredSpeed, Time.deltaTime * 8.0f);
            _mainCAnimator.SetMoveSpeed(interpolatedSpeed);

            var direction = new Vector3(_moveVectorGamepad.x, 0f, _moveVectorGamepad.y).normalized;
            var targetAngle = Mathf.Atan2(_movement.x, _movement.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocityGamepad,
                turnSmoothTime);

            Vector3 cameraForward = _cameraObj.transform.forward;
            Vector3 cameraRight = _cameraObj.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;


            if (IsOnSlope())
            {
                _cc.Move(GetSlopeMoveDirection() * (Time.deltaTime * interpolatedSpeed * CurrentSpeed));
            }
            else
            {
                _cc.Move(interpolatedSpeed * Time.deltaTime * m_moveDir);
            }

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
            return Vector3.ProjectOnPlane(m_moveDir, _slopeHit.normal).normalized;
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
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, .08f))
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

            if (GameManagerSingleton.Instance.IsOnDialogue)
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

            if (_mainCHealth.IsDeath())
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

            if(_mainCAttack.IsMovingToEnemy)
            {
                return false;
            }            
            
            if(_mainCAttack.IsAttacking)
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
                if (gotHit)
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