using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GSetMixerVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeParam;

    private void Start() 
    {
        audioMixer.SetFloat(volumeParam, GetComponent<Slider>().value);
    }

    public void GetDataFromSlider(float value)
    {
        audioMixer.SetFloat(volumeParam, value);
    }
}
