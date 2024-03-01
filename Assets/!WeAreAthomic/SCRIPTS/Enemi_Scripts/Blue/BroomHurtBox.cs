using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System;
using UnityEngine;

namespace Broom
{
    public class BroomHurtBox : MonoBehaviour, IDamageable
    {
        Broom m_broom;

        [NonSerialized] public bool CanReceiveDamage;

        private int hurtedTimes;

        [SerializeField] float maxHealth;
        [NonSerialized] public float CurrentHealth;

   

        private void Awake()
        {
            CurrentHealth = maxHealth;
            m_broom = GetComponentInParent<Broom>();
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
                if (!m_broom.IsAttacking)
                {
                    hurtedTimes++;
                    Debug.Log(hurtedTimes);
                    if (hurtedTimes < 4)
                    {
                        CancelInvoke(nameof(ResetHurtedTimes));
                        m_broom.broomDefense.Defense();
                        m_broom.broomVFX.PlayProtectedEffect();
                        Invoke(nameof(ResetHurtedTimes), 3f);
                    }
                    else
                    {
                        m_broom.broomDefense.CrossAttack();
                        ResetHurtedTimes();
                    }
                }
            }
        }

        private void ResetHurtedTimes()
        {
            Debug.Log("hola1");
            hurtedTimes = 0;
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
