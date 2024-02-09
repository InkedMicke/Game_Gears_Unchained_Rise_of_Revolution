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

        private void OnTriggerStay(Collider other)
        {
            GotStayCollision(other);
        }

        private void OnTriggerExit(Collider other)
        {
            GotExitCollision(other);
        }

        protected virtual void GotEnterCollision(Collider col)
        {

        }        
        
        protected virtual void GotStayCollision(Collider col)
        {

        }        
        
        protected virtual void GotExitCollision(Collider col)
        {

        }

        public virtual void Damage(float damage)
        {

        }

    }
}
