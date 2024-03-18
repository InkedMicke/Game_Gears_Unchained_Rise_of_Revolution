using UnityEngine;
using UnityEngine.Events;

namespace Generics
{
    public class GGenerator : MonoBehaviour
    {
        [SerializeField] private UnityEvent itActivatesWhenInteract;

        public void InteractAttack()
        {
            itActivatesWhenInteract.Invoke();
        }
    }
}
