using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Broom
{
    public class BroomVFX : MonoBehaviour
    {
        Broom m_Broom;

        [SerializeField] private ParticleSystem protectedEffect;

        [SerializeField] GameObject stunningEffect;

        [SerializeField] Transform protectedPivot;

        

        private void Awake()
        {
            m_Broom = GetComponent<Broom>();
        }

        public void PlayProtectedEffect()
        {
            var localPlayerPos = m_Broom.PlayerTr.position;
            localPlayerPos.y += .8f;
            protectedPivot.transform.LookAt(localPlayerPos);
            protectedEffect.time = 0;
            protectedEffect.Play();
        }

        public void PlayStunning() => stunningEffect.SetActive(true);
        public void StopStunning() => stunningEffect.SetActive(false);
    }
}