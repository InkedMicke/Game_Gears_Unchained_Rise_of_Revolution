using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
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
}
