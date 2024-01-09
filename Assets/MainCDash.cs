using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCDash : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        _playerInputActions.Enable();
        _playerInputActions.PlayerPC.Dash.
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
}
