using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GGenerator : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent itActivatesWhenInteract;
        public void Interact()
        {
            itActivatesWhenInteract.Invoke();
        }
    }
}
