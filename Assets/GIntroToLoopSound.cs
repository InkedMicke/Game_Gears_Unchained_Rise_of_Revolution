using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIntroToLoopSound : MonoBehaviour
{
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioSource loop;

    private PlayerInputActions _playerInput;

    private void Start()
    {
        var desiredLength = intro.length - 2.35f;
        Invoke(nameof(PlayLoop), desiredLength);
        Debug.Log(intro.length - 1f);
    }

    private void PlayLoop()
    {
        loop.Play();
    }
}
