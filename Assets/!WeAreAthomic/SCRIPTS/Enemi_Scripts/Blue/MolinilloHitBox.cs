using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;

namespace Broom
{
    public class MolinilloHitBox : MonoBehaviour
    {
        [SerializeField] EnemyDamageData damageData;
        private void OnTriggerStay(Collider other)
        {
            if(TryGetComponent(out MainCHealthManager _hurtbox))
            {
                _hurtbox.Damage(GameManagerSingleton.Instance.GetEnemyDamage(damageData));
            }
        }
    }
}