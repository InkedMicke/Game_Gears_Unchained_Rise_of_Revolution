using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GGenerator : MonoBehaviour, IInteractAttack
    {
        [SerializeField] private UnityEvent itActivatesWhenInteract;

        public void InteractAttack()
        {
            itActivatesWhenInteract.Invoke();
        }
    }
}
