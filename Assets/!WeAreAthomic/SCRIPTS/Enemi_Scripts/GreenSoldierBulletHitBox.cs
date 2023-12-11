using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{

    public class GreenSoldierBulletHitBox : MonoBehaviour
    {
        [SerializeField] private float damage = 5;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage);
            }
            Destroy(gameObject);
        }
    }
}