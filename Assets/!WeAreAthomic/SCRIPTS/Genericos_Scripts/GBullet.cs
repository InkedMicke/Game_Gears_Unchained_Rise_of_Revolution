using Hedenrag.ExVar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractAttack
{
    public void InteractAttack();
}

public class GBullet : MonoBehaviour
{
    private Rigidbody _rb;
    private BastetBulletHitBox _bulletHurtBox;

    public float bulletForce = 50f;

    [NonSerialized]public Optional<Transform> target;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
       if (target)
        {
            var dir = (target.Value.position + Vector3.up * 0.5f - transform.position).normalized;
            _rb.velocity = _rb.velocity.magnitude * dir;
        }
    }
}
