using _WeAreAthomic.SCRIPTS.Enemi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damage(float value);
}

public class BastetBulletHitBox : MonoBehaviour
{
    public PlayerDamageData damageData;

    private void Start()
    {
        StartCoroutine(WaitUntilDestroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            if (damageData != null)
            {
                damageable.Damage(GameManagerSingleton.Instance.GetDamage(damageData, other));
            }
        }


        Destroy(gameObject);
    }

    private IEnumerator WaitUntilDestroy()
    {
        yield return new WaitForSeconds(10f);

        Destroy(gameObject);
    }

}
