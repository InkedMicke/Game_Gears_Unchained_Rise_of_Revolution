using System.Collections;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

public class BastetBullet : Bullet
{
    public PlayerDamageData damageData;
    [SerializeField] private GameObject hitChispas;

    [SerializeField] private bool useHitBox;

    private void Start()
    {
        StartCoroutine(WaitUntilDestroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useHitBox)
        {
            Instantiate(hitChispas, transform.position, Quaternion.identity);
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (damageData != null)
                {
                    damageable.Damage(GameManagerSingleton.Instance.GetPlayerDamage(damageData, other.gameObject));
                }
            }
            else
            {
                if (other.TryGetComponent(out IInteractAttack interactAttack))
                {
                    if (damageData != null)
                    {
                        interactAttack.InteractAttack();
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator WaitUntilDestroy()
    {
        while (this.gameObject != null)
        {
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
        }
    }

}
