using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class HitBox : MonoBehaviour
    {
        [SerializeField] protected int damage;

        [SerializeField] protected EnemyDamageData damageData;

        private void OnTriggerEnter(Collider collision)
        {
            GotEnterCollision(collision);
        }

        public virtual void GotEnterCollision(Collider collision)
        {
            if (collision.TryGetComponent(out HurtBox hurtbox))
            {
                hurtbox.Damage(GameManagerSingleton.Instance.GetEnemyDamage(damageData));
            }
        }
    }
}