using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummieSounds : MonoBehaviour
{
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
            currentAudioSource.Play();
        }
    }
}
