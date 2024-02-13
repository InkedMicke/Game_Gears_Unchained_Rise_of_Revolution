using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCAttack : MainCMouseController
    {
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

        [SerializeField] private PlayerDamageData abilityAttackDmgData;

        [SerializeField] private Material lowRangeMaterial;
        [SerializeField] private Material maxRangeMaterial;

        [SerializeField] private Camera cameraBase;

        public GameObject weaponObj;
        [SerializeField] private GameObject scannerPrefab;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private Transform explosionEffectPosition;
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
        [SerializeField] private float nearEnemieToGoRadius = 2f;
        [SerializeField] private float nearEnemieToGoAngle = 60f;
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
            _rb = GetComponent<Rigidbody>();

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
                    MoveToEnemy();
                    if(_mainCDash.IsDashing)
                    {
                        _mainCDash.StopDash();
                    }
                    
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

                if (CanAttack() && !_isSheathed && !_mainCPistol.IsAiming)
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
            if(CanChargeAttack() && _hasUnlockedAbilityAttack && GameManagerSingleton.Instance.bastetEnergy > 20f && Time.time > _abilityAttackTotalCooldown && !IsAttacking && !GameManagerSingleton.Instance.IsAbilityMenuEnabled)
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

        public void MoveToEnemy()
        {
            var currentEnemy = GetEnemyToMove();
            if (currentEnemy != null)
            {
                if (CheckIfEnemyToMoveIsOnAngleView(currentEnemy))
                {
                    _mainG.EnableTrail();
                    _wrenchHitBox.SetGotHit(true);
                    var direction = currentEnemy.transform.position - transform.position;
                    direction.y = 0f;
                    _cc.Move(direction * .7f);



                }
            }
        }

        public void ApplyAbilityDamage()
        {
            scannerInst.GetComponent<ScannerHitBox>().ApplyDamage(abilityAttackDmgData);
            _mainCVfx.ActivateSlash4();
            InstanciateExplosion();
            _mainCSounds.PlayChargedAttackSound();
            cameraBase.DOShakePosition( .5f, .5f, 20, 40f);
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
                    _cc.Move(20f * Time.deltaTime * direction.normalized);
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
                MoveToEnemy();
                _mainCSounds.PlayAttackSound();
                _mainCSounds.PlayEffordSound();
                if (attackCount == 2)
                {
                    _mainCLayers.DisableAttackLayer();
                    _mainCLayers.EnableFinalAttackLayer();
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    _mainCAnimator.SetRootMotion(true);
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
                DestroyScanner();
            }
            _mainCAnimator.SetRootMotion(false);
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

        private Collider GetEnemyToMove()
        {
            var colliders = Physics.OverlapSphere(transform.position, nearEnemieToGoRadius, enemyHurtBox);
            if(colliders.Length == 0)
            {
                return null;
            }
            Collider enemigoMasCercano = null;
            var distanciaMasCercana = Mathf.Infinity;
            var posicionActual = transform.position;

            foreach(var x in colliders)
            {
                float distanciaEnemigo = Vector3.Distance(posicionActual, x.transform.position);

                if (distanciaEnemigo < distanciaMasCercana)
                {
                    distanciaMasCercana = distanciaEnemigo;
                    enemigoMasCercano = x;
                }
            }

            return enemigoMasCercano;


        }

        private bool CheckIfEnemyToMoveIsOnAngleView(Collider col)
        {
            var directionEnemy = col.transform.position - transform.position;
            var angle = Vector3.Angle(transform.forward, directionEnemy);
            if (angle < nearEnemieToGoAngle)
            {
                return true;
            }

            return false;
        }

        private bool CanAttack()
        {
            if (IsAttacking)
            {
                return false;
            }

            if (Time.time < timeGraceAttackPeriod)
            {
                return false;
            }

            if (!_canAttack)
            {
                return false;
            }

            if(GameManagerSingleton.Instance.IsAbilityMenuEnabled)
            {
                return false;
            }

            if(GameManagerSingleton.Instance.IsStopMenuEnabled)
            {
                return false;
            }

            if(GameManagerSingleton.Instance.IsSettingsMenuEnabled)
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

            return true;
        }
        public void InstanciateExplosion()
        {
            Instantiate(explosionEffect, explosionEffectPosition.position, Quaternion.identity);

        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

            Handles.color = Color.white;
            Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, nearEnemieToGoRadius);

            Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -nearEnemieToGoAngle / 2);
            Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, nearEnemieToGoAngle / 2);

            Handles.color = Color.yellow;
            Handles.DrawLine(transform.position, transform.position + viewAngle01 * nearEnemieToGoRadius);
            Handles.DrawLine(transform.position, transform.position + viewAngle02 * nearEnemieToGoRadius);
        }
#endif

        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
    
}
