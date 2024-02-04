using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCStopMenu : MonoBehaviour
    {
        [SerializeField] private PP m_PP;
        private PlayerInputActions _playerInputActions;
        private MainCSounds _mainCSounds;

        [SerializeField] private GameObject stopMenuContainer;
        [SerializeField] private GameObject firstButton;

        private bool _isActive;

        private void Awake()
        {
            _mainCSounds = GetComponent<MainCSounds>();

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
            if (!GameManagerSingleton.Instance.IsGameOverEnabled && !GameManagerSingleton.Instance.IsAbilityMenuEnabled)
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
                        m_PP.SetActiveToCurrentUIGameObjectList(true);
                        GameManagerSingleton.Instance.PauseGame(false);
                        GameManagerSingleton.Instance.FreezeTime(false);
                        GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
                        _mainCSounds.UnPauseCurrentSounds();
                        _isActive = false;
                    }
                    else
                    {
                        stopMenuContainer.SetActive(true);
                        if(GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                        {
                            EventSystem.current.SetSelectedGameObject(firstButton);
                            firstButton.GetComponent<C_UIMaterial_Changer>().OnPointerEnter();
                        }
                        else
                        {
                            EventSystem.current.SetSelectedGameObject(null);
                        }
                        m_PP.SetActiveToCurrentUIGameObjectList(false);
                        GameManagerSingleton.Instance.CursorMode(true);
                        GameManagerSingleton.Instance.PauseGame(true);
                        GameManagerSingleton.Instance.FreezeTime(true);
                        GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                        _mainCSounds.PauseCurrentSounds();
                        _isActive = true;
                    }
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
