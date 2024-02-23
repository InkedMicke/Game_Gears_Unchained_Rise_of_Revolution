using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System;
using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomHurtBox : Broom, IDamageable
    {
        private Coroutine m_c_waitForEndShield;

        [NonSerialized] public bool CanReceiveDamage;

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
                broomAnimator.SetShieldCount(1);
            }
        }

        private IEnumerator WaitForDisableShield()
        {
            yield return new WaitForSeconds(1f);
        }

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
