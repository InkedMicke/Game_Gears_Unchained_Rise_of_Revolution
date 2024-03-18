using Interfaces;
using System;
using UnityEngine;

namespace Generics.Collision
{
    public class HurtBox : MonoBehaviour, IDamageable
    {
        public Action<float> OnGetDamage;

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
            
        }

        public virtual bool CanReceiveDamage()
        {
            return true;
        }
    }
}
