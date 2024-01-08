using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePadController : MonoBehaviour
{
    private MainCInputSwitcher _mainCInputSwitcher;

    private Scene _currentScene;

    private Vector2 vectorGamepad;

    private bool _isActive;
    public bool IsGamepadActive;



    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;

    [SerializeField] private List<GameObject> tabsSettings;
    [SerializeField] private List<GameObject> panelsSettings;

    private void Awake()
    {
        _mainCInputSwitcher = GetComponent<MainCInputSwitcher>();



        _currentScene = SceneManager.GetActiveScene();

        if(_currentScene.name == "TESTING")
        {
            GameManagerSingleton.Instance.SetIsStopMenuEnabled(false);
            GameManagerSingleton.Instance.GameState(false);
            GameManagerSingleton.Instance.SetIsSettingsMenuEnabled(false);
        }
    }

    private void Update()
    {
        if (GameManagerSingleton.Instance.IsStopMenuEnabled || GameManagerSingleton.Instance.IsSettingsMenuEnabled)
        {
/*            vectorGamepad = _playerInputActions.Player.MovementGamepad.ReadValue<Vector2>();
            if (_mainCInputSwitcher.isUsingGamepad)
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



                if (_mainCInputSwitcher.isUsingMouse)
                {
                    if (IsGamepadActive)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        IsGamepadActive = false;
                    }
                    GameManagerSingleton.Instance.CursorMode(true);
                    // Realiza acciones cuando el ratón se mueve
                }


            }*/
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
