using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCRagdoll : MonoBehaviour
{

    private Rigidbody[] _rbs;

    public List<Rigidbody> prueba;

    public bool ragDoll;

    private void Start()
    {
        _rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in _rbs)
        {
            prueba.Add(rb);
        }

        SetEnabled(false);
    }

    private void Update()
    {
        if(ragDoll)
        {
            SetEnabled(true);
        }
    }

    public void SetEnabled(bool enabled)
    {
        var isKinematic = !enabled;
        foreach(var rb in _rbs)
        {
            rb.isKinematic = isKinematic;
        }
    }
}
