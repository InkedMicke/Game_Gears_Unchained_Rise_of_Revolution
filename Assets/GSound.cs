using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceTwo;
    [SerializeField] AudioClip inClip;
    [SerializeField] AudioClip bucleClip;

    private double musicDuration;
    private double goalTime;
    

    protected void Start()
    {
        OnPlayMusic();
    }

    private void OnPlayMusic()
    {
        goalTime = AudioSettings.dspTime + .5f;

        audioSource.clip = inClip;
        audioSource.PlayScheduled(goalTime);

        musicDuration = (double)inClip.samples / inClip.frequency;
        goalTime += musicDuration;

    }

    private void Update()
    {
        if(AudioSettings.dspTime > goalTime - 1)
        {
            audioSourceTwo.clip = bucleClip;
            audioSourceTwo.PlayScheduled(goalTime);

            musicDuration = (double)bucleClip.samples / bucleClip.frequency;
            goalTime = AudioSettings.dspTime + musicDuration;
        }
    }
}
