using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCFuryAttack : MainCMouseController
    {
        private MainCAttack _mainCAttack;
        private MainCMovement _mainCMovement;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private MainCRailGrindSystem _railGrindSystem;
        private MainCAnimatorController _mainCAnimator;
        private MainCSounds _mainCSounds;
        private BoxCollider _weaponBC;
        private CharacterController _cc;
        private MainCTutorialChecker _mainCTutorial;
        private WrenchHitBox _wrenchHitBox;
        private MainCPistol _mainCPistol;
        private MainCPlayerInterface _mainCInterface;
        private MainCDash _mainCDash;
        private MainCVFX _mainCVfx;
        private MainCRail _mainCRail;
        private G_MeshTrail _mainG;
        private Rigidbody _rb;

        [SerializeField] private Slider furySlider;
        [SerializeField] private float maxFury;
        private float currentFury;
        private float furySpend;
        
        [SerializeField] private PlayerDamageData furyDamageData;
        [SerializeField] private Material lowRangeMaterial;
        [SerializeField] private Material maxRangeMaterial;
        public GameObject weaponObj;
        private GameObject scannerInst;
        [SerializeField] private GameObject scannerPrefab;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private Transform explosionEffectPosition;
        [SerializeField] private Transform groundTr;
        [SerializeField] private CinemachineVirtualCamera cameraBase;

        [SerializeField] private bool IsFullFury;

        [SerializeField] private float scannerSizeSpeed = .1f;
        [SerializeField] private float scannerSize = 15f;

       
        
        [System.NonSerialized] public bool IsFinalAttacking;

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
            _railGrindSystem = GetComponent<MainCRailGrindSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCPistol = GetComponent<MainCPistol>();
            _wrenchHitBox = weaponObj.GetComponent<WrenchHitBox>();
            _mainCInterface = GetComponent<MainCPlayerInterface>();
            _mainCDash = GetComponent<MainCDash>();
            _mainCVfx = GetComponent<MainCVFX>();
            _mainG = GetComponent<G_MeshTrail>();
            _mainCRail = GetComponent<MainCRail>();
            _mainCAttack = GetComponent<MainCAttack>();
            _rb = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            ChargeAttack();
            if (IsChargingAttack && scannerInst != null)
            {
                ChangeMaterialScanner();
            }
        }
        private void ChangeMaterialScanner()
        {
            if (scannerInst.transform.localScale.x >= scannerSize)
            {
                if (scannerInst.GetComponent<MeshRenderer>().material != maxRangeMaterial)
                {
                    scannerInst.GetComponent<MeshRenderer>().material = maxRangeMaterial;
                }
            }
        }
        private void ChargeAttack()
        {
            if (CanChargeAttack() && _hasUnlockedAbilityAttack && GameManagerSingleton.Instance.bastetEnergy > 20f  && !_mainCAttack.IsAttacking && !GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                if (_mainCTutorial.FirstTimeAbility)
                {
                    //tut_ES.transform.GetChild(0).gameObject.SetActive(true);
                }
                _mainCLayers.EnableAbilityAttackLayer();
                _mainCAttack.SetAttackCount(4);
                _mainCMovement.DisableMovement();
                weaponObj.SetActive(true);
                scannerInst = Instantiate(scannerPrefab, groundTr.position, Quaternion.identity);
                IsChargingAttack = true;
                _mainCPistol.StopRecoveringEnergy();
            }

            if (_mouseMagnitude > timeToCharged && _isLeftMousePressed && _hasUnlockedAbilityAttack  && !_mainCAttack.IsAttacking)
            {
                if (GameManagerSingleton.Instance.bastetEnergy <= 0 && !_runOutFury)
                {
                    _mainCAttack.SetAttackCount(5);
                    _runOutFury = true;
                    _canAbilityAttack = false;
                }


                if (scannerInst != null)
                {
                    if (scannerInst.transform.localScale.x >= scannerSize && _mainCTutorial.FirstTimeAbility)
                    {
                        //tut_ES.transform.GetChild(0).gameObject.SetActive(false);
                        //tut_ES.transform.GetChild(1).gameObject.SetActive(true);
                    }

                    if (scannerInst.transform.localScale.x < scannerSize && GameManagerSingleton.Instance.bastetEnergy > 0)
                    {
                        if (!_mainCTutorial.FirstTimeAbility)
                        {
                            GameManagerSingleton.Instance.bastetFury -= furySpend;
                            _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
                        }
                        scannerInst.transform.localScale += Vector3.one * scannerSizeSpeed;
                    }
                }
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

        public void ControlUp()
        {
            if (IsChargingAttack && GameManagerSingleton.Instance.bastetEnergy > 0)
            {
                if (_mainCTutorial.FirstTimeAbility && scannerInst.transform.localScale.x >= scannerSize)
                {
                    _mainCAttack.SetAttackCount(5);
                    //tut_ES.transform.GetChild(1).gameObject.SetActive(false);
                    //tut_ES.transform.GetChild(0).gameObject.SetActive(false);
                    _mainCMovement.EnableMovement();
                    _mainCTutorial.SetFirstAbilityAttack(false);
                    GameManagerSingleton.Instance.TakeEnergy(75);
                    _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
                    _mainCSounds.RemoveAllTutorialSounds();
                    _mainCSounds.PlayExpressionSound();
                }

                if (_mainCTutorial.FirstTimeAbility && scannerInst.transform.localScale.x < scannerSize)
                {
                    EndAttack();
                    //tut_ES.transform.GetChild(1).gameObject.SetActive(false);
                    //tut_ES.transform.GetChild(0).gameObject.SetActive(true);
                    if (scannerInst != null)
                    {
                        Destroy(scannerInst);
                    }
                }
            }
        }
        private void ControlDown()
        {
            _isLeftMousePressed = true;
        }
        public void EndAttack()
        {

            _mainCAnimator.SetRootMotion(false);
            _mainCAttack.IsAttacking = false;
            _mainCAttack.attackCount = 0;
            _mainCAnimator.SetAttackCountAnim(_mainCAttack.attackCount);
            _mainCLayers.DisableAttackLayer();
            _mainCLayers.DisableFinalAttackLayer();
            _mainCLayers.DisableAbilityAttackLayer();
            IsFinalAttacking = false;
            IsChargingAttack = false;
        }
        private bool CanChargeAttack()
        {
            if (IsChargingAttack)
            {
                return false;
            }

            if (!(_mouseMagnitude > timeToCharged))
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

