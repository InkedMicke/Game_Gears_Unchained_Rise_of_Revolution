using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

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
        private MainCWrenchHitBox _wrenchHitBox;
        private MainCPistol _mainCPistol;
        private MainCPlayerInterface _mainCInterface;
        private MainCDash _mainCDash;
        private MainCVFX _mainCVfx;
        private MainCRail _mainCRail;
        private G_MeshTrail _mainG;
        private MainCFuryAttack _mainCFuryAttack;

        private Rigidbody _rb;


        [SerializeField] private CinemachineVirtualCamera cameraBase;
        public GameObject weaponObj;
        [SerializeField] private GameObject tut_ES;

        [SerializeField] private Transform middlePosTr;

        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAttacking;
        [System.NonSerialized] public bool IsFinalAttacking;
        [System.NonSerialized] public bool IsMovingToEnemy;
        private bool _canNextAttack;
        public bool _isSheathed;
        private bool _canAttack;
        private bool _isLeftMousePressed;
        private bool _attackTutorial;
        private bool _sheathTutorial;


        public int attackCount;

        [SerializeField] private float timeNextAttack = 0.5f;
        [SerializeField] private float nearEnemieToGoRadius = 2f;
        [SerializeField] private float nearEnemieToGoAngle = 60f;
        [SerializeField] private float rotationNearEnemie = 8f;
        [SerializeField] private float hideWeaponTimer = 8f;


        public float timeGraceAttackPeriod;
        private float _currentTimeSheath;


        protected override void Awake()
        {
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _cc = GetComponent<CharacterController>();
            _railGrindSystem = GetComponent<MainCRailGrindSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();
            _mainCPistol = GetComponent<MainCPistol>();
            _wrenchHitBox = weaponObj.GetComponent<MainCWrenchHitBox>();
            _mainCInterface = GetComponent<MainCPlayerInterface>();
            _mainCDash = GetComponent<MainCDash>();
            _mainCVfx = GetComponent<MainCVFX>();
            _mainG = GetComponent<G_MeshTrail>();
            _mainCRail = GetComponent<MainCRail>();
            _mainCFuryAttack = GetComponent<MainCFuryAttack>();

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

        private void ControlUp()
        {
            _isLeftMousePressed = false;
            Attack();
            _currentTimeSheath = Time.time;

        }

        private void ControlDown()
        {
            _isLeftMousePressed = true;
        }



        private void Attack()
        {
            if (_typeOfAttack == TypeOfAttack.NormalAttack)
            {
                if (CanAttack() && _isSheathed && !_mainCPistol.IsAiming)
                {
                    MoveToEnemy();
                    if (_mainCDash.IsDashing)
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
                    GCameraShake.Instance.ShakeCamera(1f, .1f);
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    weaponObj.GetComponent<MainCWrenchHitBox>().ClearList();
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

        public void MoveToEnemy()
        {
            var currentEnemy = GetEnemyToMove();
            if (currentEnemy != null)
            {
                if (CheckIfEnemyToMoveIsOnAngleView(currentEnemy))
                {
                    if (Vector3.SqrMagnitude(currentEnemy.transform.position - transform.position) > 2)
                    {
                        StartCoroutine(MoveToEnemyCoroutine(currentEnemy));
                        _mainG.EnableTrail();
                    }
                    _wrenchHitBox.SetGotHit(true);
                }
            }
        }

        private IEnumerator MoveToEnemyCoroutine(Collider other)
        {
            IsMovingToEnemy = true;
            while (true)
            {
                var enemyPos = other.transform.position;
                var direction = enemyPos - transform.position;
                direction.y = 0f;
                if (Vector3.SqrMagnitude(enemyPos - transform.position) < 2)
                {
                    IsMovingToEnemy = false;
                    break;
                }
                _cc.Move(10f * Time.deltaTime * direction.normalized);

                IsMovingToEnemy = false;
                yield return new WaitForEndOfFrame();
            }
        }

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack && IsAttacking)
            {
                MoveToEnemy();
                GCameraShake.Instance.ShakeCamera(1f, .1f);
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

        public void FinalAttackShake() { GCameraShake.Instance.ShakeCamera(2f, .1f); }

        public void EnableNextAttack() => _canNextAttack = true;

        public void DisableNextAttack() => _canNextAttack = false;

        public void EndAttack()
        {
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
            _mainCMovement.EnableMovement();
            if (_mainCFuryAttack.scannerInst != null)
            {
                Destroy(_mainCFuryAttack.scannerInst);
            }

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


        private void PlayTutorialFifth()
        {
            _mainCSounds.PlayTutorialSound(5, "pc");
        }

        private Collider GetEnemyToMove()
        {
            var colliders = Physics.OverlapSphere(transform.position, nearEnemieToGoRadius, enemyHurtBox);
            if (colliders.Length == 0)
            {
                return null;
            }
            Collider enemigoMasCercano = null;
            var distanciaMasCercana = Mathf.Infinity;
            var posicionActual = transform.position;

            foreach (var x in colliders)
            {
                float distanciaEnemigo = Vector3.SqrMagnitude(posicionActual - x.transform.position);

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
                Debug.Log("hola2");
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

            return true;
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
