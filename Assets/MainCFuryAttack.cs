using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using System;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
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
        private WrenchHitBox _wrenchHitBox;
        private MainCPistol _mainCPistol;
        private MainCPlayerInterface _mainCInterface;
        private MainCVFX _mainCVfx;

        [SerializeField] private float maxFury;
        private float currentFury;
        private float furySpend;

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
        public float furyPerHit = 20f;
        private float _mouseMagnitude;

        [NonSerialized] public bool IsFinalAttacking;

        public bool IsChargingAttack;
        private bool _isLeftMousePressed;
        private bool _hasUnlockedAbilityAttack;
        private bool _runOutFury;
        private bool _canAbilityAttack = true;

        private void Awake()
        {
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _cc = GetComponent<CharacterController>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCPistol = GetComponent<MainCPistol>();
            _wrenchHitBox = weaponObj.GetComponent<WrenchHitBox>();
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

        private void Start()
        {
            GetFury(100f);
        }

        private void Update()
        {
            if (_isLeftMousePressed)
            {
                _mouseMagnitude += Time.deltaTime;
            }

            ChargeAttack();
            if (IsChargingAttack && scannerInst != null)
            {
                ChangeMaterialScanner();
            }
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

        private void ChangeMaterialScanner()
        {
            if (scannerInst.transform.localScale.x >= scannerSize - .5f)
            {
                if (scannerInst.GetComponent<MeshRenderer>().material != maxRangeMaterial)
                {
                    scannerInst.GetComponent<MeshRenderer>().material = maxRangeMaterial;
                }
            }
        }
        private void ChargeAttack()
        {
            if (CanChargeAttack() && _hasUnlockedAbilityAttack && GameManagerSingleton.Instance.bastetFury == 100 && !_mainCAttack.IsAttacking && !GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                GetFury(-100);
                _mainCLayers.EnableAbilityAttackLayer();
                _mainCAttack.SetAttackCount(4);
                _mainCMovement.DisableMovement();
                weaponObj.SetActive(true);
                scannerInst = Instantiate(scannerPrefab, groundTr.position, Quaternion.identity);
                IsChargingAttack = true;
            }

            if (_mouseMagnitude > 1.1f && _isLeftMousePressed && _hasUnlockedAbilityAttack && !_mainCAttack.IsAttacking && scannerInst != null)
            {
                if (scannerInst.transform.localScale.x < scannerSize)
                {
                    scannerInst.transform.localScale += Vector3.one * scannerSizeSpeed;

                }
                else
                {
                    scannerInst.transform.localScale = Vector3.one * scannerSize;
                }
            }

            if(IsChargingAttack && scannerInst.transform.localScale.x >= scannerSize)
            {
                _mainCAttack.SetAttackCount(5);
                IsChargingAttack = false;
            }
        }
        public void ApplyAbilityDamage()
        {
            scannerInst.GetComponent<ScannerHitBox>().ApplyDamage(furyDamageData);
            _mainCVfx.ActivateSlash4();
            InstanciateExplosion();
            _mainCSounds.PlayChargedAttackSound();
            GCameraShake.Instance.ShakeCamera(5f, .1f);

        }
        public void DestroyScanner()
        {
            Destroy(scannerInst);
        }
        public void SetHasUnlockedAbilityAttack(bool condition)
        {
            _hasUnlockedAbilityAttack = condition;
        }
        public void SetRunOutEnergy(bool condition)
        {
            _runOutFury = condition;
        }
        public void InstanciateExplosion()
        {
            Instantiate(explosionEffect, explosionEffectPosition.position, Quaternion.identity);

        }

        public void GetFury(float fury)
        {
            GameManagerSingleton.Instance.bastetFury += fury;
            GameManagerSingleton.Instance.bastetFury = Mathf.Clamp(GameManagerSingleton.Instance.bastetFury, 0f, 100f);
            _mainCInterface.SetFurySlider(GameManagerSingleton.Instance.bastetFury);
        }

        public void ControlUp()
        {
            if (IsChargingAttack && GameManagerSingleton.Instance.bastetEnergy > 0)
            {
                _mainCAttack.SetAttackCount(5);
            }
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

