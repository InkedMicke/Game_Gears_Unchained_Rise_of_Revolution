using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

public class GreenSoliderHurtBox : MonoBehaviour
{
    private SoldierHealthManager _healthManager;

    private void Awake()
    {
        _healthManager = GetComponent<SoldierHealthManager>();
    }

    public bool CheckIfPlayerIsNear(float radius)
    {
        return Physics.CheckSphere(transform.position, radius);
    }
}
