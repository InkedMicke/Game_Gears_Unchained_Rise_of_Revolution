using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using UnityEngine;

public class GreenSoldierBulletHitBox : MonoBehaviour
{
    [SerializeField] private float damage = 5;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("hola1");
            damageable.Damage(damage);
        }
        Destroy(gameObject);
    }
}
