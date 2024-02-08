using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCSounds : MonoBehaviour
    {
        private MainCMovement _mainCMove;
        private MainCRail _mainCRail;

        private Scene _currentScene;

        [SerializeField] private GameObject soundComponentObj;

        [System.NonSerialized] public AudioClip CurrentExpressionClip;
        [System.NonSerialized] public AudioClip CurrentTutorialClip;

        [SerializeField] private AudioMixerGroup sfxMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup voicesMixer;
        
        [SerializeField] private List<AudioClip> attackClips;
        [Range(0, 1)]
        [SerializeField] private float attackVolume;
        [SerializeField] private List<AudioClip> tutorialClipsPC;
        [SerializeField] private List<AudioClip> tutorialClipsGamepad;
        [Range(0, 1)]
        [SerializeField] private float tutorialVolume;
        [SerializeField] private List<AudioClip> expressionClips;
        [Range(0, 1)]
        [SerializeField] private float expressionVolume;
        [SerializeField] private List<AudioClip> effortClips;
        [Range(0, 1)]
        [SerializeField] private float effortVolume;
        [SerializeField] private List<AudioClip> hurtClips;
        [Range(0, 1)]
        [SerializeField] private float hurtVolume;
        [SerializeField] private List<AudioClip> dieClips;
        [Range(0, 1)]
        [SerializeField] private float dieVolume;
        [SerializeField] private List<AudioClip> jumpClips;
        [Range(0, 1)]
        [SerializeField] private float jumpVolume;
        [SerializeField] private List<AudioClip> stepClipsIndoors;
        [Range(0, 1)]
        [SerializeField] private float stepClipsIndoorsVolume;
        [SerializeField] private List<AudioClip> stepClipsOutdoors;
        [Range(0, 1)]
        [SerializeField] private float stepClipsOutdoorsVolume;

        [SerializeField] private List<AudioClip> callBastetClips;
        [Range(0, 1)] 
        [SerializeField] private float callBasteVolume;

        [SerializeField] private AudioClip railClip;
        [Range(0, 1)]
        [SerializeField] private float railVolume;

      

        [SerializeField] private AudioClip hackInProcess;
        [Range(0, 1)]
        [SerializeField] private float hackInProcessVolume;
        [SerializeField] private AudioClip cannotHack;
        [Range(0, 1)]
        [SerializeField] private float cannotHackVolume;
        [SerializeField] private AudioClip howFight;
        [Range(0, 1)]
        [SerializeField] private float howFightVolume;
        [SerializeField] private AudioClip abilityAtackClip;
        [Range(0, 1)]
        [SerializeField] private float abilityAtackVolume;

        private bool isPaused;

        private void Awake()
        {
            _mainCMove = GetComponent<MainCMovement>();
            _mainCRail = GetComponent<MainCRail>();
        }

        private void Start()
        {
       
        }

        public void StartTutorialSound()
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
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();
            foreach (var audioSour in audiosInSoundComponent)
            {
                if(audioSour.clip == CurrentTutorialClip)
                {
                    Destroy(audioSour);
                }
            }
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
                    currentAudioSource.volume = tutorialVolume;
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
                    currentAudioSource.volume = tutorialVolume;
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
                currentAudioSource.volume = expressionVolume;
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
                currentAudioSource.volume = attackVolume;
                currentAudioSource.Play();
            }
        }
        public void PlayChargedAttackSound()
        {
            
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = abilityAtackClip;
                currentAudioSource.volume = abilityAtackVolume;
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
                currentAudioSource.volume = hackInProcessVolume;
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
                currentAudioSource.volume = cannotHackVolume;
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
                currentAudioSource.volume = howFightVolume;
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

        public void PlayEffordSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            var randomNumber = Random.Range(0, effortClips.Count);

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = effortClips[randomNumber];
                currentAudioSource.volume = effortVolume;
                currentAudioSource.Play();
            }
        }
        public void PlayHurtSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            var randomNumber = Random.Range(0, hurtClips.Count);

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = hurtClips[randomNumber];
                currentAudioSource.volume = hurtVolume;
                currentAudioSource.Play();
            }
        }
        public void PlayBastetCall()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            var randomNumber = Random.Range(0, callBastetClips.Count);

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = callBastetClips[randomNumber];
                currentAudioSource.volume = callBasteVolume;
                currentAudioSource.Play();
            }
        }
        public void PlayDieSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            var randomNumber = Random.Range(0, dieClips.Count);

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = dieClips[randomNumber];
                currentAudioSource.volume = dieVolume;
                currentAudioSource.Play();
            }
        }
        public void PlayRailSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = railClip;
                currentAudioSource.volume = railVolume;
                currentAudioSource.loop = true;
                currentAudioSource.Play();
            }
        }
        public void RemoveRailSounds()
        {
            var audiosInSoundComponent = soundComponentObj.GetComponents<AudioSource>();
            foreach (var audioSour in audiosInSoundComponent)
            {
                if (audioSour.clip == railClip)
                {
                    Destroy(audioSour);
                }
            }
        }
      
        public void PlayJumpSound()
        {
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

            var randomNumber = Random.Range(0, jumpClips.Count);

            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = jumpClips[randomNumber];
                currentAudioSource.volume = jumpVolume;
                currentAudioSource.Play();
            }
        }
        public void PlayStepsIndoorsSound()
        {
            if (_mainCMove.IsGrounded() && !_mainCRail.IsSliding)
            {
                var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;

                var randomNumber = Random.Range(0, stepClipsIndoors.Count);

                if (currentAudioSource != null)//Hay que hacer aqui un IF estamos en suelo interior o suelo exterior
                {
                    currentAudioSource.outputAudioMixerGroup = sfxMixer;
                    currentAudioSource.clip = stepClipsIndoors[randomNumber];
                    currentAudioSource.volume = stepClipsIndoorsVolume;
                    currentAudioSource.Play();
                }
            }
        }
    }
}
