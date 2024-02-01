using System.Collections.Generic;
using System;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class HitBox : MonoBehaviour
    {
        [SerializeField] protected int damage;

        [SerializeField] private bool useDamageData = true;

        [SerializeField] protected EnemyDamageData damageData;

        [NonSerialized] protected List<Collider> colList = new();

        private void OnTriggerEnter(Collider collision)
        {
            GotEnterCollision(collision);
        }

        public virtual void GotEnterCollision(Collider collision)
        {
            if (collision.TryGetComponent(out HurtBox hurtbox))
            {
                foreach (var collider in colList)
                {
                    if (collider == collision)
                    {
                        return;
                    }
                }
                if (useDamageData)
                {
                    hurtbox.Damage(GameManagerSingleton.Instance.GetEnemyDamage(damageData));
                }
                else
                {
                    hurtbox.Damage(damage);
                }
                colList.Add(collision);
            }
        }

        public void ClearList()
        {
            colList.Clear();
        }
    }
}