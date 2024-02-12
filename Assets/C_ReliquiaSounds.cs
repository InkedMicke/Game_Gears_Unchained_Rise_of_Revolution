using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ReliquiaSounds : MonoBehaviour
{
    [SerializeField] private List<AudioClip> reliquiaSounds;
    [Range (0,1)]
    [SerializeField] private float reliquiavolume;

    private AudioSource audioSource;

    [SerializeField] private TypeofSoundReliquia typeofSoundReliquia;
    enum TypeofSoundReliquia
        {
        verdes,
        naranjas,
        rojos,
        Azul,
        Seth
        }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySoundReliquia()
    { 
    switch(typeofSoundReliquia)
        {
            case TypeofSoundReliquia.verdes:
                audioSource.clip = reliquiaSounds[0];
                audioSource.Play();
                break;

            case TypeofSoundReliquia.naranjas:
                audioSource.clip = reliquiaSounds[1];
                audioSource.Play();
                break;

            case TypeofSoundReliquia.rojos:
                audioSource.clip = reliquiaSounds[2];
                audioSource.Play();
                break;

            case TypeofSoundReliquia.Azul:
                audioSource.clip = reliquiaSounds[3];
                audioSource.Play();
                break;

            case TypeofSoundReliquia.Seth:
                audioSource.clip = reliquiaSounds[4];
                audioSource.Play();
                break;

        }
            
    }
}
