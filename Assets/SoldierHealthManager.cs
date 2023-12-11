using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class SoldierHealthManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject soldierWithoutBones;
        [SerializeField] private GameObject healthSliderObj;

        public bool IsDeath;

        public float maxHealth = 100f;
        public float currentHealth = 0f;

        private void Awake()
        {
            currentHealth = maxHealth;
            SetMaxhealthSlider(maxHealth);
            SetHealthSlider(currentHealth);
        }

        public void Damage(float damage)
        {
            currentHealth -= damage;
            SetHealthSlider(currentHealth);
            CheckForDeath();
        }

        private void CheckForDeath()
        {
            if (currentHealth <= 0)
            {
                if (!IsDeath)
                {
                    IsDeath = true;
                    Death();
                }
            }
        }

        private void Death()
        {
            mesh.SetActive(false);
            healthSliderObj.SetActive(false);
            soldierWithoutBones.SetActive(true);
        }

        public void SetMaxhealthSlider(float maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        public void SetHealthSlider(float health)
        {
            healthSlider.value = health;
        }
    }
}
