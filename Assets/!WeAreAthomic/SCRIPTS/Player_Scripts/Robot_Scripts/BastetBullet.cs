using System.Collections;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

public class BastetBullet : MonoBehaviour
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
                    damageable.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(damageData, other.gameObject));
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
