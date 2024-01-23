using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie
{
    public class DummieSounds : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float volumeClips;

        [SerializeField] private List<AudioClip> hurtClips;

        private void Update()
        {
            var audiosInSoundComponent = GetComponents<AudioSource>();

            foreach (var audioSour in audiosInSoundComponent)
            {
                if (!audioSour.isPlaying)
                {
                    Destroy(audioSour);
                }
            }
        }

        public void PlayHurtSound()
        {
            var randomNumber = Random.Range(0, hurtClips.Count);
            var currentAudioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            if (!ReferenceEquals(currentAudioSource, null))
            {
                currentAudioSource.clip = hurtClips[randomNumber];
                currentAudioSource.volume = volumeClips;
                currentAudioSource.Play();
            }
        }
    }
}
