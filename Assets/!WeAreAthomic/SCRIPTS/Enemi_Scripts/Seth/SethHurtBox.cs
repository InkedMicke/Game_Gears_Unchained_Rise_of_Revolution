using Generics.Collision;
using Interfaces;
using System;
using UnityEngine;

namespace Seth
{
    public class SethHurtBox : HurtBox
    {
        Seth seth;

        [SerializeField] HealthManagerSO healthManagerSO;

        bool m_canReceiveDamage;

        private void Awake()
        {
            seth = GetComponentInParent<Seth>();

            seth.OnPushBack += DisableReceiveDamage;
            seth.OnEnemiesDead += EnableReceiveDamage;
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            healthManagerSO.healthChangeEvent(damage);
        }


        void EnableReceiveDamage() => healthManagerSO.CanReceiveDamage = true;
        void DisableReceiveDamage() => healthManagerSO.CanReceiveDamage = false;
    }
}