using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace Generics.Collision
{
    public class HitBox : MonoBehaviour
    {
        [NonSerialized] protected List<Collider> colList = new();

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

        public virtual void DoDamage(float damage, Collider col)
        {
            if (col.TryGetComponent(out HurtBox hurtbox))
            {
                if (!colList.Any(x => x == col))
                {
                    hurtbox.GetDamage(damage);
                    colList.Add(col);
                }
            }
        }

        public void ClearList()
        {
            colList.Clear();
        }
    }
}