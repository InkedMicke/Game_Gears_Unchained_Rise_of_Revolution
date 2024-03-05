using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Interfaces_Scripts
{
    public interface IDamageable
    {
        public void Damage(float value);
        public bool CanReceiveDamage();
    }
}
