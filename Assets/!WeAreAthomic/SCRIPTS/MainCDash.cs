using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCDash : MonoBehaviour
    {
        private CharacterController _cc;
        private PlayerInputActions _playerInputActions;
        private MainCMovement _mainCMove;
        private MainCLayers _mainCLayers;
        private MainCSounds _mainCSounds;
        private MainCAnimatorController _mainCAnim;
        private G_MeshTrail _gTrail;

        private Key _lastKey;

        private Vector3 _directionDash;

        [SerializeField] private Transform cameraTr;
        [SerializeField] private Transform cameraRotation;

        [System.NonSerialized] public bool IsDashing;

        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private float _dashTime = .10f;
        private float _dashTotalCooldown;
        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCMove = GetComponent<MainCMovement>();
            _mainCAnim = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _gTrail = GetComponent<G_MeshTrail>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Dash.performed += StartDashPC;
            _playerInputActions.PlayerGamepad.Dash.performed += StartDashGamepad;
        }

        

        private void StartDashPC(InputAction.CallbackContext context)
        {
            if (CanDash() && GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                StartCoroutine(Dash());
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
            _mainCLayers.DisableSlideLayer();
            StopCoroutine(Dash());
            IsDashing = false;
        }

        private IEnumerator Dash()
        {
            IsDashing = true;
            _mainCSounds.PlayJumpSound();
            _mainCLayers.EnableSlideLayer();
            _mainCAnim.TriggerDash();
            _mainCMove.DisableMovement();
            _dashTotalCooldown = Time.time + dashCooldown;
            yield return new WaitForSeconds(.1f);
            var startTime = Time.time;
            var _moveInput = _playerInputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();

            Vector3 cameraForward = cameraTr.forward;
            Vector3 cameraRight = cameraTr.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * _moveInput.y + cameraRight * _moveInput.x;

            _directionDash = new(moveDirection.x, transform.position.y, moveDirection.z);
            var desiredPos = cameraRotation.position + _directionDash.normalized * 2;
            desiredPos.y = transform.position.y;
 
            transform.LookAt(desiredPos);

            while (Time.time < startTime + _dashTime)
            {
                _gTrail.StartTrail();

                float curveTime = (Time.time - startTime) / _dashTime;
                float speedMultiplier = _mainCMove.dashSpeedCurve.Evaluate(curveTime);

                _cc.Move(dashSpeed * speedMultiplier * Time.deltaTime * moveDirection.normalized);
                yield return new WaitForEndOfFrame();
            }

            _mainCMove.EnableMovement();
        }

        private void EndDash()
        {                                                                                        
            _mainCLayers.DisableSlideLayer();
            IsDashing = false;
        }

        private bool CanDash()
        {
            if (Time.time < _dashTotalCooldown)
            {
                return false;
            }

            if (_mainCMove.IsFollowingTrajectory)
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

            if(IsDashing)
            {
                return false;
            }

            return true;
        }
    }
}
