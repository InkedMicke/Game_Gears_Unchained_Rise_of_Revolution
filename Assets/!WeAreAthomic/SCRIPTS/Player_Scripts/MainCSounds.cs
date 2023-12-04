using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCSounds : MonoBehaviour
    {
        private Scene _currentScene;

        [SerializeField] private GameObject soundComponentObj;

        [System.NonSerialized] public AudioClip CurrentExpressionClip;
        [System.NonSerialized] public AudioClip CurrentTutorialClip;

        [SerializeField] private AudioMixerGroup sfxMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup voicesMixer;
        
        [SerializeField] private List<AudioClip> attackClips;
        [SerializeField] private List<AudioClip> tutorialClipsPC;
        [SerializeField] private List<AudioClip> tutorialClipsGamepad;
        [SerializeField] private List<AudioClip> expressionClips;
        [SerializeField] private AudioClip hackInProcess;
        [SerializeField] private AudioClip cannotHack;
        [SerializeField] private AudioClip howFight;

        private bool isPaused;

        private void Start()
        {
            _currentScene = SceneManager.GetActiveScene();
            if (_currentScene.name == "S2_LABTUTORIAL")
            {
                PlayTutorialSound(0, "pc");
            }
        }

        public void StopAttackSound()
        {
            //attackAudioSource.Stop();
        }

        public void PauseCurrentSounds()
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();

            foreach (var audioSor in audiosInSoundComponent)
            {
                audioSor.Pause();
            }
        }

        public void UnPauseCurrentSounds()
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();

            foreach (var audioSor in audiosInSoundComponent)
            {
                audioSor.UnPause();
            }
        }

        public void RemoveAllSounds()
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();

            foreach (var audioSor in audiosInSoundComponent)
            {
                Destroy(audioSor);
            }
        }

        public void RemoveAllTutorialSounds()
        {
            Destroy(CurrentTutorialClip);
        }

        public float GetAudioClipLength(string clipString)
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();
            foreach(var audio in audiosInSoundComponent)
            {
                if(audio.clip.name == clipString)
                {
                    return audio.clip.length;
                }
            }

            return 0;
        }

        private void Update()
        {

            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();

            foreach (var audioSour in audiosInSoundComponent)
            {
                if (!audioSour.isPlaying && !isPaused && !GameManagerSingleton.Instance.IsGamePaused)
                {
                    Destroy(audioSour);
                }
            }
        }

        public void PlaySoundDelayed(string function, float delay)
        {
            Invoke(function, delay);
        }

        public void PlayTutorialSound(int value, string control)
        {
            if (control == "pc")
            {
                var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
                if (currentAudioSource != null)
                {
                    currentAudioSource.outputAudioMixerGroup = voicesMixer;
                    currentAudioSource.clip = tutorialClipsPC[value];
                    currentAudioSource.volume = .9f;
                    currentAudioSource.playOnAwake = false;
                    currentAudioSource.Play();
                    CurrentTutorialClip = currentAudioSource.clip;
                }
            }

            if(control == "gamepad")
            {
                var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
                if (currentAudioSource != null)
                {
                    currentAudioSource.outputAudioMixerGroup = voicesMixer;
                    currentAudioSource.clip = tutorialClipsGamepad[value];
                    currentAudioSource.volume = .9f;
                    currentAudioSource.Play();
                    CurrentTutorialClip = currentAudioSource.clip;
                }
            }
        }

        public void PlayExpressionSound()
        {
            var randomNumber = Random.Range(0, expressionClips.Count);
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = expressionClips[randomNumber];
                currentAudioSource.Play();
                CurrentExpressionClip = currentAudioSource.clip;
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
                currentAudioSource.volume = .3f;
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

        private void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
        }

    }
}
