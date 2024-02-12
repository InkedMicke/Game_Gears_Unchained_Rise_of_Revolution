using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Orange
{
    public class OrangeSoldier : Enemy
    {
        private C_MaterialChangeOnDetection _materialChange;

        [SerializeField] private SphereCollider _hitbox;

        [SerializeField] private ParticleSystem groundSplashEffect;
        [SerializeField] private AudioClip groundSplashAudio;
        [Range(0, 1)]
        [SerializeField] private float groundSplashVolume;

        [SerializeField] private GameObject soundComponentObj;
        [SerializeField] private AudioMixerGroup sfxMixer;

        [SerializeField] private GameObject decalArea;

        private bool _canGrowth;
        private bool _stoppedAttack;

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
            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTr.position);

            if (distanceToPlayer < 4.5f && !IsAttacking && !_soldierHurtBox.IsDeath && IsChasingPlayer)
            {
                    _agent.isStopped = true;
                    _soldierAnim.SetWalking(false);
                    GrowAttackIndicator();
                    IsChasingPlayer = false;
                    isPatrolling = false;
                    IsAttacking = true;
            }

            if (_canGrowth)
            {
                _passedTime += Time.deltaTime;

                if (_passedTime >= .01f)
                {
                    // Calcular la tasa de crecimiento
                    float growthRate = .08f;

                    if (indicator.size.x >= 7)
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
            _soldierAnim.SetOrangeCount(1);
            _soldierAnim.SetWalking(false);
            _stoppedAttack = false;
        }

        public void PlayHitEffect()
        {
            groundSplashEffect.Clear();
            groundSplashEffect.Play();
            PlaySplashSound();
        }

        public void EndAttack()
        {
            _soldierAnim.SetOrangeCount(0);
            var sizeIndiciator = indicator.size;
            sizeIndiciator.x = 0.1f;
            sizeIndiciator.y = 0.1f;
            indicator.size = sizeIndiciator;
            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTr.position);
            if (distanceToPlayer > 4.5f)
            {
                StartCoroutine(WaitUntilNextAttack());
            }
            else
            {
                Invoke(nameof(GrowAttackIndicator), 1f);
                
            }
        }

        public void StartGrowth()
        {
            _canGrowth = true;
        }

        public void EnableHitBoxCollision()
        {
            if (!_stoppedAttack)
            {
                _hitbox.enabled = true;
            }
        }

        public void DisableHitBoxCollision()
        {
            _hitbox.enabled = false;
        }

        public override void StopAttackDueToHurt()
        {
            _canGrowth = false;
            _stoppedAttack = true;
            groundSplashEffect.Clear();
            EndAttack();
            base.StopAttackDueToHurt();
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
