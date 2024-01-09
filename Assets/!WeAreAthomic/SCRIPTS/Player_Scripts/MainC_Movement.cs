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
        private MainCRailGrindSystem _railGrindSystem;
        private MainCHackingSystem _mainCHacking;
        private MainCAnimatorController _mainCAnimator;
        private CharacterController _cc;
        private Godmode _godMode;
        private MainCHealthManager _mainCHealth;
        private Rigidbody _rb;
        private GTrajectory _trajectory;
        private G_MeshTrail _gTrail;
        private MainCSounds _mainCSounds;

        private Scene _currentScene;

        [SerializeField] private AnimationCurve dashSpeedCurve;

        [SerializeField] private LayerMask playerBulletLayer;

        [SerializeField] private GameObject cameraBaseObj;
        private GameObject _cameraObj;

        public Transform orientation;
        public Transform checkGrounded;

        public LayerMask groundLayer;

        private Vector2 _moveVectorKeyboard;
        private Vector2 _moveVectorGamepad;
        private Vector2 _lastKeyPressed;

        private Vector3 _moveDir;
        private Vector3 _movement;
        private Vector3 _velocity;
        private Vector3[] puntosTrayectoria;

        [System.NonSerialized] public bool IsCrouch;
        [System.NonSerialized] public bool IsJumping;
        [System.NonSerialized] public bool IsFalling;
        [System.NonSerialized] public bool IsDashing;
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
        private bool _isFollowingTrajectory;
        private bool _isFirstKeyPressed;

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
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private float _dashTime = .10f;
        [SerializeField] private float timeToDash = .4f;
        [SerializeField] private float dashPcTimeThreshold = 0.5f;
        public float turnSmoothTime = 0.1f;
        private float _moveSpeed;
        private float _turnSmoothVelocityGamepad;
        private float _timeGraceCrouchPeriod;
        private float _timeGraceJumpPeriod;
        private float _moveAimingX;
        private float _moveAimingY;
        private float _dashTotalCooldown;
        private float _totalTimeToDash;

        private void Awake()
        {
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCPistol = GetComponent<MainCPistol>();
            _cc = GetComponent<CharacterController>();
            _godMode = GetComponent<Godmode>();
            _railGrindSystem = GetComponent<MainCRailGrindSystem>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _playerInputActions = new PlayerInputActions();
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();
            _gTrail = GetComponent<G_MeshTrail>();
            _mainCSounds = GetComponent<MainCSounds>();
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
            _playerInputActions.PlayerGamepad.Dash.performed += StartDashGamepad;
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
            _playerInputActions.PlayerGamepad.Dash.performed -= StartDashGamepad;
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
            if (_canMove && !GameManagerSingleton.Instance.IsGodModeEnabled && !_railGrindSystem.CanJumpOnRail && !_mainCHacking.isHackingAnim && !_mainCAttack.IsFinalAttacking && !_mainCHealth.IsDeath && !_railGrindSystem.IsSliding)
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
                if (!IsGrounded() && _velocity.y < 0 && !_railGrindSystem.IsOnRail() || !_railGrindSystem.IsOnRail() && _velocity.y < 0 && !IsGrounded())
                {
                    IsFalling = true;
                    IsJumping = false;
                    _mainCAnimator.SetFalling(IsFalling);
                    _mainCAnimator.SetJumping(IsJumping);
                }

                if (IsFalling && IsGrounded() || IsFalling && _railGrindSystem.IsOnRail())
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
        }

        private void ApplyGravity()
        {
            if (IsJumping || !IsGrounded() && !GameManagerSingleton.Instance.IsGodModeEnabled && !_railGrindSystem.IsOnRail() && !_isFollowingTrajectory)
            {
                _velocity += transform.up.normalized * (gravity * Time.deltaTime);
                _velocity.z = 0f;
                _cc.Move(_velocity * Time.deltaTime);
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


            MoveWhileAiming();


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
            _moveVectorGamepad = _playerInputActions.PlayerGamepad.MovementGamepad.ReadValue<Vector2>();

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

        private Vector3 CalculateMovementDirection(Vector2 inputDirection)
        {
            var direction = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

            // Detectar la inclinación de la superficie usando Raycast
            RaycastHit hit;
            if (Physics.Raycast(checkGrounded.position, Vector3.down, out hit, .5f))
            {
                // Calcular la dirección de la rampa proyectando la normal en el plano horizontal
                Vector3 rampDirection = Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;

                var verticalSlopeAngle = Vector3.Angle(rampDirection, Vector3.up);

                // Modificar la dirección de movimiento según el ángulo de la rampa
                if (verticalSlopeAngle > 10)
                {
                    Debug.DrawRay(hit.point, direction.normalized * 100f, Color.magenta, 2f);
                    direction = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                }
            }
            return direction;
        }

        public void SetFollowTrajectory(bool condition)
        {
            _isFollowingTrajectory = condition;
            if (_isFollowingTrajectory)
            {
                indexPoint = 2;
                DisableMovement();
                puntosTrayectoria = _trajectory.CalcularPuntosTrayectoria();
            }
        }

        private void FollowTrajectory()
        {

            if (_isFollowingTrajectory)
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
                    _isFollowingTrajectory = false;
                    Debug.Log(_canMove);
                }
            }
        }

        private void StartDashPC(InputAction.CallbackContext context)
        {
            if (CanDash() && GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                var x = _playerInputActions.PlayerPC.Dash.ReadValue<Vector2>();

                if (x.magnitude == 1 && !_isFirstKeyPressed)
                {
                    _totalTimeToDash = Time.time + timeToDash;
                    _lastKeyPressed = x;
                    _isFirstKeyPressed = true;
                }
                if (Time.time < _totalTimeToDash)
                {
                    if (_isFirstKeyPressed)
                    {
                        if (x == _lastKeyPressed)
                        {
                            _totalTimeToDash = Time.time + dashCooldown;
                            StartCoroutine(Dash());
                            _isFirstKeyPressed = false;
                        }
                        _lastKeyPressed = Vector2.zero;
                    }
                }
            }
        }

        private void StartDashGamepad(InputAction.CallbackContext context)
        {
            if (CanDash() && GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {
                StartCoroutine(Dash());
            }
        }

        public void StopDash()
        {
            StopCoroutine(Dash());
            _mainCLayers.DisableSlideLayer();
            IsDashing = false;
        }

        private IEnumerator Dash()
        {
            IsDashing = true;
            _mainCSounds.PlayJumpSound();
            _mainCLayers.EnableSlideLayer();
            _mainCAnimator.TriggerDash();
            DisableMovement();
            _dashTotalCooldown = Time.time + dashCooldown;
            yield return new WaitForSeconds(0.1f);
            var startTime = Time.time;
            while (Time.time < startTime + _dashTime)
            {
                _gTrail.StartTrail();

                float curveTime = (Time.time - startTime) / _dashTime;
                float speedMultiplier = dashSpeedCurve.Evaluate(curveTime);

                _cc.Move(orientation.forward * dashSpeed * speedMultiplier * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            EnableMovement();
        }
        private void EndDash()
        {
            _mainCLayers.DisableSlideLayer();
            IsDashing = false;
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
            if (CanJump())
            {
                IsJumping = true;
                _velocity.y += Mathf.Sqrt(jumpImpulse * -3.0f * -9.8f); ;
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
            if (_moveVectorKeyboard.x > 0.1f)
            {
                isMoveWhileAimingAPressed = true;
                _moveAimingX += aimSpeed * Time.deltaTime;
                if (_moveAimingX > 1)
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
            if (_isFollowingTrajectory)
            {
                foreach (var t in puntosTrayectoria)
                {
                    Gizmos.DrawWireSphere(t, 0.1f);
                }
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkGrounded.position, .1f);
        }

        private bool CanDash()
        {
            if (Time.time < _dashTotalCooldown)
            {
                return false;
            }

            if (_isFollowingTrajectory)
            {
                return false;
            }

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

            if (Time.time < _dashTotalCooldown)
            {
                return false;
            }

            return true;
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

            return true;
        }


        public bool IsGrounded()
        {
            return Physics.CheckSphere(checkGrounded.position, .1f, groundLayer);
        }
    }
}