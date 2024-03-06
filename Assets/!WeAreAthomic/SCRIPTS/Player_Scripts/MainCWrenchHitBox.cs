using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

namespace Player
{
    public class MainCWrenchHitBox : MonoBehaviour
    {
        [SerializeField] private MainCFuryAttack mainCFuryAttack;

        [SerializeField] private PlayerDamageData wrenchDamageData;
        [SerializeField] private List<ParticleSystem> _hitParticles;
        public List<Collider> colliderList = new();

        public bool GotHit;

        private void OnValidate()
        {
            mainCFuryAttack = GetComponentInParent<MainCFuryAttack>();
        }

        private void OnTriggerEnter(Collider other)
        {
            colliderList.Add(other);
        }

        public void ClearList()
        {
            colliderList.Clear();
        }

        public void SetGotHit(bool gotHit)
        {
            GotHit = gotHit;
        }

        public void ApplyDamage()
        {
            foreach (var col in colliderList)
            {
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    if (damageable.CanReceiveDamage())
                    {
                        damageable.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(wrenchDamageData, col.gameObject));
                        mainCFuryAttack.GetFury(mainCFuryAttack.furyPerHit);
                        HitParticlesInvoke();
                        GCameraShake.Instance.ShakeCamera(1f, 5f,.2f);
                    }
                    else
                    {
                        GCameraShake.Instance.ShakeCamera(.5f, 10f, .1f);
                        damageable.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(wrenchDamageData, col.gameObject));
                    }
                }

            }
            ClearList();
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