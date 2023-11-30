using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GSetSettings : MonoBehaviour
{
    public GameManagerSingleton.Settings settings;

    public void GetDataFromSlider(float value)
    {
        GameManagerSingleton.Instance.EnumToValue(settings, (int)value);
    }
}
