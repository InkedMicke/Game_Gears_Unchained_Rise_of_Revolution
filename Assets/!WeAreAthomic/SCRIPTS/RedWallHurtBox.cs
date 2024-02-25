using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;

public class RedWallHurtBox : HurtBox, IInteractAttack
{
    GDestroyObject gDestroy;

    private void Awake()
    {
        gDestroy = GetComponent<GDestroyObject>();
    }
    public void InteractAttack()
    {
        gDestroy.DestroyThisObject(0F);
    }

    public override void GotEnterCollision(Collider col)
    {
        InteractAttack();
        base.GotEnterCollision(col);
    }
}
