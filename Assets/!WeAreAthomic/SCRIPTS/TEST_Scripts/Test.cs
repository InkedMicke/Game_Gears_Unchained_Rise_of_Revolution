using System;
using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.TEST
{
    public class Test : MonoBehaviour
    { 


/*        private void Awake()
        {
            currentAudio.clip = introClip;
            currentAudio.Play();
            Invoke(nameof(PlayLoop), currentAudio.clip.length);
        }

        private void PlayLoop()
        {
            currentAudio.clip = loopClip;
            currentAudio.Play();
            currentAudio.loop = true;
        }*/

        public void ShowMessage(string text)
        {
            Debug.Log(text);
        }
        

    }
}
