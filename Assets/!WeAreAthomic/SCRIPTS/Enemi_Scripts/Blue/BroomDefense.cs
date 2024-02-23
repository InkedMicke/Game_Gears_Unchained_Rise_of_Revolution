using System;
using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomDefense : Broom
    {
        private Coroutine m_c_waitForEndShield;

        [NonSerialized] public bool IsDefending;

        public void Defense()
        {
            if(IsDefending)
            {
                StopCoroutine(m_c_waitForEndShield);
            }
            _broomAnimator.SetShieldCount(1);
            m_c_waitForEndShield = StartCoroutine(WaitForDisableShield());
        }

        public void EndDefense()
        {
            _broomAnimator.SetShieldCount(0);
        }

        public void CrossAttack()
        {
            _isAttacking = true;
            _broomAnimator.SetCrossAttackCount(1);
        }

        public void EndCrossAttack()
        {

        }

        private IEnumerator WaitForDisableShield()
        {
            IsDefending = true;
            yield return new WaitForSeconds(1f);
            EndDefense();
            IsDefending = false;
        }
    }
}