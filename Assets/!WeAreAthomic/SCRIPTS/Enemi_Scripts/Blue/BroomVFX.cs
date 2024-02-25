using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Broom
{
    public class BroomVFX : MonoBehaviour
    {
        Broom m_Broom;

        [SerializeField] private ParticleSystem protectedEffect;

        [SerializeField] Transform protectedPivot;

        private void Awake()
        {
            m_Broom = GetComponent<Broom>();
        }

        public void PlayProtectedEffect()
        {
            protectedPivot.transform.LookAt(m_Broom.PlayerTr.position);
            protectedEffect.time = 0;
            protectedEffect.Play();
        }
    }
}