using _WeAreAthomic.SCRIPTS.PP_Scripts;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PPS;
using Cinemachine;
using Generics;

namespace Player
{
    public class MainCStopMenu : MonoBehaviour
    {
        CharacterController m_cc;

        [SerializeField] private PP m_PP;
        [SerializeField] private CheckIfEnoughMejoras m_CheckIfEnoughMejoras;
        private PlayerInputActions _playerInputActions;
        private MainCSounds _mainCSounds;
        MainCAnimatorController m_anim;
        [SerializeField] private CinemachineVirtualCamera cameraUI;
        [SerializeField] private GameObject stopMenuContainer;
        [SerializeField] private GameObject firstButton;

        [SerializeField] LayerMask wallLayer;

        private bool _isActive;

        float m_stopMenuCooldown = 1f;
        float m_stopMenuTotalCooldown;

        [SerializeField] private UnityEvent setTriggerAnim;

        private void Awake()
        {
            m_anim = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            m_cc = GetComponent<CharacterController>();

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
        public void ToggleMenu()
        {
            if (CanToggleMenu())
            {
                if (GameManagerSingleton.Instance.IsStopMenuEnabled)
                {
                    m_CheckIfEnoughMejoras.CheckIfYouHaveEnoughToBuy();
                    stopMenuContainer.SetActive(false);
                    cameraUI.Priority = 0;
                    m_anim.SetIsOnMenu(false);
                    if (!GameManagerSingleton.Instance.thereIsCanvasBelow)
                    {
                        GameManagerSingleton.Instance.ShowCursor(false);
                    }
                    m_PP.SetActiveToCurrentUIGameObjectList(true);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
                    _mainCSounds.UnPauseCurrentSounds();
                    m_stopMenuTotalCooldown = Time.time + m_stopMenuCooldown;
                }
                else
                {
                    CheckWall();
                    setTriggerAnim.Invoke();
                    stopMenuContainer.SetActive(true);
                    m_anim.SetIsOnMenu(true);
                    if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
                    {
                        EventSystem.current.SetSelectedGameObject(firstButton);
                        firstButton.GetComponent<C_UIMaterial_Changer>().OnPointerEnter();
                    }
                    
                    m_PP.SetActiveToCurrentUIGameObjectList(false);
                    GameManagerSingleton.Instance.ShowCursor(true);
                    GameManagerSingleton.Instance.SetIsStopMenuEnabled(true);
                    _mainCSounds.PauseCurrentSounds();
                    _mainCSounds.PlayOpenMenuSound();
                    cameraUI.Priority = 100;
                    m_stopMenuTotalCooldown = Time.time + m_stopMenuCooldown;
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

        void CheckWall()
        {
            const float distance = 1f;
            var ray = new Ray(transform.position + transform.up, transform.forward);
            if(Physics.Raycast(ray, out var hit, distance, wallLayer))
            {
                var posToMove = Vector3.Distance(transform.position, ray.GetPoint(distance)) - hit.distance;
                m_cc.Move(-transform.forward * posToMove);
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position + transform.up * 2f, transform.forward * 1f, Color.white);
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
            
            if(GameManagerSingleton.Instance.IsOnDialogue)
            {
                return false;
            }

            if(Time.time < m_stopMenuTotalCooldown)
            {
                return false;
            }

            return true;
        }
    }
}
