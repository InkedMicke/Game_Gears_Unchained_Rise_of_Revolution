using UnityEngine;
using UnityEngine.UI;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCHealthManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Slider healthSlider;

        public float currentHealth = 50f;
        public float maxHealth = 100f;

        private void Start()
        {
            SetMaxHealthSlider();
            SetHealthSlider();
        }

        private void Update()
        {
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public void Damage(float damage)
        {
            currentHealth -= damage;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            Debug.Log("hola2");
            SetHealthSlider();
        }

        public void GetHealth(float health)
        {
            currentHealth += health;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            SetHealthSlider();
        }

        public void SetHealthSlider()
        {
            healthSlider.value = currentHealth;
        }

        private void SetMaxHealthSlider()
        {
            healthSlider.maxValue = maxHealth;
        }
    }
}
