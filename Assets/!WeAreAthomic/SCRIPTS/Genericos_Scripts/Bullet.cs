using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    public float bulletForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Debug.Log(bulletForce);
        rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
    }
}
