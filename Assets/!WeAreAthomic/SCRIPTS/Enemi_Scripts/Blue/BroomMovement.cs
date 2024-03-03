using System;
using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomMovement : MonoBehaviour
    {
        Broom m_broom;

        private Coroutine m_c_chasingPlayer;

        float m_startSpeed;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();
        }

        private void Start()
        {
            m_startSpeed = m_broom.Agent.speed;
        }

        public void ChasePlayerAtDistance(float distance = 0, Action method = null)
        {
            SetAgentSpeed(m_startSpeed);
            if (m_broom.IsChasingPlayer)
            {
                StopCoroutine(m_c_chasingPlayer);
            }
            m_broom.broomAnimator.SetIsWalking(true);
            EnableMovement();
            m_c_chasingPlayer = StartCoroutine(ChasingPlayerDistance(method, distance));
        }

        public void ChasePlayerWithTime(float time = 0, Action method = null)
        {
            SetAgentSpeed(m_startSpeed);
            m_broom.broomAnimator.SetIsWalking(true);
            EnableMovement();
            StartCoroutine(ChasingPlayerTime(method, time));
        }

        public void StopChasePlayer()
        {
            m_broom.SetIsChasingPlayer(false);
            m_broom.broomAnimator.SetIsWalking(false);
            StopCoroutine(m_c_chasingPlayer);
        }

        private IEnumerator ChasingPlayerDistance(Action method, float distance)
        {
            m_broom.SetIsChasingPlayer(true);
            while(Vector3.Distance(transform.position, m_broom.PlayerTr.position) > distance)
            {
                m_broom.Agent.SetDestination(m_broom.PlayerTr.position);
                yield return new WaitForEndOfFrame();
            }

            DisableMovement();
            m_broom.broomAnimator.SetIsWalking(false);
            m_broom.SetIsChasingPlayer(false);
            if (method == null)
                yield return null;
            method();
        }        
        
        private IEnumerator ChasingPlayerTime(Action method, float time)
        {
            var golTime = Time.time + time;
            m_broom.SetIsChasingPlayer(true);
            while(Time.time < golTime)
            {
                m_broom.Agent.SetDestination(m_broom.PlayerTr.position);
                yield return new WaitForEndOfFrame();
            }

            DisableMovement();
            m_broom.broomAnimator.SetIsWalking(false);
            m_broom.SetIsChasingPlayer(false);
            if (method == null)
                yield return null;
            method();
        }

        public void EnableMovement() => m_broom.Agent.isStopped = false;
        public void DisableMovement() => m_broom.Agent.isStopped = true;
        public void SetAgentSpeed(float speed) => m_broom.Agent.speed = speed;
    }
}