using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private Vector2 _lastKeyPressed;
        private Vector2 _dashVector;

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

        private void Update()
        {

        }

        private void StartDashPC(InputAction.CallbackContext context)
        {
            if (CanDash() && GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                if(Keyboard.current.wKey.wasPressedThisFrame)
                {
                    if (Time.time - _totalTimeToDash < dashPcTimeThreshold && _lastKey == Key.W) 
                    {
                        _directionDash = transform.forward;
                        StartCoroutine(Dash());
                    }
                    else
                    {
                        _totalTimeToDash = Time.time;
                        _lastKey = Key.W;
                    }
                }

                else if (Keyboard.current.sKey.wasPressedThisFrame)
                {
                    if (Time.time - _totalTimeToDash < dashPcTimeThreshold && _lastKey == Key.S)
                    {
                        _directionDash = -transform.forward;
                        StartCoroutine(Dash());
                    }
                    else
                    {
                        _totalTimeToDash = Time.time;
                        _lastKey = Key.S;
                    }
                }                
                
                else if (Keyboard.current.aKey.wasPressedThisFrame)
                {
                    if (Time.time - _totalTimeToDash < dashPcTimeThreshold && _lastKey == Key.A)
                    {
                        _directionDash = -transform.right;
                        StartCoroutine(Dash());
                    }
                    else
                    {
                        _totalTimeToDash = Time.time;
                        _lastKey = Key.A;
                    }
                }                
                
                else if (Keyboard.current.dKey.wasPressedThisFrame)
                {
                    if (Time.time - _totalTimeToDash < dashPcTimeThreshold && _lastKey == Key.D)
                    {
                        _directionDash = transform.right;
                        StartCoroutine(Dash());
                    }
                    else
                    {
                        _totalTimeToDash = Time.time;
                        _lastKey = Key.D;
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
            _mainCAnim.TriggerDash();
            _mainCMove.DisableMovement();
            _dashTotalCooldown = Time.time + dashCooldown;
            yield return new WaitForSeconds(_dashTime);
            var startTime = Time.time;
            while (Time.time < startTime + _dashTime)
            {
                _gTrail.StartTrail();

                float curveTime = (Time.time - startTime) / _dashTime;
                float speedMultiplier = _mainCMove.dashSpeedCurve.Evaluate(curveTime);

                _cc.Move(_directionDash.normalized * dashSpeed * speedMultiplier * Time.deltaTime);
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
