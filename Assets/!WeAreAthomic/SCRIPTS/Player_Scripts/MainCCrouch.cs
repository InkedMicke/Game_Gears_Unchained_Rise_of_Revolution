using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCCrouch : MonoBehaviour
    {
        private PlayerInputActions m_inputActions;
        private CharacterController m_cc;
        private MainCLayers m_mainCLayers;
        private MainCAnimatorController m_mainCAnim;
        private MainCMovement m_mainCMove;
        private MainCRails m_mainCRail;

        [SerializeField] private CapsuleCollider _hurtBoxCC;

        [SerializeField] private LayerMask canStandLayers;

        private Vector2 _moveVectorKeyboard;
        private Vector2 _moveVectorGamepad;

        [NonSerialized] public bool IsCrouch;
        private bool _isCrouchWalking;

        [SerializeField] private float crouchSpeed;
        [SerializeField] private float timeNextCrouch = 0.5f;
        private float m_timeGraceCrouchPeriod;

        private void OnValidate()
        {
            m_cc = GetComponent<CharacterController>();
        }

        private void Awake()
        {
            m_mainCMove = GetComponent<MainCMovement>();
            m_mainCAnim = GetComponent<MainCAnimatorController>();
            m_mainCLayers = GetComponent<MainCLayers>();
            m_mainCRail = GetComponent<MainCRails>();

            m_inputActions = new PlayerInputActions();
            m_inputActions.Enable();
            m_inputActions.PlayerPC.Crouch.performed += StartEndCrouch;
            m_inputActions.PlayerGamepad.Crouch.performed += StartEndCrouch;
        }

        private void Update()
        {
            _moveVectorKeyboard = m_inputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();
            _moveVectorGamepad = m_inputActions.PlayerGamepad.MovementGamepad.ReadValue<Vector2>();

            CrouchWalking();
        }

        private void StartEndCrouch(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.IsOnDialogue)
            {
                return;
            }

            if (Time.time > m_timeGraceCrouchPeriod)
            {
                IsCrouch = !IsCrouch;
                if(!CanStandUp())
                {
                    return;
                }
                ToggleCCSize();
                if (!IsCrouch && _isCrouchWalking)
                {
                    m_mainCAnim.SetCrouchWalking(false);
                    _isCrouchWalking = false;
                    m_mainCMove.CurrentWalkSpeed = m_mainCMove.walkSpeed;
                    Invoke(nameof(InvokeDisableCrouchLayer), 0.1f);
                }

                if (m_mainCRail.IsSliding)
                {
                    m_mainCAnim.SetSlidingCrouch(IsCrouch);
                }
                else
                {
                    m_mainCAnim.SetCrouch(IsCrouch);
                }

                m_timeGraceCrouchPeriod = Time.time + timeNextCrouch;

                if (IsCrouch)
                {
                    m_mainCLayers.EnableCrouchLayer();
                    if (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f)
                    {
                        _isCrouchWalking = true;
                        m_mainCAnim.SetCrouchWalking(_isCrouchWalking);
                    }
                }
                else
                {
                    Invoke(nameof(InvokeDisableCrouchLayer), 0.5f);
                }
            }
        }

        private void CrouchWalking()
        {
            if (IsCrouch && (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f))
            {
                _isCrouchWalking = true;
                m_mainCAnim.SetCrouchWalking(_isCrouchWalking);
                m_mainCMove.CurrentWalkSpeed = crouchSpeed;
                if (m_mainCMove.IsRunningKeyboard || m_mainCMove.IsRunningGamepad)
                {
                    m_mainCMove.IsRunningKeyboard = !m_mainCMove.IsRunningKeyboard;
                    m_mainCMove.IsRunningGamepad = !m_mainCMove.IsRunningGamepad;
                }

            }
            else
            {
                if (_isCrouchWalking)
                {
                    _isCrouchWalking = false;
                    m_mainCAnim.SetCrouchWalking(_isCrouchWalking);
                    m_mainCMove.CurrentWalkSpeed = m_mainCMove.walkSpeed;
                }
            }
        }

        private void InvokeDisableCrouchLayer()
        {
            m_mainCLayers.DisableCrouchLayer();
        }


        /// <summary>
        /// Cambia el tamaño de la capsula del Character Controller
        /// </summary>
        public void ToggleCCSize()
        {
            if (IsCrouch)
            {
                //Character Controller
                var crouchPosCc = m_cc.center;
                crouchPosCc.y = .63f;
                m_cc.center = crouchPosCc;
                m_cc.height = 1.24f;
                //HurtBox
                var crouchPosCC = _hurtBoxCC.center;
                crouchPosCC.y = .73f;
                _hurtBoxCC.center = crouchPosCC;
                _hurtBoxCC.height = 1.43f;
            }
            else
            {
                //Character Controller
                var originalPosCc = m_cc.center;
                originalPosCc.y = .87f;
                m_cc.center = originalPosCc;
                m_cc.height = 1.69f;
                //HurtBox
                var originalPosCC = _hurtBoxCC.center;
                originalPosCC.y = .91f;
                _hurtBoxCC.center = originalPosCC;
                _hurtBoxCC.height = 1.79f;
            }
        }

        public void SetIsCrouch(bool isCrouch)
        {
            IsCrouch = isCrouch;
        }

        public bool CanStandUp()
        {
            if(Physics.CheckSphere(transform.position + Vector3.up.normalized * m_cc.height, 0.3f, canStandLayers))
            {
                var col = Physics.OverlapSphere(transform.position + Vector3.up.normalized * m_cc.height, 0.3f, canStandLayers);
                Debug.Log(col[0].name);
                return false;
            }

            return true;
        }
    }
}