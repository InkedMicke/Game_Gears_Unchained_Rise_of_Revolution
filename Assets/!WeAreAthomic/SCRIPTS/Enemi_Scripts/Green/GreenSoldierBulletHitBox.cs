using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Green
{

    public class GreenSoldierBulletHitBox : MonoBehaviour
    {
        [SerializeField] private float damage = 5;
        [SerializeField] private GameObject hitChispas;
        private void OnTriggerEnter(Collider other)
        {
            Instantiate(hitChispas,transform.position,Quaternion.identity);
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage);
                
            }
           
            Destroy(gameObject);
        }
    }
}