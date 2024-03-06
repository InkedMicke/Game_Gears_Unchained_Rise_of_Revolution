using Interfaces;
using UnityEngine;

namespace Generics.Collision
{
    public class HurtBox : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealth;
        public float CurrentHealth;

        protected virtual void Awake()
        {
            CurrentHealth = maxHealth;
        }

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

        public virtual void GotEnterCollision(Collider col)
        {

        }        
        
        public virtual void GotStayCollision(Collider col)
        {

        }        
        
        public virtual void GotExitCollision(Collider col)
        {

        }

        public virtual void GetDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        public virtual bool CanReceiveDamage()
        {
            return true;
        }

        public bool IsDeath()
        {
            return CurrentHealth <= 0;
        }

    }
}
