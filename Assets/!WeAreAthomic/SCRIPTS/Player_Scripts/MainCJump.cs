using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCJump : MonoBehaviour
    {
        private PlayerInputActions m_playerInputActions;
        private MainCAnimatorController m_mainCAnim;
        private MainCLayers m_mainCLayers;
        private MainCMovement m_mainCMove;
        private MainCRail m_mainCRail;
        private MainCSounds m_mainCSounds;
        private MainCVFX m_mainCVFX;
        private MainCCrouch m_mainCCrouch;

        [NonSerialized] public bool IsJumping;

        [SerializeField] private float jumpImpulse = 5f;
        [SerializeField] private float jumpImpulseOnRail = 5f;
        public float TimeNextJump = 0.5f;
        [NonSerialized] public float TimeGraceJumpPeriod;

        private void Awake()
        {
            m_mainCLayers = GetComponent<MainCLayers>();
            m_mainCAnim = GetComponent<MainCAnimatorController>();
            m_mainCMove = GetComponent<MainCMovement>();
            m_mainCRail = GetComponent<MainCRail>();
            m_mainCSounds = GetComponent<MainCSounds>();
            m_mainCCrouch = GetComponent<MainCCrouch>();
            m_mainCVFX = GetComponent<MainCVFX>();

            m_playerInputActions = new PlayerInputActions();
            m_playerInputActions.Enable();
            m_playerInputActions.PlayerPC.Jump.performed += StartJumpPC;
            m_playerInputActions.PlayerGamepad.Jump.performed += StartJumpGamepad;
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
            if (CanJumpGround() && (m_mainCMove.IsGrounded() || m_mainCMove.IsOnSlope()))
            {
                m_mainCSounds.PlayJumpSound();
                IsJumping = true;
                m_mainCAnim.SetGrounded(false);
                m_mainCLayers.EnableJumpLayer();
                m_mainCAnim.SetJumping(true);
                m_mainCMove.Velocity = transform.up.normalized * jumpImpulse;
                TimeGraceJumpPeriod = Time.time + TimeNextJump;
            }

            if (CanJumpRail())
            {
                if (m_mainCCrouch.IsCrouch)
                {
                    m_mainCAnim.SetSlidingCrouch(false);
                    m_mainCCrouch.IsCrouch = false;
                }
                m_mainCSounds.PlayJumpSound();
                IsJumping = true;
                m_mainCAnim.SetGrounded(false);
                m_mainCLayers.EnableJumpLayer();
                m_mainCAnim.SetJumping(true);
                m_mainCMove.Velocity = transform.up.normalized * (m_mainCRail.HigherJumpDueToInclination() ? jumpImpulseOnRail * 3f : jumpImpulseOnRail);
                TimeGraceJumpPeriod = Time.time + TimeNextJump;

                m_mainCVFX.SetActiveSparks(false);
            }
        }

        public void SetIsJumping(bool isJumping)
        {
            IsJumping = isJumping;
        }

        public void EndJump()
        {
            if (IsJumping)
            {
                m_mainCLayers.DisableJumpLayer();
            }
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

            if(GameManagerSingleton.Instance.IsOnDialogue)
            {
                return false;
            }

            if (IsJumping)
            {
                return false;
            }

            if (Time.time < TimeGraceJumpPeriod)
            {
                return false;
            }

            if (m_mainCRail.IsSliding)
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

            if (!m_mainCRail.IsOnRail())
            {
                return false;
            }

            if (IsJumping)
            {
                return false;
            }

            if (Time.time < TimeGraceJumpPeriod)
            {
                return false;
            }

            if (m_mainCRail.HigherJumpDueToInclination())
            {
                return false;
            }

            return true;
        }

    }
}