using Generics.Collision;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SethHurtBox : HurtBox
{
    [SerializeField] private Slider healthSlider;

    [NonSerialized] public float AcumulativeTakenHealth;

    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
        AcumulativeTakenHealth += damage;
        healthSlider.value = CurrentHealth;
    }
    
    public bool GetIsDeath()
    {
        return IsDeath();
    }
}
