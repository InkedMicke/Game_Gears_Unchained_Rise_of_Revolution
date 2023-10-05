using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummieHurtBox : MonoBehaviour
{
    private GHealthManager _gHealthManager;
    private Animator _anim;
    private CharacterController _cc;

    private Transform playerTr;

    [SerializeField] private ParticleSystem sparksHit;

    [SerializeField] private float pushForce = 5f;
    
    private void Awake()
    {
        _gHealthManager = GetComponentInParent<GHealthManager>();
        _anim = GetComponentInParent<Animator>();
        _cc = GetComponentInParent<CharacterController>();
    }

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void TakeDamage(float value)
    {
        _gHealthManager.currentHealth -= value;
        _anim.SetTrigger(string.Format("isHurt"));
        sparksHit.Play();
        PushBack();
    }

    public void PushBack()
    {
        var pushDirection = (playerTr.position - transform.position).normalized;
        _cc.SimpleMove(-(pushDirection) * pushForce);

    }
}
