using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System;
using UnityEngine;

namespace Broom
{
    public class BroomHurtBox : Broom, IDamageable
    {

        [NonSerialized] public bool CanReceiveDamage;

        private int hurtedTimes;

        [SerializeField] float maxHealth;
        [NonSerialized] public float CurrentHealth;

   

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Damage(float dmg)
        {
            if (CanReceiveDamage)
            {
                if (!CheckForDeath())
                {
                    CurrentHealth -= dmg;
                }
            }
            else
            {
                hurtedTimes++;
                if (hurtedTimes < 3)
                {
                    CancelInvoke(nameof(ResetHurtedTimes));
                    _broomDefense.Defense();
                    Invoke(nameof(ResetHurtedTimes), 1f);
                }
                else
                {
                    _broomDefense.CrossAttack();
                }
            }
        }

        private void ResetHurtedTimes() => hurtedTimes = 0;

        private bool CheckForDeath()
        {
            if (CurrentHealth <= 0)
            {
                return true;
            }

            return false;
        }

        public void SetCanReceiveDamage(bool canReceiveDamage)
        {
            CanReceiveDamage = canReceiveDamage;
        }
    }
}
