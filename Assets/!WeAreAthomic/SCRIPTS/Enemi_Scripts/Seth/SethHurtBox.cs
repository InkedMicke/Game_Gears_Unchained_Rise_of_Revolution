using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SethHurtBox : HurtBox
{
    [SerializeField] private Slider healthSlider;

    private bool IsDeath;

    [SerializeField] private float maxHealth;
    public float CurrentHealth;
    [NonSerialized] public float AcumulativeTakenHealth;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public override void Damage(float damage)
    {
        CurrentHealth -= damage;
        AcumulativeTakenHealth += damage;
        healthSlider.value = CurrentHealth;
        CheckForDeath();

        base.Damage(damage);
    }

    private void CheckForDeath()
    {
        if (CurrentHealth <= 0)
        {
            IsDeath = true;
        }
    }
}
