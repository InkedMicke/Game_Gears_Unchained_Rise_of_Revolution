using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDecalHurtBox : MonoBehaviour
{
    [NonSerialized] public bool IsPlayerInside;

    private void OnTriggerEnter(Collider other)
    {
        IsPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsPlayerInside = false;
    }
}
