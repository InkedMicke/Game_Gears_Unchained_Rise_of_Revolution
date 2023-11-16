using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GSumMenuValues : MonoBehaviour
{
    [SerializeField] private TMP_InputField fieldValue;

    public void SumValues(int value)
    {
        if (int.TryParse(fieldValue.text, out int currentValue))
        {
            // Si la conversi�n a int fue exitosa, realiza la suma
            currentValue += value;
            fieldValue.text = currentValue.ToString();
        }
    }
}
