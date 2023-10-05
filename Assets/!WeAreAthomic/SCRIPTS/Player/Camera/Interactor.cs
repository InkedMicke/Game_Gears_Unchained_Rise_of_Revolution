using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Interact();
}


public class Interactor : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

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
