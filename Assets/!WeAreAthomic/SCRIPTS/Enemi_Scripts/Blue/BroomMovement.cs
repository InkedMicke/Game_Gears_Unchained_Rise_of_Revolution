using System;
using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomMovement : Broom
    {
        private Coroutine m_c_chasingPlayer;

        public void ChasePlayer(Action method = null, float distance = 0)
        {
            if(_isChasingPlayer)
            {
                StopCoroutine(m_c_chasingPlayer);
            }
            _broomAnimator.SetIsWalking(true);
            m_c_chasingPlayer = StartCoroutine(ChasingPlayer(method, distance));
        }

        public void StopChasePlayer()
        {
            _isChasingPlayer = false;
            _broomAnimator.SetIsWalking(false);
            StopCoroutine(m_c_chasingPlayer);
        }

        private IEnumerator ChasingPlayer(Action method, float distance)
        {
            _isChasingPlayer = true;
            while(true)
            {
                if(Vector3.Distance(transform.position, _playerTr.position) < distance)
                {
                    break;
                }

                _agent.SetDestination(_playerTr.position);
                yield return new WaitForEndOfFrame();
            }

            method();
        }

        public void EnableMovement() => _agent.isStopped = true;
        public void DisableMovement() => _agent.isStopped = false;
    }
}