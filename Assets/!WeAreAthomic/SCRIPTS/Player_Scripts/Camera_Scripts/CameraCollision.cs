using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts
{
    public class CameraCollision : MonoBehaviour
    {
        [SerializeField] Transform cameraTr;

        [SerializeField] private float minDistance = 1.0f;
        [SerializeField] private float maxDistance = 4.0f;
        [SerializeField] private float smooth = 10.0f;
        private Vector3 dollyDir;
        [SerializeField] private Vector3 dollyDirAdjusted;
        Vector3 currentHitDistance;
        private float m_distance;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask colLayers;

        // Use this for initialization
        void Awake()
        {
            var localPosition = transform.localPosition;
            dollyDir = localPosition.normalized;
            m_distance = localPosition.magnitude;
        }


        void Update()
        {
            /*            {
                            if (Physics.Raycast(new Ray(transform.parent.position, -transform.forward), out var hit, maxDistance, colLayers))
                                m_distance = hit.distance;
                        }
                        {
                            if (Physics.SphereCast(new Ray(-transform.parent.forward * m_distance, -transform.forward), radius, out var hit, maxDistance, colLayers))
                                m_distance = hit.distance;
                            else
                                m_distance = maxDistance;
                        }
                        transform.localPosition = dollyDir * m_distance;*/

            if (Physics.SphereCast(new Ray(transform.position, -transform.forward), radius, out var hit, maxDistance, colLayers))
                cameraTr.localPosition = Vector3.back * hit.distance;
            else
                cameraTr.localPosition = Vector3.back * maxDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + -transform.forward * m_distance, radius);
        }
    }
}