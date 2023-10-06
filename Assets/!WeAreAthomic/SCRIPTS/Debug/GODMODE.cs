using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GODMODE : MonoBehaviour
{
    private CharacterController _cc;
    private PlayerInputActions _playerInputActions;

    private GameObject _cameraObj;
    
    public bool isGodModeEnabled;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.GodMode.performed += EnableGodMode;
    }

    private void Start()
    {
        _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void EnableGodMode(InputAction.CallbackContext context)
    {
        
    }
}
