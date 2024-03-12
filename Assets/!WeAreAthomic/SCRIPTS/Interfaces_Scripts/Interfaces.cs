using System;

namespace Interfaces
{
    public interface IDamageable
    {
        public bool CanReceiveDamage();
        public void GetDamage(float value);
    }
}
