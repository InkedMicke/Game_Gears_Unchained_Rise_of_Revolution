using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIntroToLoop : MonoBehaviour
{
    [SerializeField] private AudioSource introSource;
    [SerializeField] private AudioSource loopSource;

    private bool isPlayed;

    private void Update()
    {
        if(introSource.time >= (introSource.clip.length - .45f) && !isPlayed)
        {
            loopSource.Play();
            isPlayed = true;
            Debug.Log("hola2");
        }
    }

}
