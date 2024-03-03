using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.ParticleSystem;

namespace Broom
{
    public class BroomDash : MonoBehaviour
    {
        Broom m_broom;

        [SerializeField] BroomDashHitBox hitbox;

        [SerializeField] EnemyDamageData dashDamageData;

        [SerializeField] DecalProjector indicator;

        [SerializeField] BoxCollider boxColliderDash;

        private Coroutine m_c_turnToPlayer;

        [SerializeField] private LayerMask obstaclesLayer;

        [SerializeField] GameObject decalPivot;

        [SerializeField] Transform endTrDecal;
        [SerializeField] Transform posCheckObstacles;

        Vector3 m_indicatorStartPos;

        bool m_isDasing;

        [SerializeField] float dashSpeed = 40;
        [SerializeField] float dashDisplacement = 10;
        [SerializeField] float timeStunned = 4;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();

            m_indicatorStartPos = indicator.transform.localPosition;
        }

        public void StartDecalToAttack()
        {
            m_broom.SetIsAttacking(true);
            decalPivot.SetActive(true);
            m_broom.broomMove.DisableMovement();
            m_broom.broomMove.SetAgentSpeed(0);
            m_c_turnToPlayer = StartCoroutine(TurnToPlayer());
        }

        private IEnumerator TurnToPlayer()
        {
            var distanceToPlayer = m_broom.PlayerTr.position - transform.position;
            var targetRotation = Quaternion.LookRotation(distanceToPlayer);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            StartCoroutine(DecalSizer(true));
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
                m_broom.broomAnimator.SetThurstCount(1);
            }
            else
            {
                indicator.uvBias = new(indicator.uvBias.x, 0);

                while (indicator.uvBias.y < 1)
                {
                    indicator.uvBias += new Vector2(0, Time.deltaTime * 16);
                    yield return new WaitForSeconds(.01f);
                }
                decalPivot.SetActive(false);
                indicator.uvBias = new(indicator.uvBias.x, 1);
            }
        }

        public void StartDash()
        {
            m_isDasing = true;
            hitbox.EnemyDamageDataDash = dashDamageData;
            StartCoroutine(CheckForObstacles());
            StartCoroutine(Dash());
            indicator.gameObject.transform.SetParent(transform.parent);
            //_trail.EnableTrail();
            //PlayDashSound();
        }

        public void EndDash()
        {
            indicator.uvBias = new(0, 1);
            indicator.gameObject.transform.SetParent(decalPivot.transform);
            indicator.transform.localPosition = m_indicatorStartPos;
            m_broom.broomAnimator.SetThurstCount(0);
            m_broom.WaitForDecideWhatToDo();
            m_broom.SetIsAttacking(false);
            decalPivot.SetActive(false);
        }

        private IEnumerator CheckForObstacles()
        {
            while (m_isDasing)
            {
                Debug.Log("hola1");
                if (Physics.CheckSphere(posCheckObstacles.position, .5f, obstaclesLayer))
                {
                    Debug.Log("hola2");
                    var cols = Physics.OverlapSphere(posCheckObstacles.position, .5f, obstaclesLayer);

                    foreach (var col in cols)
                    {
                        if (col.TryGetComponent(out SethBustHurtBox hurtbox))
                        {
                            hurtbox.StartBustDestroyEffect();
                        }
                    }

                    m_broom.broomAnimator.SetIsStunned(true);
                    m_broom.SetIsAttacking(false);
                    m_broom.broomHurtBox.SetCanReceiveDamage(true);
                    m_broom.broomVFX.PlayStunning();
                    indicator.uvBias = new(0, 1);
                    indicator.gameObject.transform.SetParent(decalPivot.transform);
                    indicator.transform.localPosition = m_indicatorStartPos;
                    decalPivot.SetActive(false);
                    StartCoroutine(WaitForEndStunn());
                    m_isDasing = false;

                    break;
                }

                yield return new WaitForEndOfFrame();
            }




        }

        private IEnumerator Dash()
        {
            var startPos = transform.position;
            while(Mathf.Abs(Vector3.SqrMagnitude(startPos - transform.position)) < dashDisplacement)
            {
                m_broom.CC.Move(Time.deltaTime * dashSpeed * transform.forward.normalized);
                yield return new WaitForEndOfFrame();
            }

            m_broom.broomAnimator.SetThurstCount(2);
            m_isDasing = false;
        }

        public void DisableHitBoxCollision() => boxColliderDash.enabled = false;
        public void EnableHitBoxCollision() => boxColliderDash.enabled = true;

        private IEnumerator WaitForEndStunn()
        {
            yield return new WaitForSeconds(timeStunned);
            m_broom.broomVFX.StopStunning();
            m_broom.broomAnimator.SetIsStunned(false);
            m_broom.broomHurtBox.SetCanReceiveDamage(false);
            m_broom.WaitForDecideWhatToDo();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(posCheckObstacles.position, .5f);
        }

    }
}