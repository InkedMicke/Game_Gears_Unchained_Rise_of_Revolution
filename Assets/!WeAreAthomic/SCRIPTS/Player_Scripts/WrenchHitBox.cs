using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class WrenchHitBox : MonoBehaviour
    {
        private MainCAttack _mainCAttack;

        [SerializeField] private PlayerDamageData wrenchDamageData;

        public List<Collider> colliderList = new();

        public bool GotHit;

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
                    foreach (var collider in colliderList)
                    {
                        if(collider == other)
                        {
                            return;
                        }
                    }
                    damageable.Damage(GameManagerSingleton.Instance.GetPlayerDamage(wrenchDamageData, other.gameObject));
                    colliderList.Add(other);
                }

                _mainCAttack.MoveToEnemy(other);
            }
        }

        public void ClearList()
        {
            colliderList.Clear();
        }

        public void SetGotHit(bool gotHit)
        {
            GotHit = gotHit;
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