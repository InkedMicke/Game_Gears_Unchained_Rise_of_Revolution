using Enemy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class C_EnemiSounds : MonoBehaviour
{
    SoldierHurtBox m_hurtbox;

    [SerializeField] private GameObject soundComponentObj;

    [System.NonSerialized] public AudioClip CurrentExpressionClip;
    [System.NonSerialized] public AudioClip CurrentTutorialClip;

    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup voicesMixer;


    [SerializeField] private float enemiHitsVolume;
    [SerializeField] private List<AudioClip> enemiHitsClips;

    private void Awake()
    {
        m_hurtbox = GetComponent<SoldierHurtBox>();
    }

    private void OnEnable()
    {
        m_hurtbox.OnHurt += PlayHitEnemiSound;
    }

    private void OnDisable()
    {
        m_hurtbox.OnHurt -= PlayHitEnemiSound;
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
