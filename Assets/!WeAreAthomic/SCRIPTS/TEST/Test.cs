using System;
using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.TEST
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private AudioSource currentAudio;
        
        [SerializeField] private AudioClip introClip;
        [SerializeField] private AudioClip loopClip;

        public double musicDuration;
        public double goalTime;
        
        
        private void Awake()
        {
            currentAudio.clip = introClip;
            currentAudio.Play();
            Invoke(nameof(PlayLoop), currentAudio.clip.length);
        }

        private void Update()
        {
        }

        private void PlayLoop()
        {
            currentAudio.clip = loopClip;
            currentAudio.Play();
            currentAudio.loop = true;
        }
        

    }
}
