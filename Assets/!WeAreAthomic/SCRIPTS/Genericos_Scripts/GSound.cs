using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceTwo;
    [SerializeField] private AudioClip inClip;
    [SerializeField] private AudioClip bucleClip;

    private double musicDuration;
    private double goalTime;

    private bool _isInClipPlayed;
    private bool _isBucleClipPlayed;
    

    protected void Start()
    {
        OnPlayMusic();
    }

    private void OnPlayMusic()
    {
        goalTime = AudioSettings.dspTime + .5f;

        audioSource.clip = inClip;
        _isInClipPlayed = true;
        audioSource.PlayScheduled(goalTime);

        musicDuration = (double)inClip.samples / inClip.frequency;
        goalTime += musicDuration;

    }

    private void Update()
    {
        if(GameManagerSingleton.Instance.IsGamePaused)
        {
            if(_isInClipPlayed && audioSource.isPlaying)
            {
                audioSource.Pause();
            }         
            
            if(_isBucleClipPlayed && audioSourceTwo.isPlaying)
            {
                audioSourceTwo.Pause();
            }
        }

        if (!GameManagerSingleton.Instance.IsGamePaused)
        {
            if (_isInClipPlayed && !audioSource.isPlaying)
            {
                audioSource.UnPause();
            }

            if (_isBucleClipPlayed && !audioSourceTwo.isPlaying)
            {
                audioSourceTwo.UnPause();
            }
        }

        if (AudioSettings.dspTime > goalTime - 1)
        {
            audioSourceTwo.clip = bucleClip;
            audioSourceTwo.PlayScheduled(goalTime);
            _isBucleClipPlayed = true;
            _isInClipPlayed = false;

            musicDuration = (double)bucleClip.samples / bucleClip.frequency;
            goalTime = AudioSettings.dspTime + musicDuration;
        }
    }
}
