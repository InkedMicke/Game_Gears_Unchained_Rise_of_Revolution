using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] float _height;
    [SerializeField] float _castRadius = 0.49f;
    [SerializeField] float _castLength = 0.1f;
    [SerializeField] float _maxAngle = 45f;
    Vector3 _downDir = Vector3.down;

    [SerializeField] LayerMask _groundMask = int.MinValue;


    public Vector3 GroundNormal => _groundNormal;
    Vector3 _groundNormal;
    public bool Grounded => _grounded;
    bool _grounded;

    public Vector3 SlideNormal => _slideNormal;
    Vector3 _slideNormal;
    public bool Sliding => _sliding;
    bool _sliding;

    private void OnValidate()
    {
        _downDir = _downDir.normalized;
    }


    private void FixedUpdate()
    {
        if (Physics.SphereCast(transform.position + transform.up * _height, _castRadius, -transform.up, out RaycastHit hitInfo, _castLength, _groundMask))
        {
            if (Vector3.Dot(hitInfo.normal, -_downDir) > Mathf.Sin((90f - _maxAngle) * Mathf.PI / 180f))
            {
                _groundNormal = hitInfo.normal;
                _grounded = true;
                _sliding = false;
                Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.blue, Time.fixedDeltaTime);
                _slideNormal = hitInfo.normal;
            }
            else
            {
                _grounded = false;
                _sliding = true;
                _slideNormal = hitInfo.normal;
            }
        }
        else
        {
            _grounded = false;
            _groundNormal = Vector3.up;
            _sliding = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.up * _height, _castRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((transform.position + (transform.up * _height)) + -transform.up * _castLength, _castRadius);
    }
}
