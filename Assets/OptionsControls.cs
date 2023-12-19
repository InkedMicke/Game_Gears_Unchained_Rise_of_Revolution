using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsControls : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown inputDropdown;

    public void ValueFromInputDropdown(int value)
    {
        if(value == 0)
        {
            //GameManagerSingleton.Instance.typeOfInput = GameManagerSingleton.TypeOfInput.pc;
        }
        else
        {
            //GameManagerSingleton.Instance.typeOfInput = GameManagerSingleton.TypeOfInput.gamepad;
        }
    }
}
