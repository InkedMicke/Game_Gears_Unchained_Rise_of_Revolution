using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBright : MonoBehaviour
{

    [SerializeField] private OptionsScreen _optionsScreen;

    public void SetBrightn(float value)
    {
        _optionsScreen.SetBrightness(value);
    }
}
