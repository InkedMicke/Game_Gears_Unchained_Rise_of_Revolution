using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLookAtLockedY : MonoBehaviour
{
    private Transform mLookAt;

    void Start()
    {
        mLookAt = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        var desiredPos = new Vector3(mLookAt.position.x, transform.position.y, mLookAt.position.z);
        transform.LookAt(desiredPos);
    }
}
