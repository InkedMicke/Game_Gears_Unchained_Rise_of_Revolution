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

        _playerInputActions.Player.QuitWindow.performed += EscapeAction;
    }
    
    private void EscapeAction(InputAction.CallbackContext context)
    {
        actionsWhenEscape.Invoke();
    }
}
