using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRotateX : MonoBehaviour
{
    [SerializeField] private float rotateAmount;
    void Update()
    {
        transform.Rotate(rotateAmount, transform.position.x, transform.position.z);
    }
}
