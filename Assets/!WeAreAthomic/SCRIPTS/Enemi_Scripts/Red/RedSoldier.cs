using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Red
{
    public class RedSoldier : Enemy
    {
        G_MeshTrail _trail;

        [SerializeField] private RedAttackHitBox _attackHitBox;

        [SerializeField] private BoxCollider _hitboxCol;

        [SerializeField] private ParticleSystem _particlesTornadoDash;
        [SerializeField] private AudioClip dashAudio;
        [Range(0, 1)]
        [SerializeField] private float dashVolume;

        [SerializeField] private GameObject soundComponentObj;
        [SerializeField] private AudioMixerGroup sfxMixer;

        private Tween _dashTween;

        [SerializeField] private Ease dashEase;

        [SerializeField] private LayerMask layersToStopDashing;

        [SerializeField] private Transform indiciatorPivot;
        [SerializeField] private Transform endTrDecal;

        private Vector3 indicatorStartPos;

        private bool _isDashing;
        private bool _stoppedAttack;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float dashDuracion = 5f;
        [SerializeField] private float dashForce = 10f;

        protected override void Awake()
        {
            _trail = GetComponent<G_MeshTrail>();
            indicatorStartPos = indicator.transform.localPosition;
            base.Awake();
        }

        protected override void Start()
        {

            base.Start();
        }

        protected override void Update()
        {
            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTr.position);

            if (distanceToPlayer < 9f && !IsAttacking && !_soldierHurtBox.IsDeath && IsChasingPlayer)
            {
                _agent.isStopped = true;
                _soldierAnim.SetWalking(false);
                StartDecalToAttack();
                IsChasingPlayer = false;
                isPatrolling = false;
                IsOnWarning = false;

            }

            if (_isDashing)
            {

                if (Physics.Raycast(transform.position + Vector3.up * 1.1f, transform.forward, 1f, layersToStopDashing))
                {
                    _dashTween.Kill();
                    EndDash();
                }
            }

            base.Update();
        }

        private void StartDecalToAttack()
        {
            IsAttacking = true;
            StartCoroutine(TurnToPlayer());
        }

        private IEnumerator TurnToPlayer()
        {
            while (!_soldierHurtBox.IsDeath)
            {
                var distanceToPlayer = PlayerTr.position - transform.position;
                distanceToPlayer.y = 0f;
                if (distanceToPlayer.magnitude > 9f)
                {
                    ChangingValuesToChase();
                    IsAttacking = false;
                    break;
                }

                // Calcular la rotaci?n hacia el objetivo
                Quaternion targetRotation = Quaternion.LookRotation(distanceToPlayer);

                var actualSpeed = rotationSpeed * Mathf.Pow(15 / distanceToPlayer.magnitude, 2);

                actualSpeed = Mathf.Clamp(actualSpeed, 0f, 25f);

                // Rotar suavemente hacia el objetivo
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, actualSpeed * Time.deltaTime);

                // Si estamos casi mirando al objetivo, detener la rotaci?n
                if (Quaternion.Angle(transform.rotation, targetRotation) < 2f)
                {
                    StartCoroutine(DecalSizer(true));
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator DecalSizer(bool greaterOrSmaller)
        {
            yield return new WaitForSeconds(.2f);

            if (greaterOrSmaller)
            {
                indicator.uvBias = new(indicator.uvBias.x, 1);

                while (indicator.uvBias.y > 0)
                {
                    indicator.uvBias -= new Vector2(0, Time.deltaTime * 8);
                    yield return new WaitForSeconds(.01f);
                }
                indicator.uvBias = new(indicator.uvBias.x, 0);
                _soldierAnim.SetRedCount(1);
                _stoppedAttack = false;
                _particlesTornadoDash.Play();
            }
            else
            {
                indicator.uvBias = new(indicator.uvBias.x, 0);

                while (indicator.uvBias.y < 1)
                {
                    indicator.uvBias += new Vector2(0, Time.deltaTime * 16);
                    yield return new WaitForSeconds(.01f);
                }

                indicator.uvBias = new(indicator.uvBias.x, 1);
            }
        }

        public void StartDash()
        {
            //StartCoroutine(DashForward());
            Rb.mass = 10;
            _agent.enabled = false;
            _attackHitBox.ClearList();
            var desiredEndPos = new Vector3(endTrDecal.position.x, transform.position.y, endTrDecal.position.z);
            _soldierAnim.SetRedCount(2);
            //_dashTween = transform.DOMove(desiredEndPos, dashDuracion).SetEase(dashEase).OnComplete(() => _soldierAnim.SetRedCount(3));
            StartCoroutine(WaitForEndAnim());
            Rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            indicator.gameObject.transform.SetParent(transform.parent);
            _trail.EnableTrail();
            PlayDashSound();
            _isDashing = true;
        }

        private IEnumerator WaitForEndAnim()
        {
            yield return new WaitForSeconds(.8f);
            _soldierAnim.SetRedCount(3);
        }

        public void EndDash()
        {
            _isDashing = false;
            indicator.uvBias = new(0, 1);
            indicator.gameObject.transform.SetParent(indiciatorPivot.transform);
            indicator.transform.localPosition = indicatorStartPos;
            _soldierAnim.SetRedCount(0);
            _agent.enabled = true;
            _soldierAnim.SetAnimRootMotion(false);
            StartCoroutine(WaitUntilNextAttack());
        }

        public override void StopAttackDueToHurt()
        {
            if (!_isDashing)
            {
                _soldierAnim.SetRedCount(0);
                StartCoroutine(DecalSizer(false));
                _stoppedAttack = true;
                StartDecalToAttack();
            }
            base.StopAttackDueToHurt();
        }

        public override void Knockback()
        {
            if (!_isDashing)
            {
                base.Knockback();
            }
        }

        public void EnableHitboxCollision()
        {
            if (!_stoppedAttack)
            {
                _hitboxCol.enabled = true;
            }
        }

        public void DisableHitBoxCollision()
        {
            _hitboxCol.enabled = false;
        }

        public void PlayDashSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = dashAudio;
                currentAudioSource.volume = dashVolume;

                currentAudioSource.Play();
            }
        }
    }


}