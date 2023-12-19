using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class SoldierHealthManager : MonoBehaviour, IDamageable
    {
        private GreenSoliderMovement _greenMove;
        
       [SerializeField] private C_DisolveEnemi _disolveEnemi;

        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject soldierWithoutBones;
        [SerializeField] private GameObject healthSliderObj;
        [SerializeField] private GameObject botonSoldier;

        public bool IsDeath;

        public float maxHealth = 100f;
        public float currentHealth = 0f;

        private void Awake()
        {
            _greenMove = GetComponentInParent<GreenSoliderMovement>();
            

            currentHealth = maxHealth;
            SetMaxhealthSlider(maxHealth);
            SetHealthSlider(currentHealth);
        }

        public void Damage(float damage)
        {
            currentHealth -= damage;
            SetHealthSlider(currentHealth);
            CheckForDeath();
            _greenMove.SetChasePlayer(true);
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
            _greenMove.DisableMovement();
            botonSoldier.SetActive(false);
            _disolveEnemi.StartDisolving();
          
            
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
