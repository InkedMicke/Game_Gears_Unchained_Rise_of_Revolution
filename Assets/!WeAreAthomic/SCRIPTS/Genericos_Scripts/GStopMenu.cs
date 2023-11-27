using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GStopMenu : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private MainCTutorialChecker _mainCTutorial;

        [SerializeField] private GameObject mainMenuObj;

        private bool _isActive;

        private void Awake()
        {
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
        
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Escape.performed += ToggleMenu;
        }

        private void Update()
        {
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (!GameManagerSingleton.Instance.IsOnTutorialImage)
            {
                if (GameManagerSingleton.Instance.IsStopMenuEnabled)
                {
                    mainMenuObj.SetActive(false);
                    if (!GameManagerSingleton.Instance.thereIsCanvasBelow)
                    {
                        GameManagerSingleton.Instance.CursorMode(false);
                    }
                    GameManagerSingleton.Instance.PauseGame(false);
                    GameManagerSingleton.Instance.FreezeTime(false);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
                }
                else
                {
                    mainMenuObj.SetActive(true);
                    GameManagerSingleton.Instance.CursorMode(true);
                    GameManagerSingleton.Instance.PauseGame(true);
                    GameManagerSingleton.Instance.FreezeTime(true);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                    _isActive = true;
                }
            }


        }

        public void ToggleMenuCallable(bool condition)
        {
            mainMenuObj.SetActive(condition);
            _isActive = condition;

        }

        public void ToggleToggleMenuCallable()
        {
            if (_isActive)
            {
                mainMenuObj.SetActive(true);
                GameManagerSingleton.Instance.CursorMode(false);
                GameManagerSingleton.Instance.PauseGame(false);
                GameManagerSingleton.Instance.FreezeTime(false);
                _isActive = false;
            }
            else
            {
                mainMenuObj.SetActive(false);
                GameManagerSingleton.Instance.CursorMode(true);
                GameManagerSingleton.Instance.PauseGame(true);
                _isActive = true;
            }

        }
    }
}
