using _WeAreAthomic.SCRIPTS.Enemi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastetBulletHurtBox : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(WaitUntilDestroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<DummieHurtBox>()?.TakeDamage(5);
        Destroy(this.gameObject);
    }

    private IEnumerator WaitUntilDestroy()
    {
        yield return new WaitForSeconds(10f);

        Destroy(this.gameObject);
    }
}
