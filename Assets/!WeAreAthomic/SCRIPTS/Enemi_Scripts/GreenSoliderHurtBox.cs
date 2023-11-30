using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

public class GreenSoliderHurtBox : MonoBehaviour, IDamageable
{
    public bool CheckIfPlayerIsNear(float radius)
    {
        return Physics.CheckSphere(transform.position, radius);
    }

    public void Damage(float damage)
    {

    }
}
