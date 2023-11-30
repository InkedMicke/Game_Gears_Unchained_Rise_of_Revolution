using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GCallEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent events;

    public void CallEvents()
    {
        events.Invoke();
    }
}
