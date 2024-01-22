using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OrangeSoldier : Enemy
{
    private C_MaterialChangeOnDetection _materialChange;

    [SerializeField] private DecalProjector indicator;

    [SerializeField] private ParticleSystem groundSplashEffect;

    [SerializeField] private GameObject decalArea;

    [SerializeField] private float maxDecalRange;
    [SerializeField] private float velocityDecalGrow;

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
            StartCoroutine(GrowAttackIndicator());
            IsChasingPlayer = false;
            isPatrolling = false;
            IsAttacking = true;
        }
        base.Update();
    }

    private IEnumerator GrowAttackIndicator()
    {
        _materialChange.WarningDecal();
        _soldierAnim.ShootTrigger();
        while (indicator.size.x < 9)
        {
            var sizeIndiciator = indicator.size;
            sizeIndiciator.x += .053f;
            sizeIndiciator.y += .053f;
            indicator.size = sizeIndiciator;
            yield return new WaitForEndOfFrame();
        }
        _materialChange.CatchDecal();
    }



}
