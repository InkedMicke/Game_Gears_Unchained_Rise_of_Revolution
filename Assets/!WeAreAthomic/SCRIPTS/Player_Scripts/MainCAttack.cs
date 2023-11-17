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

        [SerializeField] private GameObject weaponObj;

        [SerializeField] private Transform middlePosTr;
        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        private UnityEngine.SceneManagement.Scene _currentScene;

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
            _currentScene = SceneManager.GetActiveScene();


        }

        private void MouseDown(InputAction.CallbackContext context)
        {
            _currentTimeSheath = Time.time;
        }

        private void MouseUp(InputAction.CallbackContext context)
        {

            Attack();
        }

        private void Attack()
        {
            if (CanAttack() && _isSheathed || _railGrindSystem.IsOnRail() && CanAttack())
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

        /*private IEnumerator Sheath()
        {
            var enable = true;

            while (enable)
            {
                if (_currentTimeSheath + hideWeaponTimer < Time.time)
                {
                    HideWeapon();
                    _isSheathed = false;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }*/

        public void StopSheathCoroutine()
        {

        }

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack && IsAttacking)
            {
                if (attackCount == 2)
                {
                    _mainCLayers.EnableFinalAttackLayer();
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
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
            IsAttacking = false;
            attackCount = 0;
            _mainCAnimator.SetAttackCountAnim(attackCount);
            _mainCLayers.DisableAttackLayer();
            _mainCLayers.DisableFinalAttackLayer();
            timeGraceAttackPeriod = Time.time + timeNextAttack;
            DisableNextAttack();
            IsFinalAttacking = false;
            Debug.Log("hola");
        }

        public void EnableWeaponCollision()
        {
            weaponObj.GetComponent<BoxCollider>().enabled = true;
        }

        public void DisableWeaponCollision()
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

            if (_typeOfAttack != TypeOfAttack.NormalAttack)
            {
                return false;
            }

            return true;
        }
    }
}
