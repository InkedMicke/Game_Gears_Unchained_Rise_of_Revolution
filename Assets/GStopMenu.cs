using System.Collections;
using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (!_mainCTutorial.IsOnTutorialImage)
        {
            if (_isActive)
            {
                mainMenuObj.SetActive(false);
                CursorMode(false);
                GameManagerSingleton.Instance.PauseGame(false);
                FreezeTime(false);
                _isActive = false;
            }
            else
            {
                mainMenuObj.SetActive(true);
                CursorMode(true);
                GameManagerSingleton.Instance.PauseGame(true);
                FreezeTime(true);
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
        Debug.Log("hola1");
        if (_isActive)
        {
            mainMenuObj.SetActive(true);
            CursorMode(false);
            GameManagerSingleton.Instance.PauseGame(false);
            FreezeTime(false);
            _isActive = false;
        }
        else
        {
            mainMenuObj.SetActive(false);
            CursorMode(true);
            GameManagerSingleton.Instance.PauseGame(true);
            _isActive = true;
        }

    }

    public void CursorMode(bool condition)
    {
        if (condition)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    public void FreezeTime(bool condition)
    {
        Time.timeScale = condition ? 0 : 1;
    }

    public void GameState(bool condition)
    {
        GameManagerSingleton.Instance.PauseGame(condition);
    }
}
