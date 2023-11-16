using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSoldierBulletHurtBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var mainCHealth = other.GetComponent<MainCHealthManager>();
        mainCHealth.TakeDamage(5f);
        Destroy(gameObject);
    }
}
