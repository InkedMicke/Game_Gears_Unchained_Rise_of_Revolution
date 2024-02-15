using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GEscapeClose : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    [SerializeField] private UnityEvent actionsWhenEscape;
    [SerializeField] private UnityEvent actionsWhenEscapeWithDelay;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.UI.LeaveUI.performed += EscapeAction;
    }

    private void EscapeAction(InputAction.CallbackContext context)
    {

        actionsWhenEscape.Invoke();
        Invoke(nameof(EventWithDelay), .1f);
    }

    public void EventWithDelay()
    {
        actionsWhenEscapeWithDelay.Invoke();
    }
}
