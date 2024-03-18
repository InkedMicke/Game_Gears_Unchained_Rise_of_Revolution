using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Generics.Camera;

namespace Player
{
    public class MainCFuryAttack : MonoBehaviour
    {
        private MainCAttack _mainCAttack;
        private MainCMovement _mainCMovement;
        private PlayerInputActions _inputActions;
        private MainCLayers _mainCLayers;
        private MainCAnimatorController _mainCAnimator;
        private MainCSounds _mainCSounds;
        private CharacterController _cc;
        private MainCTutorialChecker _mainCTutorial;
        private MainCWrenchHitBox _wrenchHitBox;
        private MainCPistol _mainCPistol;
        private MainCPlayerInterface _mainCInterface;
        private MainCVFX _mainCVfx;
        [SerializeField] GCameraShake cameraShake;

        [SerializeField] private HealthManagerSO furyManager;

        [SerializeField] private PlayerDamageData furyDamageData;
        [SerializeField] private Material lowRangeMaterial;
        [SerializeField] private Material maxRangeMaterial;
        public GameObject weaponObj;
        [NonSerialized] public GameObject scannerInst;
        [SerializeField] private GameObject scannerPrefab;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private Transform explosionEffectPosition;
        [SerializeField] private Transform groundTr;
        [SerializeField] private CinemachineVirtualCamera cameraBase;

        [SerializeField] private bool IsFullFury;

        [SerializeField] private float scannerSizeSpeed = .1f;
        [SerializeField] private float scannerSize = 15f;
        private float _mouseMagnitude;

        [NonSerialized] public bool IsFinalAttacking;

        public bool IsChargingAttack;
        private bool _isLeftMousePressed;
        private bool _hasUnlockedAbilityAttack;


        private void Awake()
        {
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _cc = GetComponent<CharacterController>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCPistol = GetComponent<MainCPistol>();
            _wrenchHitBox = weaponObj.GetComponent<MainCWrenchHitBox>();
            _mainCInterface = GetComponent<MainCPlayerInterface>();
            _mainCVfx = GetComponent<MainCVFX>();
            _mainCAttack = GetComponent<MainCAttack>();


            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
            _inputActions.PlayerPC.Attack.performed += MouseDown;
            _inputActions.PlayerPC.Attack.canceled += MouseUp;
            _inputActions.PlayerGamepad.Attack.performed += GamepadDown;
            _inputActions.PlayerGamepad.Attack.canceled += GamepadUp;
        }

   

        private void Update()
        {
            if (_isLeftMousePressed)
            {
                _mouseMagnitude += Time.deltaTime;
            }

            ChargeAttack();
        }

        private void GamepadDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {
                ControlDown();
            }
        }

        private void GamepadUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {
                ControlUp();
            }
        }

        private void MouseDown(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                ControlDown();
            }
        }

        private void MouseUp(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                ControlUp();
            }
        }

        private void ChargeAttack()
        {
            if (CanChargeAttack())
            {
                furyManager.DecreaseHealth(100f);
                _mainCLayers.EnableAbilityAttackLayer();
                _mainCAttack.SetAttackCount(5);
                _mainCMovement.DisableMovement();
                scannerInst = Instantiate(scannerPrefab, groundTr.position, Quaternion.identity);
                scannerInst.transform.localScale = Vector3.one * scannerSize;
                IsChargingAttack = true;
            }
        }
        public void ApplyAbilityDamage()
        {
            scannerInst.GetComponent<ScannerHitBox>().ApplyDamage(furyDamageData);
            _mainCVfx.ActivateSlash4();
            InstanciateExplosion();
            _mainCSounds.PlayChargedAttackSound();
            cameraShake.ShakeCamera(5f, 1f,.1f);

        }
        public void DestroyScanner()
        {
            Destroy(scannerInst);
        }
        public void SetHasUnlockedAbilityAttack(bool condition)
        {
            _hasUnlockedAbilityAttack = condition;
        }
    
        public void InstanciateExplosion()
        {
            Instantiate(explosionEffect, explosionEffectPosition.position, Quaternion.identity);

        }

    

        public void ControlUp()
        {
            _isLeftMousePressed = false;
            _mouseMagnitude = 0;
        }
        private void ControlDown()
        {
            _isLeftMousePressed = true;

        }

        private bool CanChargeAttack()
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
            if(furyManager.CurrentHealth < 100) { return false; }
            if(!_hasUnlockedAbilityAttack) { return false; }

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
        }
    }

}

