using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.ParticleSystem;

namespace Broom
{
    public class BroomDash : MonoBehaviour
    {
        Broom m_broom;

        [SerializeField] DecalProjector indicator;

        [SerializeField] BoxCollider boxColliderDash;

        private Coroutine m_c_turnToPlayer;

        [SerializeField] private LayerMask obstaclesLayer;

        [SerializeField] GameObject decalPivot;

        [SerializeField] Transform endTrDecal;
        [SerializeField] Transform posCheckObstacles;

        Vector3 m_indicatorStartPos;

        bool m_isDasing;

        [SerializeField] float dashForce = 40;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();

            m_indicatorStartPos = indicator.transform.localPosition;
        }

        private void Update()
        {
            if(m_isDasing)
            {
                if(Physics.CheckSphere(posCheckObstacles.position, .5f, obstaclesLayer))
                {
                    m_isDasing = false;
                    m_broom.Rb.mass = 40;
                    m_broom.Rb.velocity = Vector3.zero;
                    m_broom.broomAnimator.SetIsStunned(true);
                    m_broom.SetIsAttacking(false);
                    m_broom.broomHurtBox.SetCanReceiveDamage(true);

                    indicator.uvBias = new(0, 1);
                    indicator.gameObject.transform.SetParent(decalPivot.transform);
                    indicator.transform.localPosition = m_indicatorStartPos;
                    decalPivot.SetActive(false);
                }
            }
        }

        public void StartDecalToAttack()
        {
            m_broom.SetIsAttacking(true);
            decalPivot.SetActive(true);
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
            m_broom.Rb.mass = 10;
            //_attackHitBox.ClearList();
            StartCoroutine(WaitForEndAnim());
            m_broom.Rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            indicator.gameObject.transform.SetParent(transform.parent);
            //_trail.EnableTrail();
            //PlayDashSound();
            m_isDasing = true;
        }

        public void EndDash()
        {
            indicator.uvBias = new(0, 1);
            indicator.gameObject.transform.SetParent(decalPivot.transform);
            indicator.transform.localPosition = m_indicatorStartPos;
            m_broom.broomAnimator.SetThurstCount(0);
            m_broom.Rb.mass = 40;
            m_broom.WaitForDecideWhatToDo();
            m_broom.SetIsAttacking(false);
            decalPivot.SetActive(false);
        }

        public void DisableHitBoxCollision() => boxColliderDash.enabled = false;
        public void EnableHitBoxCollision() => boxColliderDash.enabled = true;

        private IEnumerator WaitForEndAnim()
        {
            yield return new WaitForSeconds(.8f);
            m_broom.broomAnimator.SetThurstCount(2);
            m_isDasing = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(posCheckObstacles.position, .5f);
        }

    }
}