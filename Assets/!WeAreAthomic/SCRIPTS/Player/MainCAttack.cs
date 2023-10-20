using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCAttack : MonoBehaviour
    {
        private MainCMovement _mainCMovement;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private Animator _anim;
        private RailGrindSystem _railGrindSystem;
        private MainCAnimatorController _mainCAnimator;
        private BoxCollider _weaponBC;
        private CharacterController _cc;

        [SerializeField] private GameObject weaponObj;

        [SerializeField] private Transform middlePosTr;
        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        [System.NonSerialized] public bool IsAttacking;
        [System.NonSerialized] public bool CanDealDamage;
        [System.NonSerialized] public bool CanMove;
        private bool _clickedOnTime;
        private bool _canNextAttack;
        private bool _isSheathed;

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
            _railGrindSystem = GetComponent<RailGrindSystem>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Attack.performed += Attack;
            _playerInputActions.Player.Attack.performed += NextCombo;

            _weaponBC = weaponObj.GetComponent<BoxCollider>();
        }

        private void Update() => Sheath();

        private void Attack(InputAction.CallbackContext context)
        {
            if (_mainCMovement.IsGrounded() && CanAttack() || _railGrindSystem.IsOnRail() && CanAttack())
            {
                _mainCLayers.EnableAttackLayer();
                if (!_isSheathed)
                {
                    ShowWeapon();
                    _isSheathed = true;
                }

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
        }

        private void Sheath()
        {
            if(!IsAttacking && _isSheathed)
            {
                if(_currentTimeSheath + hideWeaponTimer < Time.time)
                {
                    HideWeapon();
                    _isSheathed = false;
                }
            }
        }

        private void OnDrawGizmos() => Gizmos.DrawWireSphere(middlePosTr.position, nearEnemieToGoFloat);

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack)
            {
                if (attackCount == 1)
                {
                    attackCount++;
                    _mainCAnimator.SetAttackCountAnim(attackCount);
                    _canNextAttack = false;
                }
                else
                {
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
            _currentTimeSheath = Time.time;
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

        public void SetAttackCount(int value)
        {
            attackCount = value;
            _mainCAnimator.SetAttackCountAnim(value);
        }

        private bool CanAttack()
        {
            if (IsAttacking) { return false; }
            if (_mainCMovement.IsCrouch) { return false; }
            if (!(Time.time > timeGraceAttackPeriod)) { return false; }

            return true;
        }
    }
}
