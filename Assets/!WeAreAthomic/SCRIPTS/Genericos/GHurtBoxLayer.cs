using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DetectionInfoLayer
{
    public bool enabled;
    public LayerMask detectionLayer;
    public DetectionType detectionType; // Agregamos el tipo de detección
    public UnityEvent eventsToActivate;
    public string additionalInfo;
}

public class GHurtBoxLayer : MonoBehaviour
{
    [Header("Collision Detection")]
    [SerializeField] private List<DetectionInfoLayer> collisionDetection;
    [SerializeField] private bool useTargetScript;
    [SerializeField] private MonoBehaviour targetScript;

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
            if (detectionInfo.enabled &&
                ((detectionInfo.detectionLayer & (1 << other.gameObject.layer)) != 0) &&
                detectionInfo.detectionType == detectionType) // Comprobamos si la capa y el tipo de detección coinciden
            {
                detectionInfo.eventsToActivate.Invoke();
                if (useTargetScript && targetScript != null)
                {
                    targetScript.SendMessage("OnCollisionDetected", new CollisionData(other, detectionInfo.additionalInfo));
                }
            }
        }
    }
}

// Resto del script igual que el original