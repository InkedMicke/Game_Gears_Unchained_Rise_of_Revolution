using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GStopMenu : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    [SerializeField] private GameObject mainMenuObj;

    private bool _isActive;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Escape.performed += ToggleMenu;
    }

    private void Update()
    {
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (_isActive)
        {
            mainMenuObj.SetActive(true);
            CursorMode(false);
            GameManagerSingleton.Instance.PauseGame(false);
            FreezeTime(_isActive);
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

    public void ToggleMenuCallable(bool condition)
    {
        mainMenuObj.SetActive(condition);
        _isActive = condition;

    }

    public void ToggleToggleMenuCallable()
    {
        _isActive = !_isActive;
        mainMenuObj.SetActive(_isActive);
        CursorMode(_isActive);
        GameManagerSingleton.Instance.PauseGame(_isActive);
        FreezeTime(_isActive);

    }

    public void CursorMode(bool condition)
    {
        if (condition)
        {
            Cursor.visible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = CursorLockMode.Locked;
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
