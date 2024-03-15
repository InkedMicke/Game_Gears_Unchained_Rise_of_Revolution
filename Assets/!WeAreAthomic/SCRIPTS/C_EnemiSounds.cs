using Broom;
using Enemy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Enemy
{
    public class C_EnemiSounds : MonoBehaviour
    {
        SoldierHurtBox m_soldierHurtbox;
        BroomHurtBox m_broomHurtbox;

        [SerializeField] private GameObject soundComponentObj;

        [System.NonSerialized] public AudioClip CurrentExpressionClip;
        [System.NonSerialized] public AudioClip CurrentTutorialClip;

        [SerializeField] private AudioMixerGroup sfxMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup voicesMixer;

        bool m_usingSoldierHurtBox;

        [SerializeField] private float enemiHitsVolume;
        [SerializeField] private List<AudioClip> enemiHitsClips;

        private void Awake()
        {
            if (m_soldierHurtbox.GetComponent<SoldierHurtBox>() != null)
            {
                m_soldierHurtbox = GetComponent<SoldierHurtBox>();
                m_usingSoldierHurtBox = true;
            }
            else
                m_broomHurtbox = GetComponent<BroomHurtBox>();
        }

        private void OnEnable()
        {
            if (m_usingSoldierHurtBox)
                m_soldierHurtbox.OnHurt += PlayHitEnemiSound;
            else
                m_broomHurtbox.OnHurt += PlayHitEnemiSound;
        }

        private void OnDisable()
        {
            if (m_usingSoldierHurtBox)
                m_soldierHurtbox.OnHurt -= PlayHitEnemiSound;
            else
                m_broomHurtbox.OnHurt -= PlayHitEnemiSound;
        }

        public void PlayHitEnemiSound()
        {
            var randomNumber = Random.Range(0, enemiHitsClips.Count);
            var currentAudioSource = soundComponentObj.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentAudioSource != null)
            {
                currentAudioSource.outputAudioMixerGroup = sfxMixer;
                currentAudioSource.clip = enemiHitsClips[randomNumber];
                currentAudioSource.volume = enemiHitsVolume;
                currentAudioSource.Play();

            }
        }
    }
}