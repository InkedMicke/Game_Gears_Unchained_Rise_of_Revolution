using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GHealthManager : MonoBehaviour
{
    public Slider slider;
    
    public float maxHealth;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxSlider(float value)
    {
        slider.maxValue = value;
    }

    public void SetSlider(float value)
    {
        slider.value = value;
    }
}
