using System.Collections;
using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Interact();
}


public class Interactor : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private ButtonInteractable _button;

    public LayerMask interactableLayer;

    [SerializeField] private float interactRange = 5f;

    [System.NonSerialized] public bool isSeeing;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Interact.performed += InteractRay;
    }

    private void FixedUpdate()
    {
        Ray();
    }

    void Ray()
    {
        var r = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(r, out RaycastHit hitInfo, interactRange, interactableLayer))
        {
            _button = hitInfo.collider.gameObject.GetComponent<ButtonInteractable>();
            _button.ShowButton();
        }
        else
        {
            if (_button)
            {
                _button.HideButton();
            }
        }
    }

    private void InteractRay(InputAction.CallbackContext context)
    {
        var r = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(r, out RaycastHit hitInfo, interactRange, interactableLayer))
        {
            if(hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
    }
    
}
