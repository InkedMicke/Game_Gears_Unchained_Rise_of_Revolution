using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class C_MaterialChangeOnDetection : MonoBehaviour
    {
        [SerializeField] private GameObject detectionDecal;
        [SerializeField] private Material neutralMat;
        [SerializeField] private Material warningMat;
        [SerializeField] private Material catchMat;
       
        public void PatrolDecal()
        {
            detectionDecal.GetComponent<DecalProjector>().material = neutralMat;
        }
        public void WarningDecal()
        {
            detectionDecal.GetComponent<DecalProjector>().material = warningMat;
        }
        public void CatchDecal()
        {
            detectionDecal.GetComponent<DecalProjector>().material = catchMat;
        }


    }

}

