using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider healthSlider;

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
