using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
   public class MainCMouseController : MonoBehaviour
   {
      protected enum TypeOfAttack
      {
         NormalAttack,
         ChargedAttack
      }

      protected TypeOfAttack _typeOfAttack;

      private PlayerInputActions _playerInputActions;

      private bool _isMouseDown;

      private float _mouseMagnitude;

      private protected virtual void Awake()
      {
         _playerInputActions = new PlayerInputActions();
         _playerInputActions.Enable();
         _playerInputActions.Player.Attack.performed += MousePressedDown;
         _playerInputActions.Player.Attack.canceled += MousePressedUp;


      }

      private void Start()
      {
         StartCoroutine(nameof(TestCoroutine));
      }

      private IEnumerator TestCoroutine()
      {
         while (true)
         {
            if (_isMouseDown)
            {
               _mouseMagnitude += Time.deltaTime;

            }

            if (_mouseMagnitude > .5f)
            {
               _typeOfAttack = TypeOfAttack.ChargedAttack;
            }
            else
            {
               _typeOfAttack = TypeOfAttack.NormalAttack;
            }
            yield return new WaitForSeconds(.01f);
         }
      }

      private void MousePressedUp(InputAction.CallbackContext context)
      {
         _mouseMagnitude = 0f;
         _isMouseDown = false;
      }

      private void MousePressedDown(InputAction.CallbackContext context)
      {
         _isMouseDown = true;
      }
   }
}
