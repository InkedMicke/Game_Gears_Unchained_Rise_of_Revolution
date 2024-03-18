using Generics.Collision;
using UnityEngine;

namespace Enemy.Red
{
    public class RedAttackHitBox : HitBox
    {
        [SerializeField] EnemyDamageData damageData;
        protected override void GotEnterCollision(Collider collision)
        {
            DoDamage(GameManagerSingleton.Instance.GetEnemyDamage(damageData), collision);
        }
    }
}