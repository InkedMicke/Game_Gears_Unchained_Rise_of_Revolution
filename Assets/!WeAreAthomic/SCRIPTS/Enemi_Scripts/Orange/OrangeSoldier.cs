using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Orange
{
    public class OrangeSoldier : Enemy
    {
        private C_MaterialChangeOnDetection _materialChange;

        [SerializeField] private HitBox[] _hitBoxes;

        [SerializeField] private ParticleSystem groundSplashEffect;

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

            if (distanceToPlayer < 6f && !IsAttacking && !_soldierHurtBox.IsDeath && IsChasingPlayer && !IsAttacking)
            {
                _agent.isStopped = true;
                _soldierAnim.SetWalking(false);
                //StartCoroutine(GrowAttackIndicator());
                GrowAttackIndicator();
                IsChasingPlayer = false;
                isPatrolling = false;
                IsAttacking = true;
            }

            if(_canGrowth)
            {
                _passedTime += Time.deltaTime;

                if (_passedTime >= .01f)
                {
                    // Calcular la tasa de crecimiento
                    float growthRate = .03f;

                    if(indicator.size.x >= 9)
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
        }

        public void EndAttack()
        {
            var sizeIndiciator = indicator.size;
            sizeIndiciator.x = 0.1f;
            sizeIndiciator.y = 0.1f;
            indicator.size = sizeIndiciator;
            IsAttacking = false;
            ChangingValuesToChase();
        }

        public void StartGrowth()
        {
            _canGrowth = true;
        }
    }
}
