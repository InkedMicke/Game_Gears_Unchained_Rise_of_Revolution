using UnityEngine;
using Generics.Collision;

namespace Enemy.Orange
{
    public class OrangeAreaAttackHitBox : HitBox
    {
        [SerializeField] PlayerDamageData damageData;
        public override void GotEnterCollision(Collider collision)
        {
            DoDamage(GameManagerSingleton.Instance.GetPlayerDamage(damageData, collision.gameObject), collision);
        }
    }
}