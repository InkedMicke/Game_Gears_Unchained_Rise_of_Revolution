using _WeAreAthomic.SCRIPTS.Props;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using Unity.VisualScripting;
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

    private Transform playerTr => GameObject.FindGameObjectWithTag("Player").transform;

    [SerializeField] private float interactRange = 5f;
    [SerializeField] private float interactRangeSoldier = 2f;

    [System.NonSerialized] public bool isSeeing;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.PlayerPC.Interact.performed += InputPC;
        _playerInputActions.PlayerGamepad.Interact.performed += InputGamepad;
    }

    private void FixedUpdate()
    {
        Ray();
    }

    void Ray()
    {
        var r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out var hitInfo, interactRange, interactableLayer))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable _))
            {
                _button = hitInfo.collider.GetComponent<ButtonInteractable>();

                if (_button.typeOfHacked == TypeOfHacked.soldier)
                {
                    if(Vector3.Distance(playerTr.position, _button.transform.position) < interactRangeSoldier)
                    {
                        _button.ShowButton();
                    }
                }
                else
                {
                    _button.ShowButton();
                }
                
            }


        }
        else
        {
            if (_button)
            {
                _button.HideButton();
            }
        }
    }

    private void InputPC(InputAction.CallbackContext context)
    {
        if(GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
        {
            InteractRay();
        }
    }

    private void InputGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
        {
            InteractRay();
        }
    }

    private void InteractRay()
    {
        var r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out var hitInfo, interactRange, interactableLayer))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                _button = hitInfo.collider.GetComponent<ButtonInteractable>();

                if (_button.typeOfHacked == TypeOfHacked.soldier)
                {
                    if (Vector3.Distance(playerTr.position, _button.transform.position) < interactRangeSoldier)
                    {
                        interactObj.Interact();
                    }
                }
                else
                {
                    interactObj.Interact();
                }

            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
    }

}
