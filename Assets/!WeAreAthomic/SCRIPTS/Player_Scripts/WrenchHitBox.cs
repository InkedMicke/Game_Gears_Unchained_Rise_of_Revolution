using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Enemi;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class WrenchHitBox : MonoBehaviour
    {
        private MainCAttack _mainCAttack;

        [SerializeField] private PlayerDamageData wrenchDamageData;

        public List<Collider> colliderList = new();

        private void Awake()
        {
            _mainCAttack = GetComponentInParent<MainCAttack>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (wrenchDamageData != null)
                {
                    damageable.Damage(GameManagerSingleton.Instance.GetDamage(wrenchDamageData, other));
                    colliderList.Add(other);
                }
            }
        }

        public void ClearList()
        {
            colliderList.Clear();
        }

        private void SpeedUpTime()
        {
            Time.timeScale = 1f;
        }

        private void SpeedDownTime()
        {
            Time.timeScale = 0.1f;
        }
    }
}