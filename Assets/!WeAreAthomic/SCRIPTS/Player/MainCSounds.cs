using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCSounds : MonoBehaviour
    {
        [SerializeField] private GameObject soundComponentObj;

        [SerializeField] private AudioMixerGroup sfxMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup voicesMixer;
        
        [SerializeField] private List<AudioClip> attackClips;
        [SerializeField] private AudioClip hackInProcess;
        [SerializeField] private AudioClip cannotHack;
        [SerializeField] private AudioClip howFight;

        public void StopAttackSound()
        {
            //attackAudioSource.Stop();
        }

        private void Update()
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();

            foreach (var audioSour in audiosInSoundComponent)
            {
                if (!audioSour.isPlaying)
                {
                    Destroy(audioSour);
                }
            }
        }

        public void PlayAttackSound()
        {
            var randomNumber = Random.Range(0, attackClips.Count);
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = attackClips[randomNumber];
                currentAudioSource.Play();
            }
        }
        
        public void PlayHackInProcessSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = hackInProcess;
                currentAudioSource.volume = .7f;
                currentAudioSource.Play();
                currentAudioSource.loop = true;
            }
        }
        
        public void StopHackInProcessSound()
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();

            foreach (var audioSour in audiosInSoundComponent)
            {
                if (audioSour.clip == hackInProcess)
                {
                    Destroy(audioSour);
                }
            }
        }

        public void PlayCannotHackSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = cannotHack;
                currentAudioSource.volume = .7f;
                currentAudioSource.Play();
            }
        }

        public void PlayHowFightSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = howFight;
                currentAudioSource.volume = .7f;
                currentAudioSource.Play();
            }
        }
        
        
    }
}
