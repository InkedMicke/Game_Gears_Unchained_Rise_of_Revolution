using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GUnityEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent events;

        [SerializeField] private bool onStart;
        void Start()
        {
            if (onStart)
            {
                events.Invoke();
            }
        }

        public void InvokeEvent()
        {
            events.Invoke();
        }
    }
}
