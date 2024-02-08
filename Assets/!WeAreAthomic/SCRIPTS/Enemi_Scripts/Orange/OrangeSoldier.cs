using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Orange
{
    public class OrangeSoldier : Enemy
    {
        private C_MaterialChangeOnDetection _materialChange;

        [SerializeField] private HitBox[] _hitBoxes;

        [SerializeField] private ParticleSystem groundSplashEffect;
        [SerializeField] private AudioClip groundSplashAudio;
        [Range(0, 1)]
        [SerializeField] private float groundSplashVolume;

        [SerializeField] private GameObject soundComponentObj;
        [SerializeField] private AudioMixerGroup sfxMixer;

        [SerializeField] private GameObject decalArea;

        private bool _canGrowth;

        [SerializeField] private float maxDecalRange;
        [SerializeField] private float velocityDecalGrow;
        private float _passedTime = 0f;

        protected override void Awake()
        {
            _materialChange = GetComponent<C_MaterialChangeOnDetection>();
            base.Awake();
        }

        protected override void Update()
        {
            var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

            if (distanceToPlayer < 4.5f && !IsAttacking && !_soldierHurtBox.IsDeath && IsChasingPlayer && !IsAttacking)
            {
                if (!IsAttacking)
                {
                    _agent.isStopped = true;
                    _soldierAnim.SetWalking(false);
                    //StartCoroutine(GrowAttackIndicator());
                    GrowAttackIndicator();
                    IsChasingPlayer = false;
                    isPatrolling = false;
                    IsAttacking = true;
                }
            }

            if(_canGrowth)
            {
                _passedTime += Time.deltaTime;

                if (_passedTime >= .01f)
                {
                    // Calcular la tasa de crecimiento
                    float growthRate = .08f;

                    if(indicator.size.x >= 7)
                    {
                        _canGrowth = false;
                        _materialChange.CatchDecal();
                    }

                    // Incrementar el tamaño de la esfera
                    var sizeIndiciator = indicator.size;
                    sizeIndiciator.x *= 1 + growthRate;
                    sizeIndiciator.y *= 1 + growthRate;
                    indicator.size = sizeIndiciator;

                    // Reiniciar el temporizador
                    _passedTime = 0f;
                }
            }

            base.Update();
        }

        private void GrowAttackIndicator()
        {
            _materialChange.WarningDecal();
            _soldierAnim.ShootTrigger();
            foreach(var x in _hitBoxes)
            {
                x.ClearList();
            }
        }

        public void PlayHitEffect()
        {
            groundSplashEffect.Play();
            PlaySplashSound();
        }

        public void EndAttack()
        {
            var sizeIndiciator = indicator.size;
            sizeIndiciator.x = 0.1f;
            sizeIndiciator.y = 0.1f;
            indicator.size = sizeIndiciator;
            StartCoroutine(WaitUntilNextAttack());
        }

        public void StartGrowth()
        {
            _canGrowth = true;
        }
        public void PlaySplashSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = groundSplashAudio;
                currentAudioSource.volume = groundSplashVolume;
                
                currentAudioSource.Play();
            }
        }
    }
}
