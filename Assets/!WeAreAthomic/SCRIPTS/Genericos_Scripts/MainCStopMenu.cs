using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCStopMenu : MonoBehaviour
    {
        [SerializeField] private PP m_PP;
        [SerializeField] private CheckIfEnoughMejoras m_CheckIfEnoughMejoras;
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

        [Button]
        private void ToggleMenu()
        {
            if (!GameManagerSingleton.Instance.IsGameOverEnabled && !GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                if (!GameManagerSingleton.Instance.IsSettingsMenuEnabled)
                {
                    if (GameManagerSingleton.Instance.IsStopMenuEnabled)
                    {
                        m_CheckIfEnoughMejoras.CheckIfYouHaveEnoughToBuy();
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
                    }
                    else
                    {
                        stopMenuContainer.SetActive(true);
                        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                        {
                            EventSystem.current.SetSelectedGameObject(firstButton);
                            firstButton.GetComponent<C_UIMaterial_Changer>().OnPointerEnter();
                        }

                        m_PP.SetActiveToCurrentUIGameObjectList(false);
                        GameManagerSingleton.Instance.CursorMode(true);
                        GameManagerSingleton.Instance.PauseGame(true);
                        //StartCoroutine(WaitForFreeze());
                        GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                        _mainCSounds.PauseCurrentSounds();
                    }
                }
            }

        }

        private IEnumerator WaitForFreeze()
        {
            yield return new WaitForSeconds(.2f);
            GameManagerSingleton.Instance.FreezeTime(true);
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
