using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainCBastetSpecialAttacks : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    [SerializeField] private Image genericImage;
    [SerializeField] private Image rafagaImage;
    [SerializeField] private Image rafaga360Image;
    [SerializeField] private Image protectionImage;
    [SerializeField] private Image healImage;

    private int _currentItem;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.PlayerPC.ChangeAbility.performed += NextAttackPC;
        _inputActions.PlayerGamepad.ChangeAbility.performed += NextAttackGamepad;
    }

    private void NextAttackPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
        {
            NextAttack();
        }
    }

    private void NextAttackGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
        {
            NextAttack();
        }
    }

    private void NextAttack()
    {
        if(_currentItem == GameManagerSingleton.Instance.abiltiesList.Count - 1)
        {
            _currentItem = 0;
        }
        _currentItem++;
        foreach(var x in GameManagerSingleton.Instance.abiltiesList)
        {
            if(x.value == _currentItem)
            {
                GameManagerSingleton.Instance.currentAbility = x.currentAbility;
                genericImage.sprite = x.sprite;
            }
        }
    }
}
