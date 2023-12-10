using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainVFXCharacter : MonoBehaviour
{
    [Header("Steps")]
    [SerializeField] private GameObject vfxStep;

    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;

    [Header("Slash")]
    [SerializeField] private ParticleSystem slash1;
    [SerializeField] private ParticleSystem slash2;
    [SerializeField] private ParticleSystem slash3;


    




    public void ActivateRightStep()
    {
        Instantiate(vfxStep, rightFoot.position,Quaternion.identity);
    }
    public void ActivateLeftStep()
    {
        Instantiate(vfxStep, leftFoot.position, Quaternion.identity);
    }
    public void ActivateSlash1()
    {
        slash1.Play();
    }
    public void ActivateSlash2()
    {
        slash2.Play();
    }
    public void ActivateSlash3()
    {
        slash3.Play();
    }

}
