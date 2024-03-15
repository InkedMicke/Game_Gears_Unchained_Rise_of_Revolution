using Generics.Collision;
using Interfaces;
using System;
using UnityEngine;

namespace Seth
{
    public class SethHurtBox : HurtBox
    {
        Seth seth;

        public Action OnAcumulativeEvent;

        bool m_canReceiveDamage;

        public float TakenHealthToPush = 250;
        public float MaxHealth = 100;
        float m_currentHealth;
        float m_currentAcumulativeHealth;

        private void Awake()
        {
            seth = GetComponentInParent<Seth>();

            m_currentHealth = MaxHealth;

            seth.OnPushBack += DisableReceiveDamage;
            seth.OnEnemiesDead += EnableReceiveDamage;
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            DecreaseHealth(damage);
        }

        public void DecreaseHealth(float health)
        {
            if(m_canReceiveDamage)
            {
                m_currentHealth -= health;
                m_currentAcumulativeHealth += health;
                if(m_currentAcumulativeHealth >= 250)
                {
                    m_currentAcumulativeHealth = 0;
                    OnAcumulativeEvent?.Invoke();
                }
            }
        }

        void EnableReceiveDamage() => m_canReceiveDamage = true;
        void DisableReceiveDamage() => m_canReceiveDamage = false;
    }
}