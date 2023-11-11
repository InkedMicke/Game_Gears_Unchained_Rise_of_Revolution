using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GSliderInputGrabber : MonoBehaviour
{
    private Slider _slider;

    public GameManagerSingleton.Settings settings;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void AddValueToGameManager()
    {
        GameManagerSingleton.Instance.EnumToValue(settings, (int)_slider.value);
    }
}
