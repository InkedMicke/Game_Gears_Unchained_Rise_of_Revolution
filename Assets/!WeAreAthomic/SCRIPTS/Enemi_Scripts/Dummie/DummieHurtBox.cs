using Generics;
using Interfaces;
using System;
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

        [SerializeField] private bool useKnockback = true;
        public bool isDeath;
        
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
            _gHealthManager.currentHealth -= value;
            _gHealthManager.SetSlider(_gHealthManager.currentHealth);
            //_anim.SetTrigger(string.Format("isHurt"));
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

        private void CheckForDeath()
        {
            if (_gHealthManager.currentHealth <= 0)
            {
                OnDeath?.Invoke();
                DummieDies.Invoke();
                isDeath = true;
                _anim.SetBool(string.Format("isDeath"), true);
            }
        }

        public bool CanReceiveDamage()
        {
            return true;
        }
    }
}
