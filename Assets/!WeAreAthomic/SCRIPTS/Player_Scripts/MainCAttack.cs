using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

        [SerializeField] private GameObject weaponObj;

        [SerializeField] private Transform middlePosTr;
        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAttacking;
        [System.NonSerialized] public bool CanDealDamage;
        [System.NonSerialized] public bool CanMove;
        [System.NonSerialized] public bool IsFinalAttacking;
        private bool _clickedOnTime;
        private bool _canNextAttack;
        private bool _isSheathed;
        private bool _canAttack;
        private bool _isMousePressed;
        private bool _attackTutorial;
        private bool _sheathTutorial;

        public int attackCount;

        [SerializeField] private float timeNextAttack = 0.5f;
        [SerializeField] private float nearEnemieToGoFloat = 2.5f;
        [SerializeField] private float rotationNearEnemie = 8f;
        [SerializeField] private float hideWeaponTimer = 8f;
        public float timeGraceAttackPeriod;
        private float _currentTimeSheath;

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

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += NextCombo;
            _playerInputActions.Player.Attack.performed += MouseDown;
            _playerInputActions.Player.Attack.canceled += MouseUp;

            _weaponBC = weaponObj.GetComponent<BoxCollider>();
            _canAttack = false;

            base.Awake();
        }

        private protected void Update()
        {


        }

        private void MouseDown(InputAction.CallbackContext context)
        {
            Attack();
            _currentTimeSheath = Time.time;
        }

        private void MouseUp(InputAction.CallbackContext context)
        {
        }

        private void Attack()
        {
            if (CanAttack() && _isSheathed && !_mainCPistol.IsAiming)
            {
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

        private IEnumerator MoveToEnemyCoroutine(Collider other)
        {
            while(true)
            {
                var enemyPos = other.gameObject.transform.position;
                var direction = enemyPos - transform.position;
                direction.y = 0f;
                if(direction.magnitude > 1f)
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Debug.DrawRay(middlePosTr.position, middlePosTr.forward * 1f);
        }

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack && IsAttacking)
            {
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
            _anim.applyRootMotion = false;
            IsAttacking = false;
            attackCount = 0;
            _mainCAnimator.SetAttackCountAnim(attackCount);
            _mainCLayers.DisableAttackLayer();
            _mainCLayers.DisableFinalAttackLayer();
            timeGraceAttackPeriod = Time.time + timeNextAttack;
            DisableNextAttack();
            IsFinalAttacking = false;
            _canNextAttack = false;
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
    }
}
