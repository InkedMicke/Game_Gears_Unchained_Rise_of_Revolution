using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
    }
}
