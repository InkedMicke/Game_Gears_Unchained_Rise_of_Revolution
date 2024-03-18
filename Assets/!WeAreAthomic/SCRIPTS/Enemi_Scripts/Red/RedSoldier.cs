using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Generics;
using Broom;
using Seth;

namespace Enemy.Red
{
    public class RedSoldier : EnemyP
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

        [SerializeField] private LayerMask obstaclesLayer;

        [SerializeField] private Transform indiciatorPivot;
        [SerializeField] private Transform endTrDecal;
        [SerializeField] private Transform posCheckObstacles;

        private Vector3 indicatorStartPos;

        private bool _isDashing;
        private bool _stoppedAttack;
        bool m_gotObstacle;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float dashDuracion = 5f;
        [SerializeField] private float dashForce = 10f;
        [SerializeField] float obstacleDetectionRadius;

        protected override void Awake()
        {
            base.Awake();
            _trail = GetComponent<G_MeshTrail>();
            indicatorStartPos = indicator.transform.localPosition;
            _soldierHurtBox.OnHurtedSmallerTwo += EndDash;
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
            _agent.enabled = false;
            _attackHitBox.ClearList();
            _soldierAnim.SetRedCount(2);
            indicator.gameObject.transform.SetParent(null);
            StartCoroutine(Dash());
            StartCoroutine(CheckForObstacles());
            PlayDashSound();
            _isDashing = true;
        }

        private IEnumerator Dash()
        {
            var startPos = transform.position;
            _trail.EnableTrail();
            while (Vector3.SqrMagnitude(startPos - transform.position) < 60f && !m_gotObstacle)
            {
                _cc.Move(Time.deltaTime * dashForce * transform.forward.normalized);
                yield return new WaitForEndOfFrame();
            }
            _soldierAnim.SetRedCount(3);
            IsAttacking = false;
        }

        private IEnumerator CheckForObstacles()
        {
            while (_isDashing)
            {
                if (Physics.CheckSphere(posCheckObstacles.position, obstacleDetectionRadius, obstaclesLayer))
                {
                    var cols = Physics.OverlapSphere(posCheckObstacles.position, obstacleDetectionRadius, obstaclesLayer);
                    m_gotObstacle = true;
                    foreach (var col in cols)
                    {
                        if (col.TryGetComponent(out SethBustHurtBox hurtbox))
                        {
                            hurtbox.StartBustDestroyEffect();
                        }
                    }
                    _soldierAnim.SetRedCount(3);
                    IsAttacking = false;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
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
            StopCoroutine(WaitUntilNextAttack());
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