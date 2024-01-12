using System.Collections;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

public class BastetBulletHitBox : MonoBehaviour
{
    public PlayerDamageData damageData;
    [SerializeField] private GameObject hitChispas;

    private void Start()
    {
        StartCoroutine(WaitUntilDestroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(hitChispas, transform.position, Quaternion.identity);
        if (other.TryGetComponent(out IDamageable damageable))
        {
            if (damageData != null)
            {
                damageable.Damage(GameManagerSingleton.Instance.GetDamage(damageData, other.gameObject));
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

    private IEnumerator WaitUntilDestroy()
    {
        yield return new WaitForSeconds(10f);

        Destroy(gameObject);
    }

}
