using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Net;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie
{
    public class DummieController : MonoBehaviour
    {
        private Animator m_anim;
        private CharacterController m_cc;
        private MainCAttack m_mainCAttack;

        private Transform m_playerTr;

        public bool removeCollisionOnDeath;

        private void Awake()
        {
            m_anim = GetComponent<Animator>();
            m_cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            m_playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            m_mainCAttack = m_playerTr.GetComponent<MainCAttack>();
        }

        public void DisableCharacterController()
        {
            if (removeCollisionOnDeath)
            {
                m_cc.enabled = false;
            }
        }
        
        public void HurtAnim()
        {
            m_anim.SetTrigger(string.Format("isHurt"));
        }

        public void StartPushBack()
        {
            float speed = 0;
            float distance = 0;
            switch (m_mainCAttack.attackCount)
            {
                case 0:
                    distance = 1.5f;
                    speed = 25;
                    break;
                case 1:
                    distance = 1.5f;
                    speed = 25f;
                    break;
                case 2:
                    distance = 2f;
                    speed = 25f;
                    break;
            }

            StartCoroutine(PushBack(m_playerTr.forward.normalized, distance, speed));
        }

        private IEnumerator PushBack(Vector3 dir, float distance, float speed)
        {
            var startPos = transform.position;
            dir.y = 0;

            while (Mathf.Abs(Vector3.SqrMagnitude(startPos - transform.position)) < distance)
            {
                m_cc.Move(speed * Time.deltaTime * dir);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
