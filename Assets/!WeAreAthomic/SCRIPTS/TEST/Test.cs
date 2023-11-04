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

        /*private void Start()
        {
            currentAudio.clip = introClip;
            currentAudio.Play();
            Invoke(nameof(PlayLoop), introClip.length - 1f);
            Debug.Log(introClip.length);
        }

        private void PlayLoop()
        {
            currentAudio.clip = loopClip;
            currentAudio.Play();
            currentAudio.loop = true;
            Debug.Log("hola3");
        }*/

        private void OnPlayMusic()
        {
            goalTime = AudioSettings.dspTime + .5f;
            
            currentAudio.PlayScheduled(goalTime);

            musicDuration = (double)introClip.samples / introClip.frequency;

            goalTime = goalTime + musicDuration;
        }

        private void Update()
        {
            if (AudioSettings.dspTime > goalTime - 1)
            {
                currentAudio.clip = loopClip;
                currentAudio.PlayScheduled(goalTime);
                
                musicDuration = (double)loopClip.samples / loopClip.frequency;
                goalTime = AudioSettings.dspTime + musicDuration;
            }
        }
    }
}
