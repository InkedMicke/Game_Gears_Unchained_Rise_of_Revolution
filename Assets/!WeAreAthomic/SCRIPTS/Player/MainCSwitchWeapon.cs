using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCSwitchWeapon : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private MainCMovement _mainCMovement;
    private MainCAttack _mainCAttack;

    [SerializeField] private GameObject wrenchObj;
    [SerializeField] private GameObject pistolObj;
    
    public bool isUsingSword = true;
    public bool isUsingPistol;

    private void Awake()
    {
        _mainCAttack = GetComponent<MainCAttack>();
        _mainCMovement = GetComponent<MainCMovement>();
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.SwitchWeapon.performed += SwitchWeapon;
    }


    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (CanSwitchWeapon())
        {
            isUsingSword = !isUsingSword;
            isUsingPistol = !isUsingPistol;
            wrenchObj.SetActive(!wrenchObj.activeInHierarchy);
            pistolObj.SetActive(!pistolObj.activeInHierarchy);
        }
    }

    private bool CanSwitchWeapon()
    {
        if (_mainCAttack.isAttacking)
        {
            return false;}

        if (_mainCMovement._isJumping)
        {
            return false;}

        if (_mainCMovement._isFalling)
        {
            return false;}
        
        return true;
    }
    
}