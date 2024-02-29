using System;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxAngleView : MonoBehaviour
{
    public List<Collider> colliderList = new();

    private void OnTriggerEnter(Collider other)
    {
        colliderList.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliderList.Remove(other);
    }
}
