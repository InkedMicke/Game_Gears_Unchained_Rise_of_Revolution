using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Splines;

public class MainCRail : MonoBehaviour
{
    private SplineAnimate _splineAnim;
    private MainCAnimatorController _mainCAnim;

    [SerializeField] private LayerMask railLayer;

    [SerializeField] private GameObject splineFollower;
    [SerializeField] private GameObject PP;

    [SerializeField] private Transform railCheck;

    [System.NonSerialized] public bool IsSliding;

    private void Awake()
    {
        _mainCAnim = GetComponent<MainCAnimatorController>();
    }


    private void Update()
    {
        if(IsOnRail() && !IsSliding)
        {
            StartSlide();
        }
    }

    private void StartSlide()
    {
        IsSliding = true;
        transform.parent = splineFollower.transform;
        _splineAnim = splineFollower.GetComponent<SplineAnimate>();
        _splineAnim.Play();
        _mainCAnim.SetMoveSpeed(0);
    }

    public bool IsOnRail()
    {
        return Physics.CheckSphere(railCheck.position, .1f, railLayer);
    }
}
