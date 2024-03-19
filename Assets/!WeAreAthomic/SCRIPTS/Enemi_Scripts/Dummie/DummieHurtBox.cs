using Generics;
using Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Dummie
{
    public class DummieHurtBox : MonoBehaviour, IDamageable
    {
        private GHealthManager _gHealthManager;
        private DummieController _dummieController;
        private Animator _anim;
        private DummieSounds _dummieSounds;
        private CapsuleCollider _cC;

        [System.NonSerialized] public Action OnDeath;

        [SerializeField] private LayerMask obstacles;

        [SerializeField] private GameObject soundComponentObj;

        [SerializeField] private ParticleSystem sparksHit;
        [SerializeField] private List<ParticleSystem> _hitParticles;
        [SerializeField] private GameObject espurnaFuryPrefab;

        [SerializeField] private bool useKnockback = true;
        public bool isDeath;
        bool m_canReceiveDamage = true;
        
        [SerializeField] private float pushForce = 5f;

        [SerializeField] private UnityEvent DummieDies;
    
        private void Awake()
        {
            _gHealthManager = GetComponentInParent<GHealthManager>();
            _anim = GetComponentInParent<Animator>();
            _dummieController = GetComponentInParent<DummieController>();
            _dummieSounds = soundComponentObj.GetComponent<DummieSounds>();
            _cC = GetComponent<CapsuleCollider>();
        }

        public void TakeDamage(float value)
        {
            if (!m_canReceiveDamage)
                return;
            HitParticlesInvoke();
            _gHealthManager.currentHealth -= value;
            _gHealthManager.SetSlider(_gHealthManager.currentHealth);
            _anim.SetTrigger(string.Format("isHurt"));
            sparksHit.Play();
            if (!isDeath)
            {
                if (useKnockback)
                {
                    _dummieController.StartPushBack();
                }

                _dummieSounds.PlayHurtSound();
            }

            CheckForDeath();
        }

        public void GetDamage(float value)
        {
            TakeDamage(value);
        }
        public void InstantiateEspurna()
        {
            Instantiate(espurnaFuryPrefab, transform.position, Quaternion.identity);
        }

        private void CheckForDeath()
        {
            if (_gHealthManager.currentHealth <= 0)
            {
                OnDeath?.Invoke();
                DummieDies.Invoke();
                isDeath = true;
                _anim.SetBool(string.Format("isDeath"), true);
                m_canReceiveDamage = false;
            }
        }

        public bool CanReceiveDamage()
        {
            return m_canReceiveDamage;
        }
        public void HitParticlesInvoke()
        {
            if (_hitParticles == null || _hitParticles.Count == 0)
            {
                return;
            }
            var random = UnityEngine.Random.Range(0, _hitParticles.Count);

            var randomParticleSystem = _hitParticles[random];

            if (randomParticleSystem != null)
            {
                randomParticleSystem.Play();
            }


        }
    }
}
