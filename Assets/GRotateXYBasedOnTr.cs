using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GRotateXYBasedOnTr : MonoBehaviour
{
    [SerializeField] private Transform target;
    void Update()
    {
        Vector3 newRotation = new Vector3(0f, target.eulerAngles.y, target.eulerAngles.z);
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
