using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class HurtBox : MonoBehaviour, IDamageable
    {
        private void OnTriggerEnter(Collider other)
        {
            GotEnterCollision(other);
        }

        protected virtual void GotEnterCollision(Collider col)
        {

        }

        public virtual void Damage(float damage)
        {

        }

    }
}
