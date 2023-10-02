using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainCAttack : MonoBehaviour
{
    private MainCMovement _mainCMovement;
    private PlayerInputActions _playerInputActions;
    private MainCLayers _mainCLayers;
    private Animator _anim;
    private BoxCollider _weaponBC;

    [SerializeField] private GameObject weaponObj;

    [System.NonSerialized] public bool isAttacking;
    [System.NonSerialized] public bool isFinalAttacking;
    [System.NonSerialized] public bool canDealDamage;
    private bool _clickedOnTime;
    private bool _canNextAttack;

    public int attackCount;

    [SerializeField] private float timeNextAttack = 0.5f;
    private float _timeGraceAttackPeriod;

    private void Awake()
    {
        _mainCMovement = GetComponent<MainCMovement>();
        _mainCLayers = GetComponent<MainCLayers>();
        _anim = GetComponent<Animator>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Attack.performed += Attack;
        _playerInputActions.Player.Attack.performed += CanNextCombo;

        _weaponBC = weaponObj.GetComponent<BoxCollider>();
    }

    /*private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Running.performed += Attack;
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
        _playerInputActions.Player.Running.performed -= Attack;
    }*/

    private void Attack(InputAction.CallbackContext context)
    {
        if (CanAttack())
        {
            attackCount++;
            weaponObj.GetComponent<WrenchHitBox>().ClearList();
            _canNextAttack = false;
            if (_mainCLayers.isJumpLayerActive)
            {
                _mainCLayers.DisableJumpLayer();
            }
            isAttacking = true;
            _mainCLayers.EnableAttackLayer();
            _anim.SetInteger(string.Format("attackCount"), attackCount);
            _timeGraceAttackPeriod = Time.time + timeNextAttack;

            _canNextAttack = false;
            isFinalAttacking = true;
            _anim.SetInteger(string.Format("attackCount"), attackCount);
        
        }
    }

    private void CanNextCombo(InputAction.CallbackContext context)
    {
        if (_clickedOnTime)
        {
            _canNextAttack = true;
        }
    }

    public void NextCombo()
    {
        if (_canNextAttack)
        {
            attackCount++;
            _anim.SetInteger(string.Format("attackCount"), attackCount);
            _canNextAttack = false;
        }
    }

    public void EnableNextAttack()
    {
        _clickedOnTime = true;
    }

    public void DisableNextAttack()
    {
        if (!_canNextAttack)
        {
            _clickedOnTime = false;
        }
    }

    private void EndAttack()
    {
        isFinalAttacking = false;
        isAttacking = false;
        attackCount = 0;
        _anim.SetInteger(string.Format("attackCount"), attackCount);
        _mainCLayers.DisableAttackLayer();
        _timeGraceAttackPeriod = Time.time + timeNextAttack;
        
    }

    private void EndFinalAttack()
    {
        _mainCLayers.DisableFinalAttackLayer();
        isFinalAttacking = false;
        isAttacking = false;
        attackCount = 0;
        _anim.SetInteger(string.Format("attackCount"), attackCount);
        _mainCLayers.DisableAttackLayer();

    }

    private void EnableWeaponCollision()
    {
        weaponObj.GetComponent<BoxCollider>().enabled = true;
    }
    
    private void DisableWeaponCollision()
    {
        weaponObj.GetComponent<BoxCollider>().enabled = false;
        weaponObj.GetComponent<WrenchHitBox>().ClearList();
    }

    private bool CanAttack()
    {
        if (!_mainCMovement.IsGrounded()) { return false; }
        if (isAttacking) { return false; }
        if (_mainCMovement.isCrouch) { return false; }
        if (!(Time.time > _timeGraceAttackPeriod)) { return false; }

        return true;
    }
}
