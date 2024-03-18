using UnityEngine;
using UnityEngine.UI;

public class SliderHealthManager : MonoBehaviour
{
    [SerializeField] HealthManagerSO managerSO;
    [SerializeField] Slider slider;

    private void Awake()
    {
        managerSO.healthChangeEvent += UpdateSliderValue;

    }

    public void UpdateSliderValue(float value)
    {
        slider.value = value;
    }
}
