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
        [SerializeField] private AudioMixerGroup sfxMixer;
        [SerializeField] private GameObject soundComponentObj;
        [SerializeField] private AudioClip openMenuClip;
        [Range(0, 1)]
        [SerializeField] private float openMenuVolume;

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
                        PlayOpenMenuSound();
                        setTriggerAnim.Invoke();
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
          public void PlayOpenMenuSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = openMenuClip;
                currentAudioSource.volume = openMenuVolume;
             
                currentAudioSource.Play();
            }
        }
    }
}
