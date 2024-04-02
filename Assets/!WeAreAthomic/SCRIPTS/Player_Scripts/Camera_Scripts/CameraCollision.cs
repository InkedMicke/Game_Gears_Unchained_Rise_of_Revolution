using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts
{
    public class CameraCollision : MonoBehaviour
    {
        [SerializeField] Transform playerTr;

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
            {
                if (Physics.Linecast(transform.parent.position, transform.position, out var hit, colLayers))
                {
                    currentHitDistance = hit.point;
                }
                else
                {
                    currentHitDistance = dollyDir * maxDistance;
                }
            }
            {
                if (Physics.SphereCast(new Ray(currentHitDistance, -transform.forward), radius, out var hit, maxDistance, colLayers))
                {
                    currentHitDistance = hit.point;
                }
                else
                {
                    currentHitDistance = dollyDir * maxDistance;
                }
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, currentHitDistance, Time.deltaTime * smooth);
            /*
                        var ray = new Ray(transform.position, -transform.forward);
                        if(Physics.SphereCast(ray, radius, out var hit, cameraDistance))
                        {
                            transform.localPosition = Vector3.back * hit.distance;
                        }
                        else
                        {
                            transform.localPosition = Vector3.back * cameraDistance;
                        }*/
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + -transform.forward * m_distance, radius);
        }
    }
}