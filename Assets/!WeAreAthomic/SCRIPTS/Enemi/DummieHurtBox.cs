using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummieHurtBox : MonoBehaviour
{
    private GHealthManager _gHealthManager;
    private Animator _anim;

    [SerializeField] private ParticleSystem sparksHit;
    
    private void Awake()
    {
        _gHealthManager = GetComponentInParent<GHealthManager>();
        _anim = GetComponentInParent<Animator>();
    }

    public void TakeDamage(float value)
    {
        _gHealthManager.currentHealth -= value;
        _anim.SetTrigger(string.Format("isHurt"));
        sparksHit.Play();
    }
}
