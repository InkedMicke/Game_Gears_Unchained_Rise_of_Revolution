using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCRagdoll : MonoBehaviour
{

    private Rigidbody[] _rbs;

    public List<Rigidbody> prueba;

    private void Start()
    {
        _rbs = transform.GetComponentsInChildren<Rigidbody>();
        SetEnabled(false);
        foreach (var rb in _rbs)
        {
            prueba.Add(rb);
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
