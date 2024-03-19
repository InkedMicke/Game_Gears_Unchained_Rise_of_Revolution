using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Generics;

namespace Player
{
    public class MainCMovement : MonoBehaviour
    {

        #region Variables
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private MainCAttack _mainCAttack;
        private MainCPistol _mainCPistol;
        private MainCRail _mainCRail;
        private MainCHackingSystem _mainCHacking;
        private MainCAnimatorController _mainCAnimator;
        private CharacterController m_cc;
        private MainCGodmode _godMode;
        private MainCHealthManager _mainCHealth;
        [NonSerialized] public GTrajectory Trajectory;
        private MainCSounds _mainCSounds;
        private MainCDash m_mainCDash;
        private MainCVFX m_mainCVFX;
        private MainCJump m_mainCJump;
        private MainCCrouch m_mainCCrouch;
        private MainCInput m_input;

        public AnimationCurve dashSpeedCurve;

        private RaycastHit _slopeHit;

        [SerializeField] private LayerMask playerBulletLayer;

        [SerializeField] private GameObject cameraBaseObj;
        [SerializeField] private GameObject _cameraObj;

        public Transform orientation;
        [SerializeField] private Transform groundCheck;

        public LayerMask groundLayer;
        public Vector3 Velocity => m_velocity;
        Vector3 m_velocity;
        private Vector3 m_moveDir;
        private Vector3[] puntosTrayectoria;
        private Vector3 _localPosGroundCheckOriginal;

        [NonSerialized] public bool IsFalling;
        [NonSerialized] public bool IsFollowingTrajectory;
        [NonSerialized] public bool IsRunning;
        private bool m_canApplyGravity = true;
        private bool _canMove;

        private int indexPoint = 2;

        public float CurrentSpeed;
        public float walkSpeed;
        public float runSpeed;
        [SerializeField] private float gravityMultiplier = -3f;
        [SerializeField] private float aimSpeed;
        [SerializeField] private float maxSlopeAngle = 40f;
        [SerializeField] private float rotSpeed = 10f;
        public float turnSmoothTime = 0.1f;
        float m_currentSpeedLerp;

        #endregion

        #region Awake
        private void Awake()
        {
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCPistol = GetComponent<MainCPistol>();
            m_cc = GetComponent<CharacterController>();
            _godMode = GetComponent<MainCGodmode>();
            _mainCRail = GetComponent<MainCRail>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _playerInputActions = new PlayerInputActions();
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();
            m_input = GetComponent<MainCInput>();
            _mainCSounds = GetComponent<MainCSounds>();
            m_mainCDash = GetComponent<MainCDash>();
            m_mainCVFX = GetComponent<MainCVFX>();
            Trajectory = GetComponent<GTrajectory>();
            m_mainCJump = GetComponent<MainCJump>();
            m_mainCCrouch = GetComponent<MainCCrouch>();
        }
        #endregion

        #region EnableAndDisable
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
        #endregion

        #region Start
        private void Start()
        {
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
        #endregion

        #region Update
        private void Update()
        {
            AnimatorController();
            ApplyGravity();
            //FollowTrajectory();
        }
        #endregion

        #region FixedUpdate
        private void FixedUpdate()
        {
            if (CanMove())
            {
                Move();
                RotateInputDir();
            }


        }
        #endregion

        #region Animation
        private void AnimatorController()
        {
            if (!GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                if (!IsGrounded() && m_velocity.y < 0.0f && !_mainCRail.IsOnRail())
                {
                    IsFalling = true;
                    m_mainCJump.SetIsJumping(false);
                    _mainCAnimator.SetGrounded(false);
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(m_mainCJump.IsJumping);
                }

                if (IsFalling && IsGrounded() || IsFalling && IsOnSlope())
                {
                    IsFalling = false;
                    m_mainCJump.SetIsJumping(false);
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(m_mainCJump.IsJumping);
                    _mainCAnimator.SetGrounded(true);
                    m_mainCJump.TimeGraceJumpPeriod = Time.time + m_mainCJump.TimeNextJump;
                }
                
                if(m_input.GetInput().magnitude > .1f)
                {
                    if(IsRunning)
                    {
                        if (m_currentSpeedLerp >= runSpeed) return;
                        m_currentSpeedLerp += 15f * Time.deltaTime;
                    }
                    else
                    {
                        if(m_currentSpeedLerp > walkSpeed)
                        {
                            if (m_currentSpeedLerp <= walkSpeed) return;
                            m_currentSpeedLerp -= 15f * Time.deltaTime;
                        }

                        if(m_currentSpeedLerp <= walkSpeed)
                        {
                            m_currentSpeedLerp += 15f * Time.deltaTime;
                        }
                    }
                }
                else
                {
                    m_currentSpeedLerp -= 10f * Time.deltaTime;
                    m_currentSpeedLerp = Mathf.Clamp(m_currentSpeedLerp, 0, runSpeed);
                }

                _mainCAnimator.SetMoveSpeed(m_currentSpeedLerp);
            }
        }
        #endregion

        #region Movement

        void Move()
        {
            CurrentSpeed = IsRunning ? runSpeed : walkSpeed;
            m_moveDir = GetMoveDir();
            if (_mainCRail.IsSliding)
            {
                m_moveDir.x = 0;
                m_moveDir.z = 0;
            }
            if(IsOnSlope())
            {
                m_moveDir = GetSlopeMoveDirection();
            }
            m_cc.Move(CurrentSpeed * Time.deltaTime * m_moveDir);
        }
        #endregion

        #region Rotation
        void RotateInputDir()
        {
            var mouseVector = m_input.GetInput();
            if (mouseVector.magnitude > 0)
            {
                var desiredRot = Quaternion.LookRotation(m_moveDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region Gravity
        private void ApplyGravity()
        {
            if (!m_canApplyGravity) return;
            if (!IsGrounded() || m_mainCJump.IsJumping)
            {
                m_velocity.y -= Physics.gravity.y * Time.deltaTime * gravityMultiplier;
                m_cc.Move(m_velocity * Time.deltaTime);
            }
            else
            {
                m_velocity.y = 0;
            }

            if (IsFalling && _mainCRail.IsOnRail())
            {
                m_mainCVFX.SetActiveSparks(true);
            }


        }
        #endregion

        #region Run
        private void RunToggle(InputAction.CallbackContext context)
        {
            if (m_mainCJump.IsJumping || IsFalling) return;

            if (m_mainCCrouch.IsCrouch)
            {
                if (m_mainCCrouch.CanStandUp())
                {
                    _mainCLayers.DisableCrouchLayer();
                    m_mainCCrouch.SetIsCrouch(false);
                    m_mainCCrouch.ToggleCCSize();
                    _mainCAnimator.SetCrouch(m_mainCCrouch.IsCrouch);

                    IsRunning = !IsRunning;
                }
            }
            else
            {

                IsRunning = !IsRunning;
            }

        }
        #endregion

        #region Trajectory
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
                    m_cc.enabled = true;
                    IsFollowingTrajectory = false;
                }
            }
        }
        #endregion

        #region EnableAndDisableVariables
        public void EnableMovement()
        {
            _canMove = true;
        }

        public void DisableMovement()
        {
            _canMove = false;
        }
        #endregion

        #region Set Values

        public void SetVelocity(Vector3 velocity)
        {
            m_velocity = velocity;
        }

        public void SetCanApplyGravity(bool condition)
        {
            m_canApplyGravity = condition;
        }
        #endregion

        #region Return Values
        public Vector3 GetMoveDir()
        {
            var mouseVector = m_input.GetInput();
            var moveDir = new Vector3(mouseVector.x, 0f, mouseVector.y).normalized;
            Vector3 cameraForward = cameraBaseObj.transform.forward;
            Vector3 cameraRight = cameraBaseObj.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            return moveDir = cameraForward * mouseVector.y + cameraRight * mouseVector.x;
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

        #endregion

        #region Bools
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

            /*            if (_mainCHealth.IsDeath())
                        {
                            return false;
                        }*/

            if (!_canMove)
            {
                return false;
            }

            if (m_mainCDash.IsDashing)
            {
                return false;
            }

            if (_mainCAttack.IsMovingToEnemy)
            {
                return false;
            }

            if (_mainCAttack.IsAttacking)
            {
                return false;
            }
                        
            if (_mainCRail.IsSliding)
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

        public bool IsOnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, .08f))
            {
                var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        #endregion

        #region Only Editor
#if UNITY_EDITOR
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
#endif
        #endregion

    }
}