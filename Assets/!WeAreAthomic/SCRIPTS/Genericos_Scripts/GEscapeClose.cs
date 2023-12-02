using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GEscapeClose : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    [SerializeField] private UnityEvent actionsWhenEscape;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.UI.LeaveUI.performed += EscapeAction;
    }
    
    private void EscapeAction(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.IsGamePaused)
        {
            actionsWhenEscape.Invoke();
            MethodBase methodBase = MethodBase.GetCurrentMethod();
            Debug.Log(methodBase.Name);
        }
    }
}
