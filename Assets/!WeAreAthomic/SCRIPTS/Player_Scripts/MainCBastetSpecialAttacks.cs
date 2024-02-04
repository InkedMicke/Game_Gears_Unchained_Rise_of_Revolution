using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainCBastetSpecialAttacks : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    [SerializeField] private Image genericImage;

    private int _currentItem;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.PlayerPC.ChangeAbility.performed += NextAttackPC;
        _inputActions.PlayerGamepad.ChangeAbility.performed += NextAttackGamepad;

        ResfreshAbilitiesSprites();
    }

    private void NextAttackPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
        {
            _currentItem = GetIndex();
            StartCoroutine(NextAttack());
        }
    }

    private void NextAttackGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
        {
            _currentItem = GetIndex();
            StartCoroutine(NextAttack());
        }
    }

    private IEnumerator NextAttack()
    {
        if(_currentItem == GameManagerSingleton.Instance.abiltiesList.Count - 1)
        {
            _currentItem = 0;
        }
        
        while(true)
        {
            _currentItem++;
            if(_currentItem > GameManagerSingleton.Instance.abiltiesList.Count - 1)
            {
                _currentItem = 0;
            }
            if(GameManagerSingleton.Instance.abiltiesList[_currentItem].IsUnlocked)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        genericImage.sprite = GameManagerSingleton.Instance.abiltiesList[_currentItem].sprite;
        GameManagerSingleton.Instance.currentAbility = GameManagerSingleton.Instance.abiltiesList[_currentItem].currentAbility;
    }

    public void ResfreshAbilitiesSprites()
    {
        foreach (var x in GameManagerSingleton.Instance.abiltiesList)
        {
            if (x.currentAbility == GameManagerSingleton.Instance.currentAbility)
            {
                genericImage.sprite = x.sprite;
                _currentItem = x.value;
            }
        }
    }

    private int GetIndex()
    {
        foreach (var x in GameManagerSingleton.Instance.abiltiesList)
        {
            if (x.currentAbility == GameManagerSingleton.Instance.currentAbility)
            {
                return x.value; 
            }
        }

        return 0;
    }
}
