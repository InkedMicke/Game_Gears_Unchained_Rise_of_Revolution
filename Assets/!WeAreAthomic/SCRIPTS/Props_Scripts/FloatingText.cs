using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 4, 0);

    private void Start()
    {

        transform.localPosition += offset;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }

}
