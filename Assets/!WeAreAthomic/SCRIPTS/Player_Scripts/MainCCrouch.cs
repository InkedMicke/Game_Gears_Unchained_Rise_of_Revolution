using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MainCCrouch : MonoBehaviour
    {
        private PlayerInputActions m_inputActions;
        private CharacterController m_cc;
        private MainCLayers m_mainCLayers;
        private MainCAnimatorController m_mainCAnim;
        private MainCMovement m_mainCMove;
        private MainCRail m_mainCRail;

        [SerializeField] private CapsuleCollider _hurtBoxCC;

        [SerializeField] private LayerMask canStandLayers;

        private Vector2 _moveVectorKeyboard;
        private Vector2 _moveVectorGamepad;

        [NonSerialized] public bool IsCrouch;
        private bool _isCrouchWalking;

        [SerializeField] private float crouchSpeed;
        [SerializeField] private float timeNextCrouch = 0.5f;
        private float m_timeGraceCrouchPeriod;

        [SerializeField] TextMeshProUGUI debugBuildText1;
        [SerializeField] TextMeshProUGUI debugBuildText2;
        [SerializeField] TextMeshProUGUI debugBuildText3;
        [SerializeField] TextMeshProUGUI debugBuildText4;

        private void Awake()
        {
            m_cc = GetComponent<CharacterController>();
            m_mainCMove = GetComponent<MainCMovement>();
            m_mainCAnim = GetComponent<MainCAnimatorController>();
            m_mainCLayers = GetComponent<MainCLayers>();
            m_mainCRail = GetComponent<MainCRail>();

            m_inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            m_inputActions.Enable();
            m_inputActions.PlayerPC.Crouch.performed += ToggleCrouch;
            m_inputActions.PlayerGamepad.Crouch.performed += ToggleCrouch;
        }

        private void OnDisable()
        {
            m_inputActions.Disable();
            m_inputActions.PlayerPC.Crouch.performed -= ToggleCrouch;
            m_inputActions.PlayerGamepad.Crouch.performed -= ToggleCrouch;
        }

        private void Update()
        {
            _moveVectorKeyboard = m_inputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();
            _moveVectorGamepad = m_inputActions.PlayerGamepad.MovementGamepad.ReadValue<Vector2>();
            CrouchWalking();
        }

        void ToggleCrouch(InputAction.CallbackContext context)
        {

            if (m_mainCMove.IsGrounded() && !m_mainCRail.IsSliding)
            {
                CrouchGround();
            }

            if (!m_mainCMove.IsGrounded() && m_mainCRail.IsSliding)
            {
                CrouchRail();
            }
        }

        void CrouchGround()
        {
            if (Time.time < m_timeGraceCrouchPeriod) return;

            IsCrouch = !IsCrouch;
            ToggleCCSize();
            if (IsCrouch)
            {
                if (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f)
                {
                    _isCrouchWalking = true;
                    m_mainCAnim.SetCrouchWalking(_isCrouchWalking);
                }
            }

            m_mainCAnim.SetCrouch(IsCrouch);
            m_timeGraceCrouchPeriod = Time.time + timeNextCrouch;
        }

        void CrouchRail()
        {
            IsCrouch = !IsCrouch;
            ToggleCCSize();
            m_mainCAnim.SetSlidingCrouch(IsCrouch);
        }

        private void StartEndCrouch()
        {

            if (Time.time > m_timeGraceCrouchPeriod)
            {
                /*                if (!CanStandUp())
                                {
                                    return;
                                }*/



                if (!IsCrouch && _isCrouchWalking)
                {
                    m_mainCAnim.SetCrouchWalking(false);
                    _isCrouchWalking = false;
                    m_mainCMove.CurrentSpeed = m_mainCMove.walkSpeed;
                    Invoke(nameof(InvokeDisableCrouchLayer), 0.1f);
                }

                debugBuildText4.text = IsCrouch.ToString();
                if (IsCrouch)
                {
                    debugBuildText3.text = "funciona3";
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

                if (m_mainCRail.IsSliding)
                {
                    m_mainCAnim.SetSlidingCrouch(IsCrouch);
                }
                else
                {
                    debugBuildText3.text = "funciona4";

                }


            }
        }

        private void CrouchWalking()
        {
            if (IsCrouch && (_moveVectorKeyboard.magnitude > 0.1f || _moveVectorGamepad.magnitude > 0.1f))
            {
                _isCrouchWalking = true;
                m_mainCAnim.SetCrouchWalking(_isCrouchWalking);
                m_mainCMove.CurrentSpeed = crouchSpeed;
                if (m_mainCMove.IsRunning)
                {
                    m_mainCMove.IsRunning = !m_mainCMove.IsRunning;
                }

            }
            else
            {
                if (_isCrouchWalking)
                {
                    _isCrouchWalking = false;
                    m_mainCAnim.SetCrouchWalking(_isCrouchWalking);
                    m_mainCMove.CurrentSpeed = m_mainCMove.walkSpeed;
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
            if (Physics.CheckSphere(transform.position + Vector3.up.normalized * m_cc.height, 0.2f, canStandLayers))
            {
                var col = Physics.OverlapSphere(transform.position + Vector3.up.normalized * m_cc.height, 0.2f, canStandLayers);

                foreach (var x in col)
                {
                    if (!x.CompareTag("Volume"))
                    {
                        return false;
                    }
                }

            }

            return true;
        }
    }
}