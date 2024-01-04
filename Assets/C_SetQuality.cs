using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class C_SetQuality : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [SerializeField] private UniversalRenderPipelineAsset _urpQualityLow;
    [SerializeField] private UniversalRenderPipelineAsset _urpQualityMedium;
    [SerializeField] private UniversalRenderPipelineAsset _urpQualityHigh;
    [SerializeField] private UniversalRenderPipelineAsset _urpQualityUltra;

    public void SetQualityLevelDropdown(int index)
    {
        switch(index)
        {
            case 0:
                _urpQualityLow.shadowDistance = 20;
                break;

            case 1:
                _urpQualityMedium.shadowDistance = 50;
                break;
            case 2:
                _urpQualityHigh.shadowDistance = 75;
                break;
            case 3:
                _urpQualityUltra.shadowDistance = 100;
                break;
        }

        QualitySettings.SetQualityLevel(index, false);
    }
    
}
