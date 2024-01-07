using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsControls : MonoBehaviour
{
    public TMP_Dropdown inputDropdown;
    public Slider sensivityXSlider;
    public Slider sensivityYSlider;

    public void ValueFromInputDropdown(int value)
    {
        if(value == 0)
        {
            GameManagerSingleton.Instance.typeOfInput = TypeOfInput.pc;
        }
        else
        {
            GameManagerSingleton.Instance.typeOfInput = TypeOfInput.gamepad;
        }
    }
}
