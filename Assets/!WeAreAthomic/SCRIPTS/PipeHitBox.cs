using Generics.Collision;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class PipeHitBox : HitBox
    {
        [SerializeField] PlayerDamageData damageData;
        public override void GotEnterCollision(Collider collision)
        {
            DoDamage(GameManagerSingleton.Instance.GetPlayerDamage(damageData, collision.gameObject), collision);
        }
    }
}