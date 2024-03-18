using Interfaces;
using UnityEngine;

namespace Enemy.Green
{

    public class GreenSoldierBulletHitBox : MonoBehaviour
    {
        [SerializeField] private float damage = 5;
        [SerializeField] private GameObject hitChispas;
        [SerializeField] private EnemyDamageData damageData;
        private void OnTriggerEnter(Collider other)
        {
            Instantiate(hitChispas,transform.position,Quaternion.identity);
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.GetDamage(GameManagerSingleton.Instance.GetEnemyDamage(damageData));
                
            }
            Destroy(gameObject);
        }
    }
}