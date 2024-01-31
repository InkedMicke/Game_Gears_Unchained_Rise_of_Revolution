using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SethHurtBox : HurtBox
{
    [SerializeField] private Slider healthSlider;

    private bool IsDeath;
    private bool _canReceiveDamage;

    [SerializeField] private float maxHealth;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public override void Damage(float damage)
    {
        if (_canReceiveDamage)
        {
            currentHealth -= damage;
            healthSlider.value = currentHealth;
            CheckForDeath();
        }
        base.Damage(damage);
    }

    private void CheckForDeath()
    {
        if(currentHealth <= 0)
        {
            IsDeath = true;
        }
    }
}
