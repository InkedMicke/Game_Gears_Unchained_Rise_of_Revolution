using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip inClip;
    [SerializeField] AudioClip bucleClip;

    protected void Start()
    {
        StartCoroutine(MusicSound());
    }

    IEnumerator MusicSound()
    {
        audioSource.clip = inClip;
        audioSource.Play();

        yield return new WaitForSecondsRealtime(inClip.length);

        audioSource.clip = bucleClip;
        audioSource.Play();
        audioSource.loop = true;
    }
}
