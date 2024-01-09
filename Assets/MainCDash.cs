using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
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

        private Vector2 _lastKeyPressed;

        [System.NonSerialized] public bool IsDashing;
        private bool _isFirstKeyPressed;

        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private float _dashTime = .10f;
        [SerializeField] private float dashPcTimeThreshold = 0.5f;
        private float _dashTotalCooldown;
        private float _totalTimeToDash;
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
                var x = _playerInputActions.PlayerPC.Dash.ReadValue<Vector2>();

                if (!_isFirstKeyPressed && (x.x == 1f && x.y == 0f) || (x.x == 0f && x.y == 1f))
                {
                    _lastKeyPressed = x;
                    _isFirstKeyPressed = true;
                }

                if(_isFirstKeyPressed)
                {
                    if(x == _lastKeyPressed)
                    {
                        StartCoroutine(Dash());
                    }
                    _isFirstKeyPressed = false;
                    _lastKeyPressed = Vector2.zero;
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
            _mainCAnim.TriggerDash();
            _mainCMove.DisableMovement();
            _dashTotalCooldown = Time.time + dashCooldown;
            yield return new WaitForSeconds(0.1f);
            var startTime = Time.time;
            while (Time.time < startTime + _dashTime)
            {
                _gTrail.StartTrail();

                float curveTime = (Time.time - startTime) / _dashTime;
                float speedMultiplier = _mainCMove.dashSpeedCurve.Evaluate(curveTime);

                _cc.Move(transform.forward * dashSpeed * speedMultiplier * Time.deltaTime);
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

            return true;
        }
    }
}
