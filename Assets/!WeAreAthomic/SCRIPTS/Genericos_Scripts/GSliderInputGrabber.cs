using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GSliderInputGrabber : MonoBehaviour
{
    private Slider _slider;

    public MonoBehaviour targetScript;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SendDataToTargetScript()
    {
        targetScript.SendMessage("GetDataFromSlider", _slider.value);
    }
}
