using _WeAreAthomic.SCRIPTS.PP_Scripts;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
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

        [SerializeField] private UnityEvent setTriggerAnim;

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
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {

                ToggleMenu();
                ToggleCursorOnVideoStart();
            }
        }

        private void InputGamepad(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {

                ToggleMenu();
            }
        }

        private void ToggleCursorOnVideoStart()
        {
            if(GameManagerSingleton.Instance.IsLoadingStartVideos)
            {
                if(_isActive)
                {
                    GameManagerSingleton.Instance.ShowCursor(false);
                }
                else
                {
                    GameManagerSingleton.Instance.ShowCursor(true);
                }
            }
        }

        [Button]
        private void ToggleMenu()
        {
            if (CanToggleMenu())
            {
                if (GameManagerSingleton.Instance.IsStopMenuEnabled)
                {
                    m_CheckIfEnoughMejoras.CheckIfYouHaveEnoughToBuy();
                    stopMenuContainer.SetActive(false);
                    if (!GameManagerSingleton.Instance.thereIsCanvasBelow)
                    {
                        GameManagerSingleton.Instance.ShowCursor(false);
                    }
                    m_PP.SetActiveToCurrentUIGameObjectList(true);
                    GameManagerSingleton.Instance.PauseGame(false);
                    GameManagerSingleton.Instance.FreezeTime(false);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
                    _mainCSounds.UnPauseCurrentSounds();
                }
                else
                {
                    
                    setTriggerAnim.Invoke();
                    stopMenuContainer.SetActive(true);
                    if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                    {
                        EventSystem.current.SetSelectedGameObject(firstButton);
                        firstButton.GetComponent<C_UIMaterial_Changer>().OnPointerEnter();
                    }
                    
                    m_PP.SetActiveToCurrentUIGameObjectList(false);
                    GameManagerSingleton.Instance.ShowCursor(true);
                    GameManagerSingleton.Instance.PauseGame(true);
                    GameManagerSingleton.Instance.FreezeTime(true);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                    _mainCSounds.PauseCurrentSounds();
                    _mainCSounds.PlayOpenMenuSound();
                }

            }

        }

        private IEnumerator WaitForFreeze()
        {
            yield return new WaitForSeconds(1f);
            
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
                GameManagerSingleton.Instance.ShowCursor(false);
                GameManagerSingleton.Instance.PauseGame(false);
                GameManagerSingleton.Instance.FreezeTime(false);
                _isActive = false;
            }
            else
            {
                stopMenuContainer.SetActive(false);
                GameManagerSingleton.Instance.ShowCursor(true);
                GameManagerSingleton.Instance.PauseGame(true);
                _isActive = true;
            }

        }


        private bool CanToggleMenu()
        {
            if (GameManagerSingleton.Instance.IsGameOverEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                return false;
            }

            if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
            {
                return false;
            }

            if(GameManagerSingleton.Instance.IsLoadingStartVideos)
            {
                return false;
            }         
            
            if(GameManagerSingleton.Instance.thereIsCanvasBelow)
            {
                return false;
            }

            return true;
        }
    }
}
