using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCLayers : MonoBehaviour
{
    private Animator _anim;
    
    private float _attackLayerMultiplier;
    private float _finalAttackLayerMultiplier;
    private float _crouchLayerMultiplier;
    private float _jumpLayerMultiplier;
    private float _pistolLayerMultiplier;
    private float _sphereAttackLayerMultiplier;

    [System.NonSerialized] public bool isAttackLayerActive;
    [System.NonSerialized] public bool isFinalAttackLayerActive;
    [System.NonSerialized] public bool isJumpLayerActive;
    [System.NonSerialized] public bool isCrouchLayerActive;
    [System.NonSerialized] public bool isPistolLayerActive;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateLayers();
        EvaluateLayers();
    }

    private void UpdateLayers()
    {
        _anim.SetLayerWeight(_anim.GetLayerIndex(string.Format("Attack")), _attackLayerMultiplier);
        _anim.SetLayerWeight(_anim.GetLayerIndex(string.Format("FinalAttack")), _finalAttackLayerMultiplier);
        _anim.SetLayerWeight(_anim.GetLayerIndex(string.Format("Crouch")), _crouchLayerMultiplier);
        _anim.SetLayerWeight(_anim.GetLayerIndex(string.Format("Jump")), _jumpLayerMultiplier);
        _anim.SetLayerWeight(_anim.GetLayerIndex(string.Format("Pistol")), _pistolLayerMultiplier);
        _anim.SetLayerWeight(_anim.GetLayerIndex(string.Format("SphereAttack")), _sphereAttackLayerMultiplier);
    }

    private void EvaluateLayers()
    {
        if(Math.Abs(_attackLayerMultiplier - 1f) < 0.1f)
        {
            isAttackLayerActive = true;
        }
        else
        {
            if(isAttackLayerActive)
            {
                isAttackLayerActive = false;
            }
        }

        if (Math.Abs(_crouchLayerMultiplier - 1f) < 0.1f)
        {
            isCrouchLayerActive = true;
        }
        else
        {
            if (isCrouchLayerActive)
            {
                isCrouchLayerActive = false;
            }
        }

        if (Math.Abs(_jumpLayerMultiplier - 1f) < 0.1f)
        {
            isJumpLayerActive = true;
        }
        else
        {
            if (isJumpLayerActive)
            {
                isJumpLayerActive = false;
            }
        }

        if (Math.Abs(_finalAttackLayerMultiplier - 1f) < 0.1f)
        {
            isFinalAttackLayerActive = true;
        }
        else
        {
            if (isFinalAttackLayerActive)
            {
                isFinalAttackLayerActive = false;
            }
        }
        
        if (Math.Abs(_pistolLayerMultiplier - 1f) < 0.1f)
        {
            isPistolLayerActive = true;
        }
        else
        {
            if (isPistolLayerActive)
            {
                isPistolLayerActive = false;
            }
        }
    }

    public void EnableAttackLayer()
    {
        StartCoroutine(nameof(EnableAttackCoroutine));
    }
    
    public void DisableAttackLayer()
    {
        StartCoroutine(nameof(DisableAttackCoroutine));
    }

    public void DisableFinalAttackLayer()
    {
        StartCoroutine(nameof(DisableFinalAttackCoroutine));
    }

    public void EnableCrouchLayer()
    {
        StartCoroutine(nameof(EnableCrouchCoroutine));
    }
    
    public void DisableCrouchLayer()
    {
        StartCoroutine(nameof(DisableCrouchCoroutine));
    }
    
    public void EnableJumpLayer()
    {
        StartCoroutine(nameof(EnableJumpCoroutine));
    }
    
    public void DisableJumpLayer()
    {
        StartCoroutine(nameof(DisableJumpCoroutine));
    }
    
    public void EnablePistolLayer()
    {
        StartCoroutine(nameof(EnablePistolCoroutine));
    }
    
    public void DisablePistolLayer()
    {
        StartCoroutine(nameof(DisablePistolCoroutine));
    }
    
    public void EnableSphereAttackLayer()
    {
        StartCoroutine(nameof(EnableSphereAttackCoroutine));
    }
    
    public void DisableSphereAttackLayer()
    {
        StartCoroutine(nameof(DisableSphereAttackCoroutine));
    }

    private IEnumerator EnableAttackCoroutine()
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            _attackLayerMultiplier += 24 * Time.unscaledDeltaTime;

            if (_attackLayerMultiplier >= 1f)
            {
                _attackLayerMultiplier = 1f;
                canEnableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator DisableAttackCoroutine()
    {
        var canDisableLayer = true;

        while (canDisableLayer)
        {
            _attackLayerMultiplier -= 24 * Time.unscaledDeltaTime;

            if (_attackLayerMultiplier <= 0f)
            {
                _attackLayerMultiplier = 0f;
                canDisableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator EnableFinalAttackCoroutine()
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            _finalAttackLayerMultiplier += 24 * Time.unscaledDeltaTime;

            if (_finalAttackLayerMultiplier >= 1f)
            {
                _finalAttackLayerMultiplier = 1f;
                canEnableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator DisableFinalAttackCoroutine()
    {
        var canDisableLayer = true;

        while (canDisableLayer)
        {
            _finalAttackLayerMultiplier -= 24 * Time.unscaledDeltaTime;

            if (_finalAttackLayerMultiplier <= 0f)
            {
                _finalAttackLayerMultiplier = 0f;
                canDisableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator EnableCrouchCoroutine()
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            _crouchLayerMultiplier += 24 * Time.unscaledDeltaTime;

            if (_crouchLayerMultiplier >= 1f)
            {
                _crouchLayerMultiplier = 1f;
                canEnableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator DisableCrouchCoroutine()
    {
        var canDisableLayer = true;

        while (canDisableLayer)
        {
            _crouchLayerMultiplier -= 24 * Time.unscaledDeltaTime;

            if (_crouchLayerMultiplier <= 0f)
            {
                _crouchLayerMultiplier = 0f;
                canDisableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator EnableJumpCoroutine()
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            _jumpLayerMultiplier += 24 * Time.unscaledDeltaTime;

            if (_jumpLayerMultiplier >= 1f)
            {
                _jumpLayerMultiplier = 1f;
                canEnableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator DisableJumpCoroutine()
    {
        var canDisableLayer = true;

        while (canDisableLayer)
        {
            _jumpLayerMultiplier -= 24 * Time.unscaledDeltaTime;

            if (_jumpLayerMultiplier <= 0f)
            {
                _jumpLayerMultiplier = 0f;
                canDisableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator EnablePistolCoroutine()
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            _pistolLayerMultiplier += 24 * Time.unscaledDeltaTime;

            if (_pistolLayerMultiplier >= 1f)
            {
                _pistolLayerMultiplier = 1f;
                canEnableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator DisablePistolCoroutine()
    {
        var canDisableLayer = true;

        while (canDisableLayer)
        {
            _pistolLayerMultiplier -= 24 * Time.unscaledDeltaTime;

            if (_pistolLayerMultiplier <= 0f)
            {
                _pistolLayerMultiplier = 0f;
                canDisableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator EnableSphereAttackCoroutine()
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            _sphereAttackLayerMultiplier += 24 * Time.unscaledDeltaTime;

            if (_sphereAttackLayerMultiplier >= 1f)
            {
                _sphereAttackLayerMultiplier = 1f;
                canEnableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private IEnumerator DisableSphereAttackCoroutine()
    {
        var canDisableLayer = true;

        while (canDisableLayer)
        {
            _sphereAttackLayerMultiplier -= 24 * Time.unscaledDeltaTime;

            if (_sphereAttackLayerMultiplier <= 0f)
            {
                _sphereAttackLayerMultiplier = 0f;
                canDisableLayer = false;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}
