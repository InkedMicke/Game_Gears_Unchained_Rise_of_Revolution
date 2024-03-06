using Player;
using System;
using UnityEngine;
using Generics.Collision;

namespace Broom
{
    public class MolinilloHitBox : HitBox
    {
        [SerializeField] EnemyDamageData damageData;

        [NonSerialized] public bool GotCollision;

        protected override void GotStayCollision(Collider col)
        {
            DoDamage(GameManagerSingleton.Instance.GetEnemyDamage(damageData), col);
        }

    }
}