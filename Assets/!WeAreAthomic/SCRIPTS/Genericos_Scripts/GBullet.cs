using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBullet : MonoBehaviour
{
    private Rigidbody _rb;
    
    public float bulletForce = 50f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
    }
    
}
