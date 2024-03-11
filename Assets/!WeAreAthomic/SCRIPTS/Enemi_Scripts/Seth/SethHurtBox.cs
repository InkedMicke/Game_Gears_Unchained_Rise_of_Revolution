using Generics.Collision;
using Interfaces;
using System;

namespace Seth
{
    public class SethHurtBox : HurtBox
    {
        [NonSerialized] public float AcumulativeTakenHealth;

        bool m_canReceiveDamage;

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            AcumulativeTakenHealth += damage;
        }

        public void SetCanReceiveDamage(bool canReceiveDamage)
        {
            m_canReceiveDamage = canReceiveDamage;
        }

        public override bool CanReceiveDamage()
        {
            return m_canReceiveDamage;
        }
    }
}