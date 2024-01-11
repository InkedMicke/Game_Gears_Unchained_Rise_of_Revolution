using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainVFXCharacter : MonoBehaviour
{
    private MainCMovement _mainCMove;

    [Header("Steps")]
    [SerializeField] private GameObject vfxStep;

    public Transform rightFoot;
    public Transform leftFoot;
    public Transform edgeWrench;

    [Header("Slash")]
    [SerializeField] private ParticleSystem slash1;
    [SerializeField] private ParticleSystem slash2;
    [SerializeField] private ParticleSystem slash3;
    [SerializeField] private ParticleSystem slash4;

    [Header("Rail")]

    [SerializeField] private GameObject vfxSparks;
    [SerializeField] private GameObject vfxSpeedLines;

    [Header("Chispas")]
    [SerializeField] private GameObject chispasVFX;

    private void Awake()
    {
        _mainCMove = GetComponent<MainCMovement>();
    }







    public void ActivateRightStep()
    {
        if (_mainCMove.IsGrounded())
        {
            Instantiate(vfxStep, rightFoot.position, Quaternion.identity);
        }
    }
    public void ActivateLeftStep()
    {
        if (_mainCMove.IsGrounded())
        {
            Instantiate(vfxStep, leftFoot.position, Quaternion.identity);
        }
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
    
    public void ActivateSlash4()
    {
        slash4.Play();
        var chispas = Instantiate(chispasVFX, edgeWrench.position, Quaternion.identity);
        chispas.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        Destroy(chispas, 1f);
    }

    public void ToggleRail()
    {
        var  active = new bool();

        active = !active;

        vfxSparks.SetActive(active);
        vfxSpeedLines.SetActive(active);
    }

}
