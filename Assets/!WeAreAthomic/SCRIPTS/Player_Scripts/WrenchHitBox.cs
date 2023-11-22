using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Enemi;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class WrenchHitBox : MonoBehaviour
    {
        private MainCAttack _mainCAttack;

        [SerializeField] private LayerMask enemyHurtBox;

        public List<Collider> colliderList = new();

        private void Awake()
        {
            _mainCAttack = GetComponentInParent<MainCAttack>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((enemyHurtBox.value & (1 << other.gameObject.layer)) != 0)
            {
                if (!colliderList.Contains(other))
                {
                    other.GetComponent<DummieHurtBox>().TakeDamage(20);
                    SpeedDownTime();
                    Invoke(nameof(SpeedUpTime), .008f);
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