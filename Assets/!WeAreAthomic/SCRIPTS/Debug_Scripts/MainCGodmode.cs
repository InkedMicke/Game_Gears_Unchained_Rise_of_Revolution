using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Player
{
    public class MainCGodmode : MonoBehaviour
    {
        private CharacterController _cc;
        private PlayerInputActions _playerInputActions;
        private MainCMovement _mainCMove;
        private MainCTutorialChecker _mainCTutorial;
        private MainCRail _mainCRail;
        private MainCVFX _mainVFXCharacter;
        [SerializeField] private LabThirdRoomController _labController;
        MainCInput m_input;

        [SerializeField] private GameObject godModeContainer;
        [SerializeField] private Transform cameraBaseObj;

        [SerializeField] private Transform tpSkippedTutorial;

        private bool _isSpeedingUp;
        private bool _isSpeedingDown;

        private float _moveSpeed;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCMove = GetComponent<MainCMovement>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCRail = GetComponent<MainCRail>();
            _mainVFXCharacter = GetComponent<MainCVFX>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.GodMode.performed += EnableGodMode;
            _playerInputActions.PlayerPC.Running.performed += ShiftDown;
            _playerInputActions.PlayerPC.Running.canceled += ShiftUp;
            _playerInputActions.PlayerPC.Slow.performed += CtrlDown;
            _playerInputActions.PlayerPC.Slow.canceled += CtrlUp;
        }

        private void Update()
        {
            GodMode();
            AdjustSpeed();
            var layerValue = GameManagerSingleton.Instance.IsGodModeEnabled ? 14 : 16;
            var validLayer = Mathf.Clamp(layerValue, 0, 31);
            gameObject.layer = validLayer;
        }

        private void EnableGodMode(InputAction.CallbackContext context)
        {
            if (!GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                godModeContainer.SetActive(!godModeContainer.activeSelf);
                GameManagerSingleton.Instance.ToggleTotallyWindow();
            }
        }

        private void GodMode()
        {
            if(GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                var moveDir = GetMoveDir();

                _mainVFXCharacter.SetRailEffects(false);
                _cc.enabled = true;
                _cc.Move(moveDir);
            }
        }

        private void AdjustSpeed()
        {
            if (_isSpeedingUp)
            {
                _moveSpeed += Time.deltaTime * 4f;
            }

            if (_isSpeedingDown)
            {
                if (_moveSpeed > 0)
                {
                    _moveSpeed -= Time.deltaTime * 4f;
                }
            }
        }

        public void SkipTutorial()
        {
            if (!GameManagerSingleton.Instance.SkippedTutorial)
            {
                GameManagerSingleton.Instance.SetSkippedTutorial(true);
                GameManagerSingleton.Instance.SetHasUnlockedBastetAttack(true);
                _mainCTutorial.StopAllTutorialCoroutine();
                _cc.enabled = false;
                transform.position = tpSkippedTutorial.position;
                _cc.enabled = true;
                _labController.SkipTutorialLab();
            }
        }

        private void ShiftUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                _isSpeedingUp = false;
            }
        }
        private void ShiftDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                _isSpeedingUp = true;
            }
        }

        private void CtrlUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                _isSpeedingDown = false;
            }
        }

        private void CtrlDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.IsGodModeEnabled)
            {
                _isSpeedingDown = true;
            }
        }

        public Vector3 GetMoveDir()
        {
            var mouseVector = m_input.GetInput();
            var moveDir = new Vector3(mouseVector.x, 0f, mouseVector.y).normalized;
            Vector3 cameraForward = cameraBaseObj.forward;
            Vector3 cameraRight = cameraBaseObj.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            return moveDir = cameraForward * mouseVector.y + cameraRight * mouseVector.x;
        }

    }
}
