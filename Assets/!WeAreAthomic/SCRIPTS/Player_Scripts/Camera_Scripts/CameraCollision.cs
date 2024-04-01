using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts
{
    public class CameraCollision : MonoBehaviour
    {

        [SerializeField] private float minDistance = 1.0f;
        [SerializeField] private float maxDistance = 4.0f;
        [SerializeField] private float smooth = 10.0f;
        private Vector3 dollyDir;
        [SerializeField] private Vector3 dollyDirAdjusted;
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

            /*            var desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
                        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out var hit, colLayers))
                        {
                            distance = Mathf.Clamp((hit.distance * 0.87f), minDistance, maxDistance);
                        }
                        else
                        {
                            distance = maxDistance;
                        }*/
            var ray = new Ray(transform.position, -transform.forward);
            if (Physics.SphereCast(ray, radius, out var hit, maxDistance, colLayers))
            {
                m_distance = hit.distance;
            }
            else
            {
                m_distance = maxDistance;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * m_distance, Time.deltaTime * smooth);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position - -transform.forward * maxDistance, radius);
        }
    }
}