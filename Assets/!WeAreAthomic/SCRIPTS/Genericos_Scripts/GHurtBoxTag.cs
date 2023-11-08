using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
    [System.Serializable]
    public struct DetectionInfoTag
    {
        public bool enabled;
        public List<string> detectionStrings;
        public UnityEvent eventsToActivate;
        public string additionalInfo;
    }

    public class GHurtBoxTag : MonoBehaviour
    {
        [Header("Collision Detection")]
        [SerializeField] private List<DetectionInfoTag> collisionDetection;
        [SerializeField] private bool useTargetScript; // Indica si se utiliza el targetScript
        [SerializeField] private MonoBehaviour targetScript; // Script de destino para enviar información

        private void OnTriggerEnter(Collider other)
        {
            PerformCollisionDetection(other, DetectionType.Enter);
        }

        private void OnTriggerStay(Collider other)
        {
            PerformCollisionDetection(other, DetectionType.Stay);
        }

        private void OnTriggerExit(Collider other)
        {
            PerformCollisionDetection(other, DetectionType.Exit);
        }

        private void PerformCollisionDetection(Collider other, DetectionType detectionType)
        {
            foreach (var detectionInfo in collisionDetection)
            {
                if (detectionInfo.enabled && detectionInfo.detectionStrings.Contains(GetDetectionString(other, detectionType)))
                {
                    detectionInfo.eventsToActivate.Invoke();
                    if (useTargetScript && targetScript != null)
                    {
                        targetScript.SendMessage("OnCollisionDetected", new CollisionData(other, detectionInfo.additionalInfo));
                    }
                }
            }
        }

        private string GetDetectionString(Collider other, DetectionType detectionType)
        {
            if (detectionType == DetectionType.Enter)
            {
                return other.gameObject.tag;
            }
            else if (detectionType == DetectionType.Stay)
            {
                return other.gameObject.tag + "_stay";
            }
            else if (detectionType == DetectionType.Exit)
            {
                return other.gameObject.tag + "_exit";
            }

            return "";
        }
    }

    public enum DetectionType
    {
        Enter,
        Stay,
        Exit
    }

    public class CollisionData
    {
        public Collider collider;
        public string additionalInfo;

        public CollisionData(Collider collider, string additionalInfo)
        {
            this.collider = collider;
            this.additionalInfo = additionalInfo;
        }
    }
}