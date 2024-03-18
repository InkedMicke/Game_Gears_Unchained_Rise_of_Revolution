using UnityEngine;
using Generics.Collision;

namespace Enemy.Orange
{
    public class OrangeAreaAttackHitBox : HitBox
    {
        [SerializeField] PlayerDamageData damageData;
        protected override void GotEnterCollision(Collider collision)
        {
            DoDamage(GameManagerSingleton.Instance.GetPlayerDamage(damageData, collision.gameObject), collision);
        }
    }
}