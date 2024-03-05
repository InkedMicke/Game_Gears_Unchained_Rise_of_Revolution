using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Interfaces_Scripts
{
    public interface IDamageable
    {
        public void GetDamage(float value);
        public bool CanReceiveDamage();
    }
}
