using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GSetSensivity : MonoBehaviour
{
    public GameManagerSingleton.Settings settings;

    public void GetDataFromSlider(float value)
    {
        var x = value * GameManagerSingleton.Instance.maxSensivity / 100;
        GameManagerSingleton.Instance.EnumToValue(settings, (int)x);
    }
}
