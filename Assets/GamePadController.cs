using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePadController : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    private Scene _currentScene;

    private Vector2 vectorGamepad;
    private Vector2 lastMousePosition;

    private bool _isActive;
    public bool IsGamepadActive;

    private int _currentTab;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;

    [SerializeField] private List<GameObject> tabsSettings;
    [SerializeField] private List<GameObject> panelsSettings;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.UI.Navigate.performed += HideCursorIfVisible;
        _playerInputActions.UI.ReTab.performed += ReTab;
        _playerInputActions.UI.AvTab.performed += AvTab;

        _currentScene = SceneManager.GetActiveScene();

        if(_currentScene.name == "TESTING")
        {
            GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
            GameManagerSingleton.Instance.GameState(false);
            GameManagerSingleton.Instance.SetIsOnTutorialImage(false);
            GameManagerSingleton.Instance.SetIsSettingsMenuEnabled(false);
        }
    }

    private void Update()
    {
        if (GameManagerSingleton.Instance.IsStopMenuEnabled || GameManagerSingleton.Instance.IsSettingsMenuEnabled)
        {
            vectorGamepad = _playerInputActions.Player.MovementGamepad.ReadValue<Vector2>();
            if (vectorGamepad.magnitude > .1f)
            {
                IsGamepadActive = true;
                if (!_isActive)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    _isActive = true;
                }

                if(Cursor.visible)
                {
                    GameManagerSingleton.Instance.CursorMode(false);
                }
            }
            else
            {
                if(_isActive)
                {
                    _isActive = false;
                }

                var currentMousePosition = Mouse.current.position.ReadValue();

                if (currentMousePosition != lastMousePosition)
                {
                    if (IsGamepadActive)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        IsGamepadActive = false;
                    }
                    GameManagerSingleton.Instance.CursorMode(true);
                    // Realiza acciones cuando el ratón se mueve
                }

                lastMousePosition = currentMousePosition;
            }
        }
    }

    private void ReTab(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.IsSettingsMenuEnabled && !(_currentTab >= tabsSettings.Count - 1))
        {
            SetImageColorOfCurrentTab(_currentTab, Color.white);
            panelsSettings[_currentTab].SetActive(false);
            _currentTab++;
            SetImageColorOfCurrentTab(_currentTab, Color.red);
            panelsSettings[_currentTab].SetActive(true);
        }
    }

    private void AvTab(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.IsSettingsMenuEnabled && !(_currentTab <= 0))
        {
            SetImageColorOfCurrentTab(_currentTab, Color.white);
            panelsSettings[_currentTab].SetActive(false);
            _currentTab--;
            SetImageColorOfCurrentTab(_currentTab, Color.red);
            panelsSettings[_currentTab].SetActive(true);
        }
    }

    private void HideCursorIfVisible(InputAction.CallbackContext context)
    {
        if (Cursor.visible)
        {
            GameManagerSingleton.Instance.CursorMode(false);
        }
    }

    private void SetImageColorOfCurrentTab(int value, Color color)
    {
        tabsSettings[value].transform.GetChild(0).GetComponent<Image>().color = color;
    }


}
