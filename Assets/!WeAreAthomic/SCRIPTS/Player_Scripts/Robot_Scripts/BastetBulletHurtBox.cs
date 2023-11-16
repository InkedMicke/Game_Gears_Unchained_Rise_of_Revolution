using _WeAreAthomic.SCRIPTS.Enemi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastetBulletHurtBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        var dummie = other.GetComponent<DummieHurtBox>();
        dummie.TakeDamage(5);
        Destroy(this.gameObject);
    }
}
