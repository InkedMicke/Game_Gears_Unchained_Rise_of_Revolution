using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GStopMenu : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private MainCTutorialChecker _mainCTutorial;
        private MainCSounds _mainCSounds;
        private MainCInputSwitcher _mainCInputSwitcher;

        [SerializeField] private GameObject stopMenuContainer;
        [SerializeField] private GameObject firstButton;
        [SerializeField] private GameObject playerInterface;
        [SerializeField] private GameObject tutKeys;

        private bool _isActive;

        private void Awake()
        {
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCInputSwitcher = GetComponent<MainCInputSwitcher>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Escape.performed += InputPC;
            _playerInputActions.PlayerGamepad.Escape.performed += InputGamepad;
        }

        private void InputPC(InputAction.CallbackContext context)
        {
            if(GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                ToggleMenu();
            }
        }

        private void InputGamepad(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            if (!GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                if (GameManagerSingleton.Instance.IsStopMenuEnabled)
                {
                    stopMenuContainer.SetActive(false);
                    if (!GameManagerSingleton.Instance.thereIsCanvasBelow)
                    {
                        GameManagerSingleton.Instance.CursorMode(false);
                    }
                    tutKeys.SetActive(true);
                    playerInterface.SetActive(true);
                    GameManagerSingleton.Instance.PauseGame(false);
                    GameManagerSingleton.Instance.FreezeTime(false);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
                    _mainCSounds.UnPauseCurrentSounds();
                    _isActive = false;
                }
                else
                {
                    if(_mainCInputSwitcher.isUsingMouse)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }

                    if (_mainCInputSwitcher.isUsingGamepad)
                    {
                        EventSystem.current.SetSelectedGameObject(firstButton);
                    }
                    stopMenuContainer.SetActive(true);
                    tutKeys.SetActive(false);
                    playerInterface.SetActive(false);
                    GameManagerSingleton.Instance.CursorMode(true);
                    GameManagerSingleton.Instance.PauseGame(true);
                    GameManagerSingleton.Instance.FreezeTime(true);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                    _mainCSounds.PauseCurrentSounds();
                    _isActive = true;
                }
            }


        }

        public void ToggleMenuCallable(bool condition)
        {
            stopMenuContainer.SetActive(condition);
            _isActive = condition;

        }

        public void ToggleToggleMenuCallable()
        {
            if (_isActive)
            {
                stopMenuContainer.SetActive(true);
                GameManagerSingleton.Instance.CursorMode(false);
                GameManagerSingleton.Instance.PauseGame(false);
                GameManagerSingleton.Instance.FreezeTime(false);
                _isActive = false;
            }
            else
            {
                stopMenuContainer.SetActive(false);
                GameManagerSingleton.Instance.CursorMode(true);
                GameManagerSingleton.Instance.PauseGame(true);
                _isActive = true;
            }

        }
    }
}
