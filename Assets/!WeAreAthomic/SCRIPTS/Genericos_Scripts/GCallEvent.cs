using UnityEngine;
using UnityEngine.Events;

namespace Generics
{
    public class GCallEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent events;

        public void CallEvents()
        {
            events.Invoke();
        }
    }
}