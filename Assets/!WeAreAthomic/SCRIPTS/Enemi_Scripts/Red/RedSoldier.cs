using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Red
{
    public class RedSoldier : Enemy
    {
        G_MeshTrail _trail;

        [SerializeField] private RedAttackHitBox _attackHitBox;

        [SerializeField] private ParticleSystem _particlesTornadoDash;

        private Tween _dashTween;

        [SerializeField] private Ease dashEase;

        [SerializeField] private LayerMask layersToStopDashing;

        [SerializeField] private Transform indiciatorPivot;
        [SerializeField] private Transform endTrDecal;

        private Vector3 indicatorStartPos;

        private bool _isDashing;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float dashDuracion = 5f;

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
            var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

            if (distanceToPlayer < 9f && !IsAttacking && !_soldierHurtBox.IsDeath && IsChasingPlayer)
            {
                if (!IsAttacking)
                {
                    _agent.isStopped = true;
                    _soldierAnim.SetWalking(false);
                    StartDecalToAttack();
                    IsChasingPlayer = false;
                    isPatrolling = false;
                    IsOnWarning = false;
                }
            }

            if(_isDashing)
            {
                if (Physics.Raycast(transform.position, transform.forward, 1f, layersToStopDashing))
                {
                    _dashTween.Kill();
                    EndDash();
                }
            }

            base.Update();
        }

        private void StartDecalToAttack()
        {
            _agent.isStopped = true;
            IsAttacking = true;
            StartCoroutine(TurnToPlayer());
        }

        private IEnumerator TurnToPlayer()
        {
            while (!_soldierHurtBox.IsDeath)
            {
                var distanceToPlayer = _playerTr.position - transform.position;
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
            _agent.enabled = false;
            _attackHitBox.ClearList();
            _soldierAnim.SetAnimRootMotion(true);
            var desiredEndPos = new Vector3(endTrDecal.position.x, transform.position.y, endTrDecal.position.z);
            _dashTween = transform.DOMove(desiredEndPos, dashDuracion).SetEase(dashEase).OnComplete(() => _soldierAnim.SetRedCount(3));
            _soldierAnim.SetRedCount(2);
            indicator.gameObject.transform.SetParent(transform.parent);
            _trail.EnableTrail();
            _isDashing = true;
        }

        public void EndDash()
        {
            indicator.uvBias = new(0, 1);
            indicator.gameObject.transform.SetParent(indiciatorPivot.transform);
            indicator.transform.localPosition = indicatorStartPos;
            _soldierAnim.SetRedCount(0);
            _agent.enabled = true;
           
            _isDashing = false;
            IsAttacking = false;
            _soldierAnim.SetAnimRootMotion(false);
        }
    }

    
}