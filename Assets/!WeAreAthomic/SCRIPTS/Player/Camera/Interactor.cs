using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    PlayerInputActions playerInputActions;

    public LayerMask interactableLayer;

    [SerializeField] private float interactRange = 5f;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += InteractSphere;
    }

    /*    void InteractRay(InputAction.CallbackContext context)
        {
            Ray r = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange, interactableLayer))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    Debug.Log("hola4");
                    interactObj.Interact();
                }
                else
                {
                    Debug.Log("no va");
                }
            }
        }*/

    void InteractSphere(InputAction.CallbackContext context)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
            }
        }
    }
}
