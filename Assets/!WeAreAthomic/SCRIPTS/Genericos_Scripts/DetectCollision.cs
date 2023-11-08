using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectCollision : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> OnCollisionDetected;

    private void OnTriggerEnter(Collider other)
    {
        OnCollisionDetected.Invoke(other);
    }
}
