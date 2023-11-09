using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GStopMenu : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    [SerializeField] private GameObject mainMenuObj;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Escape.performed += ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        mainMenuObj.SetActive(!mainMenuObj.activeInHierarchy);
        CursorMode(mainMenuObj.activeInHierarchy);
        GameManagerSingleton.Instance.PauseGame(mainMenuObj.activeInHierarchy);
        FreezeTime(mainMenuObj.activeInHierarchy);

    }

    private void CursorMode(bool condition)
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

    private void FreezeTime(bool condition)
    {
        Time.timeScale = condition ? 0 : 1;
    }
}
