using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MainC360Attack : MonoBehaviour
{
    PlayerInputActions _playerInputActions;
    private BastetController _bastet;

    [SerializeField] private GameObject bastetObj;
    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
        _bastet = bastetObj.GetComponent<BastetController>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.PlayerPC.BastetAttack.performed += InputPC;
        _playerInputActions.PlayerGamepad.BastetAttack.performed += InputGamepad;
    }

    private void InputPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.ThreeSixtyAttack)
        {
            StartAttacking();
        }
            
    }

    private void InputGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.ThreeSixtyAttack)
        {
            StartAttacking();
        }
            
    }

    private void StartAttacking()
    {
        _bastet.GoToDesiredPos();
    }
}
