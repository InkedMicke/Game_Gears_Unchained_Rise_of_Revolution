using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCAttack : MainCMouseController
    {
        private MainCMovement _mainCMovement;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private Animator _anim;
        private MainCRailGrindSystem _railGrindSystem;
        private MainCAnimatorController _mainCAnimator;
        private MainCSounds _mainCSounds;
        private BoxCollider _weaponBC;
        private CharacterController _cc;
        private MainCTutorialChecker _mainCTutorial;
        private WrenchHitBox _wrenchHitBox;
        private MainCPistol _mainCPistol;
        private MainCPlayerInterface _mainCInterface;

        [SerializeField] private PlayerDamageData abilityAttackDmgData;

        [SerializeField] private Material lowRangeMaterial;
        [SerializeField] private Material maxRangeMaterial;

        public GameObject weaponObj;
        [SerializeField] private GameObject scannerPrefab;
        [SerializeField] private GameObject tut_ES;
        private GameObject scannerInst;

        [SerializeField] private Transform middlePosTr;
        [SerializeField] private Transform groundTr;
        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAttacking;
        [System.NonSerialized] public bool CanMove;
        [System.NonSerialized] public bool IsFinalAttacking;
        public bool IsChargingAttack;
        private bool _clickedOnTime;
        private bool _canNextAttack;
        private bool _isSheathed;
        private bool _canAttack;
        private bool _isLeftMousePressed;
        private bool _attackTutorial;
        private bool _sheathTutorial;
        private bool _hasUnlockedAbilityAttack;
        private bool _runOutEnergy;
        private bool _canAbilityAttack = true;

        public int attackCount;

        [SerializeField] private float timeNextAttack = 0.5f;
        [SerializeField] private float nearEnemieToGoFloat = 2.5f;
        [SerializeField] private float rotationNearEnemie = 8f;
        [SerializeField] private float hideWeaponTimer = 8f;
        [SerializeField] private float scannerSizeSpeed = .1f;
        [SerializeField] private float scannerSize = 15f;
        [SerializeField] private float energySpendSpeed = 4f;
        [SerializeField] private float abilityAttackcooldown = 2f;
        public float timeGraceAttackPeriod;
        private float _currentTimeSheath;
        private float _abilityAttackTotalCooldown;

        private protected override void Awake()
        {
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _anim = GetComponent<Animator>();
            _cc = GetComponent<CharacterController>();
            _railGrindSystem = GetComponent<MainCRailGrindSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCPistol = GetComponent<MainCPistol>();
            _wrenchHitBox = weaponObj.GetComponent<WrenchHitBox>();
            _mainCInterface = GetComponent<MainCPlayerInterface>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Attack.performed += NextCombo;
            _playerInputActions.PlayerPC.Attack.performed += MouseDown;
            _playerInputActions.PlayerPC.Attack.canceled += MouseUp;
            _playerInputActions.PlayerGamepad.Attack.performed += NextCombo;
            _playerInputActions.PlayerGamepad.Attack.performed += GamepadDown;
            _playerInputActions.PlayerGamepad.Attack.canceled += GamepadUp;

            _weaponBC = weaponObj.GetComponent<BoxCollider>();
            _canAttack = false;

            base.Awake();
        }

        private void Update()
        {
            ChargeAttack();
            if(IsChargingAttack && scannerInst != null)
            {
                ChangeMaterialScanner();
            }
        }

        private void GamepadDown(InputAction.CallbackContext context)
        {
            if(GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
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

        private void ControlUp()
        {
            _isLeftMousePressed = false;
            Attack();
            _currentTimeSheath = Time.time;
            _canAbilityAttack = true;
            if (IsChargingAttack && GameManagerSingleton.Instance.bastetEnergy > 0)
            {
                if (_mainCTutorial.FirstTimeAbility && scannerInst.transform.localScale.x >= scannerSize)
                {
                    SetAttackCount(5);
                    tut_ES.transform.GetChild(1).gameObject.SetActive(false);
                    tut_ES.transform.GetChild(0).gameObject.SetActive(false);
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
                    tut_ES.transform.GetChild(1).gameObject.SetActive(false);
                    tut_ES.transform.GetChild(0).gameObject.SetActive(true);
                    if (scannerInst != null)
                    {
                        Destroy(scannerInst);
                    }
                }

                _mainCMovement.EnableMovement();

                _abilityAttackTotalCooldown = Time.time + abilityAttackcooldown;

                var arrows = GameObject.FindGameObjectsWithTag("ArrowDisplayer");

                if (arrows.Length > 0)
                {
                    foreach (var arrow in arrows)
                    {
                        Destroy(arrow);
                    }
                }

                if (!_mainCTutorial.FirstTimeAbility)
                {
                    SetAttackCount(5);
                }
            }
        }

        private void ControlDown()
        {
            _isLeftMousePressed = true;
        }

        private void ChangeMaterialScanner()
        {
            if(scannerInst.transform.localScale.x >= scannerSize)
            {
                if(scannerInst.GetComponent<MeshRenderer>().material != maxRangeMaterial)
                {
                    scannerInst.GetComponent<MeshRenderer>().material = maxRangeMaterial;
                }
            }
        }

        private void Attack()
        {
            if (_typeOfAttack == TypeOfAttack.NormalAttack)
            {
                if (CanAttack() && _isSheathed && !_mainCPistol.IsAiming)
                {
                    _mainCMovement.StopDash();
                    _mainCMovement.EnableMovement();
                    if (_mainCTutorial.IsOnTutorial && !_attackTutorial)
                    {
                        _mainCSounds.RemoveAllSounds();
                        _mainCSounds.PlayExpressionSound();
                        _attackTutorial = true;
                    }
                    _mainCLayers.EnableAttackLayer();
                    _mainCSounds.StopAttackSound();
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    weaponObj.GetComponent<WrenchHitBox>().ClearList();
                    _canNextAttack = false;
                    _mainCSounds.PlayAttackSound();
                    _mainCSounds.PlayEffordSound();

                    IsAttacking = true;

                    _canNextAttack = false;
                }

                if (CanAttack() && !_isSheathed)
                {
                    if (_mainCTutorial.IsOnTutorial && !_sheathTutorial)
                    {
                        _mainCSounds.RemoveAllSounds();
                        _mainCSounds.PlayExpressionSound();
                        var lengthOfClip = _mainCSounds.GetAudioClipLength(_mainCSounds.CurrentExpressionClip.name);
                        Invoke(nameof(PlayTutorialFifth), lengthOfClip);
                        _sheathTutorial = true;
                    }

                    ShowWeapon();
                    _isSheathed = true;
                }
            }
        }

        private void ChargeAttack()
        {
            if(CanChargeAttack() && _hasUnlockedAbilityAttack && GameManagerSingleton.Instance.bastetEnergy > 20f && Time.time > _abilityAttackTotalCooldown && !IsAttacking)
            {
                if(_mainCTutorial.FirstTimeAbility)
                {
                    tut_ES.transform.GetChild(0).gameObject.SetActive(true);
                }
                _mainCLayers.EnableAbilityAttackLayer();
                SetAttackCount(4);
                _mainCMovement.DisableMovement();
                weaponObj.SetActive(true);
                scannerInst = Instantiate(scannerPrefab, groundTr.position, Quaternion.identity);
                IsChargingAttack = true;
                _mainCPistol.StopRecoveringEnergy();
            }

            if(_mouseMagnitude > timeToCharged && _isLeftMousePressed && _hasUnlockedAbilityAttack && Time.time > _abilityAttackTotalCooldown && !IsAttacking)
            {
                if(GameManagerSingleton.Instance.bastetEnergy <= 0 && !_runOutEnergy)
                {
                    SetAttackCount(5);
                    _runOutEnergy = true;
                    _canAbilityAttack = false;
                }


                if (scannerInst != null)
                {
                    if (scannerInst.transform.localScale.x >= scannerSize && _mainCTutorial.FirstTimeAbility)
                    {
                        tut_ES.transform.GetChild(0).gameObject.SetActive(false);
                        tut_ES.transform.GetChild(1).gameObject.SetActive(true);
                    }

                    if (scannerInst.transform.localScale.x < scannerSize && GameManagerSingleton.Instance.bastetEnergy > 0)
                    {
                        if (!_mainCTutorial.FirstTimeAbility)
                        {
                            GameManagerSingleton.Instance.bastetEnergy -= energySpendSpeed;
                            _mainCInterface.SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
                        }
                        scannerInst.transform.localScale += Vector3.one * scannerSizeSpeed;
                    }
                }
            }
        }

        public void MoveToEnemy(Collider other)
        {
            if (!_wrenchHitBox.GotHit && attackCount != 2)
            {
                _wrenchHitBox.SetGotHit(true);
                var enemyPos = other.gameObject.transform.position;
                var direction = enemyPos - transform.position;
                direction.y = 0f;
                _cc.Move(direction * 0.2f);
            }
        }

        public void ApplyAbilityDamage()
        {
            scannerInst.GetComponent<ScannerHitBox>().ApplyDamage(abilityAttackDmgData);
        }

        public void DestroyScanner()
        {
            Destroy(scannerInst);
        }

        private IEnumerator MoveToEnemyCoroutine(Collider other)
        {
            while (true)
            {
                var enemyPos = other.gameObject.transform.position;
                var direction = enemyPos - transform.position;
                direction.y = 0f;
                if (direction.magnitude > 1f)
                {
                    _cc.Move(direction.normalized * 20f * Time.deltaTime);
                }
                else
                {
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack && IsAttacking)
            {
                _mainCSounds.PlayAttackSound();
                _mainCSounds.PlayEffordSound();
                if (attackCount == 2)
                {
                    _mainCLayers.DisableAttackLayer();
                    _mainCLayers.EnableFinalAttackLayer();
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    _anim.applyRootMotion = true;
                    _canNextAttack = false;
                    IsFinalAttacking = true;
                }
                else
                {
                    _mainCSounds.StopAttackSound();
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    _canNextAttack = false;
                }
            }
        }

        public void EnableNextAttack() => _canNextAttack = true;

        public void DisableNextAttack() => _canNextAttack = false;

        public void EndAttack()
        {
            if(IsChargingAttack)
            {
                _mainCPistol.StartRecoveringEnergy(5f);
            }
            _anim.applyRootMotion = false;
            IsAttacking = false;
            attackCount = 0;
            _mainCAnimator.SetAttackCountAnim(attackCount);
            _mainCLayers.DisableAttackLayer();
            _mainCLayers.DisableFinalAttackLayer();
            _mainCLayers.DisableAbilityAttackLayer();
            timeGraceAttackPeriod = Time.time + timeNextAttack;
            DisableNextAttack();
            IsFinalAttacking = false;
            _canNextAttack = false;
            IsChargingAttack = false;
        }

        public void EnableWeaponCollision()
        {
            weaponObj.GetComponent<BoxCollider>().enabled = true;
        }

        public void DisableWeaponCollision()
        {
            weaponObj.GetComponent<BoxCollider>().enabled = false;
            _wrenchHitBox.ClearList();
            _wrenchHitBox.SetGotHit(false);
        }

        public void ShowWeapon() => weaponObj.SetActive(true);

        public void HideWeapon() => weaponObj.SetActive(false);

        public void EnableCanAttack() => _canAttack = true;

        public void DisableCanAttack() => _canAttack = false;

        public void SetAttackCount(int value)
        {
            attackCount = value;
            _mainCAnimator.SetAttackCountAnim(value);
        }

        public void SetIsSheathed(bool condition)
        {
            _isSheathed = condition;
        }

        public void SetHasUnlockedAbilityAttack(bool condition)
        {
            _hasUnlockedAbilityAttack = condition;
        }

        public void SetRunOutEnergy(bool condition)
        {
            _runOutEnergy = condition;
        }

        private void PlayTutorialFifth()
        {
            _mainCSounds.PlayTutorialSound(5, "pc");
        }

        private bool CanAttack()
        {
            if (IsAttacking)
            {
                return false;
            }

            if (!(Time.time > timeGraceAttackPeriod))
            {
                return false;
            }

            if (!_canAttack)
            {
                return false;
            }

            if (!_mainCMovement.IsGrounded())
            {
                return false;
            }

            return true;
        }

        private bool CanChargeAttack()
        {
            if(IsChargingAttack)
            {
                return false;
            }

            if(!(_mouseMagnitude > timeToCharged))
            {
                return false;
            }

            if(!_isSheathed)
            {
                return false;
            }

            if(_mainCPistol.IsAiming)
            {
                return false;
            }

            return true;
        }
    }
}
