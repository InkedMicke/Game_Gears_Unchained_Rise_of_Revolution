using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomDash : Broom
    {
        private Coroutine m_c_turnToPlayer;

        public void StartDecalToAttack()
        {
            _isAttacking = true;
            //m_c_turnToPlayer = StartCoroutine(TurnToPlayer());
        }
    }
}