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
        [SerializeField] private float distance;
        [SerializeField] private LayerMask colLayers;

        // Use this for initialization
        void Awake()
        {
            var localPosition = transform.localPosition;
            dollyDir = localPosition.normalized;
            distance = localPosition.magnitude;
        }

        // Update is called once per frame
        void Update()
        {

            var desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
            if (Physics.Linecast(transform.parent.position, desiredCameraPos, out var hit, colLayers))
            {
                distance = Mathf.Clamp((hit.distance * 0.87f), minDistance, maxDistance);
                Debug.Log(hit.collider.gameObject.name);
            }
            else
            {
                distance = maxDistance;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
        }
    }
}