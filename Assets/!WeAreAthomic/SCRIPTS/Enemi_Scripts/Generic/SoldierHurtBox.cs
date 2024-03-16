using UnityEngine;
using Interfaces;
using System.Collections;
using Generics;
using System;
using System.Collections.Generic;

namespace Enemy
{
    public class SoldierHurtBox : MonoBehaviour, IDamageable
    {
        private SoldierHurtBox _soldierHurtbox;
        private GDestroyObject _destroyObject;
        private EnemyP _enemy;
        private C_MaterialChanger _materialChanger;
        private SoldierAnimator _soldierAnimator;

        [SerializeField] private C_DisolveEnemi _disolveEnemi;

        private Coroutine _waitingForHurtedCoroutine;
        private Coroutine _changingMaterialsCoroutine;

        [NonSerialized] public Action OnHurt;
        [NonSerialized] public Action OnDeath;
        [NonSerialized] public Action OnHurtedSmallerTwo;
        
        [SerializeField] private Material matSlider;
        [SerializeField] private GameObject healthSliderObj;

        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject soldierWithoutBones;
        
        [SerializeField] private GameObject botonSoldier;
        [SerializeField] private GameObject _hurtbox;

        [SerializeField] private GameObject decalAtackDir;
        [SerializeField] private GameObject decalPatrol;

        [SerializeField] private ParticleSystem _particlesHit;
        [SerializeField] private List<ParticleSystem> _hitParticles;

        public bool IsDeath;
        private bool _isWatingForHurtedtimes;
        private bool m_changingMaterials;

        public int HurtedTimes;

        public float maxHealth = 100f;
        public float currentHealth = 0f;

        private void Awake()
        {
            _soldierHurtbox = GetComponent<SoldierHurtBox>();
            _destroyObject = GetComponentInParent<GDestroyObject>();
            _enemy = GetComponentInParent<EnemyP>();
            _materialChanger = GetComponentInParent<C_MaterialChanger>();
            _soldierAnimator = GetComponentInParent<SoldierAnimator>();

            matSlider = new Material(matSlider);
            healthSliderObj.GetComponent<MeshRenderer>().material = matSlider;

            currentHealth = maxHealth;
            SetMaxhealthSlider(maxHealth);
            SetHealthSlider(currentHealth);
        }

        public void GetDamage(float damage)
        {
            HitParticlesInvoke();
            OnHurt?.Invoke();
            StartWaitForResetHutedTimes();
            HurtedTimes++;
            _particlesHit.Play();
            if(m_changingMaterials)
            {
                StopCoroutine(_changingMaterialsCoroutine);
            }
            _changingMaterialsCoroutine = StartCoroutine(HurtMaterialChange());
            if(HurtedTimes <= 2)
            {
                OnHurtedSmallerTwo?.Invoke();
            }
            currentHealth -= damage;
            SetHealthSlider(currentHealth);
            CheckForDeath();
            if (!_soldierHurtbox.IsDeath && !_enemy.IsAttacking && !_enemy.IsChasingPlayer)
            {
                _enemy.StartChasingPlayer();
                
            }
        }
            
        private void CheckForDeath()
        {
            if (currentHealth <= 0)
            {
                if (!_soldierHurtbox.IsDeath)
                {
                    _soldierHurtbox.SetDeath(true);
                    Death();
                }
            }
        }

        private void Death()
        {
            
            decalAtackDir.SetActive(false);
            decalPatrol.SetActive(false);
            mesh.SetActive(false);
            healthSliderObj.SetActive(false);
            soldierWithoutBones.SetActive(true);
            botonSoldier.SetActive(false);
            _destroyObject.DestroyThisObject(3f);
            _hurtbox.SetActive(false);
            OnDeath?.Invoke();
        }

        private IEnumerator HurtMaterialChange()
        {
            int index = 0;
            m_changingMaterials = true;
            _materialChanger.OnEnemiHit();
            yield return new WaitForSeconds(0.3f);
            _materialChanger.OnEnemiNeutral();
            while (index < 4)
            {
                _materialChanger.OnEnemiHit();
                yield return new WaitForSeconds(.1f);
                _materialChanger.OnEnemiNeutral();
                yield return new WaitForSeconds(.1f);
                index++;
            }

            m_changingMaterials = false;
        }

        public void EndHurtAnim()
        {
            
        }

        private void StartWaitForResetHutedTimes()
        {
            if(_isWatingForHurtedtimes)
            {
                StopCoroutine(_waitingForHurtedCoroutine);
            }
            if (gameObject != null)
            {
                _waitingForHurtedCoroutine = StartCoroutine(WaitForResetHutedTimes());
            }
        }

        private IEnumerator WaitForResetHutedTimes()
        {
            _isWatingForHurtedtimes = true;
            yield return new WaitForSeconds(2f);
            _isWatingForHurtedtimes = false;
            HurtedTimes = 0;
        }

        public void SetDeath(bool isDeath)
        {
            IsDeath = isDeath;
        }
        public bool CheckIfPlayerIsNear(float radius)
        {
            return Physics.CheckSphere(transform.position, radius);
        }

        public void SetMaxhealthSlider(float maxHealth)
        {
            matSlider.SetFloat("_DisolveAmount", maxHealth / 100);
        }

        public void SetHealthSlider(float health)
        {
            matSlider.SetFloat("_DisolveAmount",health/100);
        }

        public bool CanReceiveDamage()
        {
            return true;
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
