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

        private void Start()
        {
            currentAudio.clip = introClip;
            currentAudio.Play();
            //Invoke(nameof(PlayLoop), introClip.length - 1.5f);
            StartCoroutine(PlayLoopDelayed(introClip.length));
            Debug.Log(introClip.length);
        }

        private void PlayLoop()
        {
            currentAudio.clip = loopClip;
            currentAudio.Play();
            currentAudio.loop = true;
            Debug.Log("hola3");
        }

        private IEnumerator PlayLoopDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayLoop();
        }
        
    }
}
