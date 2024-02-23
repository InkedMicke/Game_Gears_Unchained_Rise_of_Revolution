using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
namespace _WeAreAthomic.SCRIPTS.Debug_Scripts
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

        [SerializeField] private GameObject godModeContainer;
        private GameObject _cameraObj;

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

        private void Start()
        {
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
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
                var moveVectorKeyboard = _playerInputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();

                var direction = new Vector3(moveVectorKeyboard.x, 0f, moveVectorKeyboard.y).normalized;
                var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraObj.transform.eulerAngles.y;
                var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _mainCMove._turnSmoothVelocityKeyboard,
                    _mainCMove.turnSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);


                var moveDir = _cameraObj.transform.forward * (Time.deltaTime * 5f * direction.magnitude * _moveSpeed);
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
    
    }
}
