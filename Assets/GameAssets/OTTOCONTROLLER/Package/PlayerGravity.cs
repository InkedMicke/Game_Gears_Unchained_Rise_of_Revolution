using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundDetection))]
public class PlayerGravity : MonoBehaviour
{
    [SerializeField] float _gravityForce = 9.81f;
    public Vector3 GravityDir { get { return _gravityDir; } set { _gravityDir = value.normalized; } }
    Vector3 _gravityDir = Vector3.down;

    [SerializeField, HideInInspector] Rigidbody _rb;
    [SerializeField, HideInInspector] GroundDetection _groundDetection;

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDetection = GetComponent<GroundDetection>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (_groundDetection.Grounded)
        {
            _rb.AddForce(-_groundDetection.GroundNormal * _gravityForce, ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(_gravityDir * _gravityForce, ForceMode.Acceleration);
        }
    }
}
