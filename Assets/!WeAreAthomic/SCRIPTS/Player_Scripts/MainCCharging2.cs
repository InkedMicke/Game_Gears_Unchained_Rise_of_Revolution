using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCCharging2 : MonoBehaviour
{
    #region Variables
    MainCAttack m_mainCAttack;
    MainCMovement m_mainCMovement;
    MainCInput m_input;

    [NonSerialized] public bool IsFuryAttacking;
    bool m_isLeftMousePressed;

    [SerializeField] float buttonPressedTimeToAttack = .4f;
    float m_leftMousePressedTime;
    #endregion

    #region Initialization
    private void Awake()
    {
        m_input = GetComponent<MainCInput>();

        //m_input.OnLeftMouseDown 
    }

    private void OnEnable()
    {
        //_inputActions.Enable();
    }
    #endregion

    #region Control
    void ControlDown() => m_isLeftMousePressed = true;
    void ControlUp() => m_isLeftMousePressed = false;
    #endregion

    #region GameLogic

    #region Update
    private void Update()
    {
        if (m_isLeftMousePressed)
        {
            m_leftMousePressedTime += Time.deltaTime;
            if(m_leftMousePressedTime > buttonPressedTimeToAttack)
            {
                IsFuryAttacking = true;
            }
        }
    }
    #endregion

    #region Attack

    #endregion

    #endregion

    #region Bools
/*    private bool CanChargeAttack()
    {

        if (IsChargingAttack)
        {
            return false;
        }

        if (_mouseMagnitude < 1f)
        {
            return false;
        }

        if (!_mainCAttack._isSheathed)
        {
            return false;
        }

        if (_mainCPistol.IsAiming)
        {
            return false;
        }

        if (_mainCAttack.IsAttacking) { return false; }
        if (furyManager.CurrentHealth < 100) { return false; }
        if (!_hasUnlockedAbilityAttack) { return false; }

        if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
        {
            return false;
        }

        if (GameManagerSingleton.Instance.IsStopMenuEnabled)
        {
            return false;
        }

        if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
        {
            return false;
        }

        if (GameManagerSingleton.Instance.IsOnDialogue)
        {
            return false;
        }

        if (GameManagerSingleton.Instance.IsOnDialogue)
        {
            return false;
        }

        return true;
    }*/
    #endregion

    #region Decommissioning
    private void OnDisable()
    {
        //_inputActions.Disable();
    }
    #endregion
}
