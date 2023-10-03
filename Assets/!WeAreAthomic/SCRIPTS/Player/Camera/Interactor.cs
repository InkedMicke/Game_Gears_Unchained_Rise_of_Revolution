using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    PlayerInputActions playerInputActions;

    public LayerMask interactableLayer;

    [SerializeField] private float interactRange = 5f;

    [System.NonSerialized] public bool isSeeing; 

    private void FixedUpdate()
    {
        InteractRay();
    }

    void InteractRay()
    {
        var r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange, interactableLayer, QueryTriggerInteraction.Collide))
        {
            isSeeing = true;
        }
        else
        {
            isSeeing = false;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
    }
    
}
