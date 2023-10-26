using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GToggleVisible : MonoBehaviour
{
    public void ToggleVisible()
    {
        gameObject.SetActive(!gameObject);
        Debug.Log("hola");
    }

    public void DelayedToggleVisible(float value)
    {
        Invoke(nameof(ToggleVisible), value);
    }

}
