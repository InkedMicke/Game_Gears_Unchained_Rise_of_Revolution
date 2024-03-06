using Generics.Collision;
using UnityEngine;

public class RedAttackHitBox : HitBox
{
    [SerializeField] EnemyDamageData damageData;
    protected override void GotEnterCollision(Collider collision)
    {
        DoDamage(GameManagerSingleton.Instance.GetEnemyDamage(damageData), collision);
    }


}
