using System;
using System.Collections;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;

namespace Broom
{
    public class BroomDefense : MonoBehaviour
    {
        Broom m_broom;

        private Coroutine m_c_waitForEndShield;

        [NonSerialized] public bool IsDefending;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();
        }

        public void Defense()
        {
            if (IsDefending)
            {
                StopCoroutine(m_c_waitForEndShield);
                m_c_waitForEndShield = StartCoroutine(WaitForDisableShield());
            }
            else
            {
                m_broom.broomAnimator.SetShieldCount(1);
                Utilities.Invoke(this, () => m_broom.broomAnimator.SetShieldCount(2), .1f);
                m_c_waitForEndShield = StartCoroutine(WaitForDisableShield());
            }
        }

        public void EndDefense()
        {
            m_broom.broomAnimator.SetShieldCount(0);
        }

        public void CrossAttack()
        {
            m_broom.SetIsAttacking(true);
            m_broom.broomAnimator.SetRootMotion(true);
            m_broom.broomAnimator.SetCrossAttackCount(1);
        }

        public void StartRotatingToPlayerPos()
        {
            StartCoroutine(RotateToPlayerPos());
        }

        public IEnumerator RotateToPlayerPos()
        {
            var distanceToPlayer = m_broom.PlayerTr.position - transform.position;
            var targetRotation = Quaternion.LookRotation(distanceToPlayer);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 20f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        public void EndCrossAttack()
        {
            m_broom.SetIsAttacking(false);
            m_broom.broomAnimator.SetRootMotion(false);
        }

        private IEnumerator WaitForDisableShield()
        {
            IsDefending = true;
            yield return new WaitForSeconds(2f);
            EndDefense();
            IsDefending = false;
        }
    }
}