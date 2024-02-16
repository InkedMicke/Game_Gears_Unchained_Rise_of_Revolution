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

    [SerializeField] private GameObject _imagePC;
    [SerializeField] private GameObject _imageGP;

    public void ValueFromInputDropdown(int value)
    {
        if(value == 0)
        {
            GameManagerSingleton.Instance.typeOfInput = TypeOfInput.pc;
            _imagePC.SetActive(true);
            _imageGP.SetActive(false);
        }
        else
        {
            GameManagerSingleton.Instance.typeOfInput = TypeOfInput.gamepad;
            _imageGP.SetActive(true);
            _imagePC.SetActive(false);
        }
    }
}
