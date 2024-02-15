using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.TEST
{
    public class Test : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private MainCHealthManager _mainCHealthManager;

        [SerializeField] private GameObject stopMenu;

        private void Awake()
        {
            _mainCHealthManager = GetComponentInChildren<MainCHealthManager>();


        }

        public void DebugLog(string message)
        {
            Debug.Log(message);
        }
    }
}

