using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GStartEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent startEvent;

    private void Start()
    {
        startEvent.Invoke();
    }
}
