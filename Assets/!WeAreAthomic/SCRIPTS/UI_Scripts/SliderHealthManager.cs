using UnityEngine;
using UnityEngine.UI;

public class SliderHealthManager : MonoBehaviour
{
    [SerializeField] HealthManagerSO managerSO;
    [SerializeField] Slider slider;

    private void Awake()
    {
        managerSO.healthChangeEvent += UpdateSliderValue;
        slider.maxValue = managerSO.MaxHealth;
    }

    public void UpdateSliderValue(float value)
    {
        slider.value = value;
    }
}
