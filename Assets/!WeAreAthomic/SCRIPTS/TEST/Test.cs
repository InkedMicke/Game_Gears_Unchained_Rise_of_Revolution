using System;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.TEST
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private AudioSource currentAudio;
        
        [SerializeField] private AudioClip introClip;
        [SerializeField] private AudioClip loopClip;

        private void Awake()
        {
            currentAudio.clip = introClip;
            currentAudio.Play();
            Invoke(nameof(PlayLoop), introClip.length);
        }

        private void PlayLoop()
        {
            currentAudio.clip = loopClip;
            currentAudio.Play();
            currentAudio.loop = true;
            Debug.Log("hola");
        }
    }
}
