using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    public void IsSeeing();
}

public class Interactor : MonoBehaviour
{
    PlayerInputActions playerInputActions;

    public LayerMask interactableLayer;

    [SerializeField] private float interactRange = 5f;

    public List<GameObject> gameObjs = new List<GameObject>();


    private void Awake()
    {
/*        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += InteractRay;*/
    }

    private void Update()
    {
        InteractRay();
    }

    void InteractRay()
    {
        Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange, interactableLayer, QueryTriggerInteraction.Collide))
        {
            Debug.Log("hola1");
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                Debug.Log("hola4");

            }
            else
            {
                Debug.Log("hola2");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
    }

    /*    void InteractSphere(InputAction.CallbackContext context)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }*/
}
