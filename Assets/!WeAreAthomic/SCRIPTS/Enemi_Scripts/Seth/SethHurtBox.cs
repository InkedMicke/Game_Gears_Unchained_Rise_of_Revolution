using Generics.Collision;
using System;

namespace Seth
{
    public class SethHurtBox : HurtBox
    {
        [NonSerialized] public float AcumulativeTakenHealth;

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            AcumulativeTakenHealth += damage;
        }
    }
}