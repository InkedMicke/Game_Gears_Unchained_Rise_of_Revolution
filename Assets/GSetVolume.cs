using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GSetVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeParam;

    public void GetDataFromSlider(float value)
    {
        audioMixer.SetFloat(volumeParam, Mathf.Log10(value) * 10f);
        Debug.Log(Mathf.Log10(value) * 20f);
    }
}
