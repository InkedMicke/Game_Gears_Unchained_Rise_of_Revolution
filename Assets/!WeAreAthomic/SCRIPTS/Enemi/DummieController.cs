using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummieController : MonoBehaviour
{
    private Animator _anim;
    private GHealthManager _gHealthManager;

    private void Awake()
    {
        _gHealthManager = GetComponent<GHealthManager>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_gHealthManager.currentHealth <= 0)
        {
            _anim.SetBool(string.Format("isDeath"), true);
        }
    }

    public void HurtAnim()
    {
        _anim.SetTrigger(string.Format("isHurt"));
    }


}
