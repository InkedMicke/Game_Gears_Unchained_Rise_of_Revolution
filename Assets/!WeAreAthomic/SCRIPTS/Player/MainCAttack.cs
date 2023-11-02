using System;
using System.Collections;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCAttack : MonoBehaviour
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

        [SerializeField] private GameObject weaponObj;

        [SerializeField] private Transform middlePosTr;
        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        private Scene _currentScene;

        [System.NonSerialized] public bool IsAttacking;
        [System.NonSerialized] public bool CanDealDamage;
        [System.NonSerialized] public bool CanMove;
        private bool _clickedOnTime;
        private bool _canNextAttack;
        private bool _isSheathed;
        private bool _canAttack;
        private bool _isMousePressed;

        public int attackCount;

        [SerializeField] private float timeNextAttack = 0.5f;
        [SerializeField] private float nearEnemieToGoFloat = 2.5f;
        [SerializeField] private float rotationNearEnemie = 8f;
        [SerializeField] private float hideWeaponTimer = 8f;
        public float timeGraceAttackPeriod;
        private float _currentTimeSheath;

        private void Awake()
        {
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _anim = GetComponent<Animator>();
            _cc = GetComponent<CharacterController>();
            _railGrindSystem = GetComponent<MainCRailGrindSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCTutorial = GetComponent<MainCTutorialChecker>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += Attack;
            _playerInputActions.Player.Attack.performed += NextCombo;
            _playerInputActions.Player.Attack.performed += MouseDown;
            _playerInputActions.Player.Attack.canceled += MouseUp;

            _weaponBC = weaponObj.GetComponent<BoxCollider>();
        }

        private void Start()
        {
            _canAttack = false;
        }

        private void Update()
        {
            _currentScene = SceneManager.GetActiveScene();
        }

        private void MouseDown(InputAction.CallbackContext context)
        {
            StopCoroutine(Sheath());
            _currentTimeSheath = Time.time;
        }

        private void MouseUp(InputAction.CallbackContext context)
        {
            StartCoroutine(Sheath());
        }

        private void Attack(InputAction.CallbackContext context)
        {
            if (_mainCMovement.IsGrounded() && CanAttack() && _canAttack && _isSheathed || _railGrindSystem.IsOnRail() && CanAttack())
            {
                _mainCLayers.EnableAttackLayer();

                _mainCSounds.StopAttackSound();
                _mainCSounds.PlayAttackSound();
                attackCount++;
                _mainCAnimator.SetAttackCountAnim(attackCount);
                weaponObj.GetComponent<WrenchHitBox>().ClearList();
                _canNextAttack = false;
                if (_mainCLayers.isJumpLayerActive)
                {
                    _mainCLayers.DisableJumpLayer();
                }

                IsAttacking = true;

                _canNextAttack = false;
            }

            if (_mainCMovement.IsGrounded() && CanAttack() && _canAttack && !_isSheathed)
            {
                if(_mainCTutorial.isOnTutorial)
                {
                    _mainCSounds.RemoveAllSounds();
                    _mainCSounds.PlayExpressionSound();
                    _mainCSounds.PlayTutorialSound(4, "pc");
                }
                ShowWeapon();
                _isSheathed = true;
            }
        }

        private IEnumerator Sheath()
        {
            var enable = true;

            while(enable)
            {
                if (_currentTimeSheath + hideWeaponTimer < Time.time)
                {
                    HideWeapon();
                    _isSheathed = false;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack)
            {
                if (attackCount == 1)
                {
                    _mainCSounds.StopAttackSound();
                    _mainCSounds.PlayAttackSound();
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    _canNextAttack = false;
                }
                else
                {
                    _mainCSounds.StopAttackSound();
                    _mainCSounds.PlayAttackSound();
                    SetAttackCount(1);
                    _canNextAttack = false;
                }
            }
        }

        public void EnableNextAttack() => _canNextAttack = true;

        public void DisableNextAttack() => _canNextAttack = false;

        public void EndAttack()
        {
            IsAttacking = false;
            attackCount = 0;
            _mainCAnimator.SetAttackCountAnim(attackCount);
            _mainCLayers.DisableAttackLayer();
            timeGraceAttackPeriod = Time.time + timeNextAttack;
        }

        private void EnableWeaponCollision()
        {
            weaponObj.GetComponent<BoxCollider>().enabled = true;
        }

        private void DisableWeaponCollision()
        {
            weaponObj.GetComponent<BoxCollider>().enabled = false;
            weaponObj.GetComponent<WrenchHitBox>().ClearList();
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

        private bool CanAttack()
        {
            if (IsAttacking)
            {
                return false;
            }

            if (_mainCMovement.IsCrouch)
            {
                return false;
            }

            if (!(Time.time > timeGraceAttackPeriod))
            {
                return false;
            }

            return true;
        }
    }
}