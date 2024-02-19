using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class WrenchHitBox : MonoBehaviour
    {
        private MainCAttack _mainCAttack;
        [SerializeField] private MainCFuryAttack mainCFuryAttack;

        [SerializeField] private PlayerDamageData wrenchDamageData;
        [SerializeField] private List<ParticleSystem> _hitParticles;
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
                mainCFuryAttack.GetFury(mainCFuryAttack.furyPerHit);
                if (wrenchDamageData != null)
                {
                    foreach (var collider in colliderList)
                    {
                        if(collider == other)
                        {
                           
                            return;
                        }
                    }
                    HitParticlesInvoke();
                    damageable.Damage(GameManagerSingleton.Instance.GetPlayerDamage(wrenchDamageData, other.gameObject));
                    colliderList.Add(other);
                    
                }
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
        public void HitParticlesInvoke()
        {
            if (_hitParticles == null || _hitParticles.Count == 0)
            {
                return;
            }
            var random = Random.Range(0, _hitParticles.Count);

            var randomParticleSystem = _hitParticles[random];

            if (randomParticleSystem != null)
            {
                randomParticleSystem.Play();
            }


        }
    }
}