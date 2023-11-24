using Hedenrag.ExVar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void InteractAttack();
}

public class GBullet : MonoBehaviour
{
    private Rigidbody _rb;

    public float bulletForce = 50f;

    //[NonSerialized]public Optional<Transform> target;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable interactableAttack))
        {
            interactableAttack.InteractAttack();
        }

        Destroy(gameObject);
    }

    private void Update()
    {
/*        if (target)
        {
            var dir = (target.Value.position + Vector3.up * 0.5f - transform.position).normalized;
            _rb.velocity = _rb.velocity.magnitude * dir;
        }*/
    }
}
