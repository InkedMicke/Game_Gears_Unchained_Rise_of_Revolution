using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GStopMenu : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private MainCTutorialChecker _mainCTutorial;
        private MainCSounds _mainCSounds;

        [SerializeField] private GameObject stopMenuContainer;
        [SerializeField] private GameObject firstButton;

        private bool _isActive;

        private void Awake()
        {
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCSounds = GetComponent<MainCSounds>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Escape.performed += ToggleMenu;
        }

        private void Update()
        {
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (!GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                if (GameManagerSingleton.Instance.IsStopMenuEnabled)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    stopMenuContainer.SetActive(false);
                    if (!GameManagerSingleton.Instance.thereIsCanvasBelow)
                    {
                        GameManagerSingleton.Instance.CursorMode(false);
                    }
                    GameManagerSingleton.Instance.PauseGame(false);
                    GameManagerSingleton.Instance.FreezeTime(false);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
                    _mainCSounds.UnPauseCurrentSounds();
                    _isActive = false;
                }
                else
                {
                    stopMenuContainer.SetActive(true);
                    GameManagerSingleton.Instance.CursorMode(true);
                    GameManagerSingleton.Instance.PauseGame(true);
                    GameManagerSingleton.Instance.FreezeTime(true);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                    EventSystem.current.SetSelectedGameObject(firstButton);
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
