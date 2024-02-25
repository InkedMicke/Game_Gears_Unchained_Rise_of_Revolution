using System;
using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomMovement : MonoBehaviour
    {
        Broom m_broom;

        private Coroutine m_c_chasingPlayer;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();
        }

        public void ChasePlayer(Action method = null, float distance = 0)
        {
            if(m_broom.IsChasingPlayer)
            {
                StopCoroutine(m_c_chasingPlayer);
            }
            m_broom.broomAnimator.SetIsWalking(true);
            EnableMovement();
            m_c_chasingPlayer = StartCoroutine(ChasingPlayer(method, distance));
        }

        public void StopChasePlayer()
        {
            m_broom.SetIsChasingPlayer(false);
            m_broom.broomAnimator.SetIsWalking(false);
            StopCoroutine(m_c_chasingPlayer);
        }

        private IEnumerator ChasingPlayer(Action method, float distance)
        {
            m_broom.SetIsChasingPlayer(true);
            while(true)
            {
                if(Vector3.Distance(transform.position, m_broom.PlayerTr.position) < distance)
                {
                    DisableMovement();
                    method();
                    m_broom.broomAnimator.SetIsWalking(false);
                    m_broom.SetIsChasingPlayer(false);
                    break;
                }

                m_broom.Agent.SetDestination(m_broom.PlayerTr.position);
                yield return new WaitForEndOfFrame();
            }
        }

        public void EnableMovement() => m_broom.Agent.isStopped = false;
        public void DisableMovement() => m_broom.Agent.isStopped = true;
    }
}