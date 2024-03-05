using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitBoxAngleView : MonoBehaviour
{
    public List<Collider> colliderList = new();

    private void OnTriggerEnter(Collider other)
    {
        colliderList.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliderList.Remove(other);
    }

    public void RemoveDeadEnemies()
    {
        foreach (var e in colliderList.ToList())
        {
            if(e == null)
            {
                colliderList.Remove(e);
            }
            else if(e.GetComponentInChildren<DummieHurtBox>() != null && e.GetComponentInChildren<DummieHurtBox>().isDeath)
            {
                colliderList.Remove(e);
            }
            else if(e.GetComponent<SoldierHurtBox>() != null && e.GetComponent<SoldierHurtBox>().IsDeath)
            {
                colliderList.Remove(e);
            }
        }
    }
}
