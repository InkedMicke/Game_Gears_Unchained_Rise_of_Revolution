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
        private BoxCollider _weaponBC;
        private CharacterController _cc;

        [SerializeField] private GameObject weaponObj;

        [SerializeField] private Transform middlePosTr;
        private Transform _closestObject;

        public LayerMask enemyHurtBox;

        [System.NonSerialized] public bool isAttacking;
        [System.NonSerialized] public bool canDealDamage;
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
        private float currentTimeSheath;

        private void Awake()
        {
            _mainCMovement = GetComponent<MainCMovement>();
            _mainCLayers = GetComponent<MainCLayers>();
            _anim = GetComponent<Animator>();
            _cc = GetComponent<CharacterController>();
            _railGrindSystem = GetComponent<RailGrindSystem>();

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
                _anim.SetInteger(string.Format("attackCount"), attackCount);
                weaponObj.GetComponent<WrenchHitBox>().ClearList();
                _canNextAttack = false;
                if (_mainCLayers.isJumpLayerActive)
                {
                    _mainCLayers.DisableJumpLayer();
                }
                isAttacking = true;

                _canNextAttack = false;
            }
        }

        private void Sheath()
        {
            if(!isAttacking && _isSheathed)
            {
                if((currentTimeSheath + hideWeaponTimer) < Time.time)
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
                attackCount++;
                _anim.SetInteger(string.Format("attackCount"), attackCount);
                //CheckNearEnemieToGo();
                _canNextAttack = false;
                CanMove = false;
            }
        }

        public void EnableNextAttack() => _canNextAttack = true;

        public void DisableNextAttack() => _canNextAttack = false;

        public void EndAttack()
        {
            isAttacking = false;
            attackCount = 0;
            _anim.SetInteger(string.Format("attackCount"), attackCount);
            _mainCLayers.DisableAttackLayer();
            timeGraceAttackPeriod = Time.time + timeNextAttack;
            currentTimeSheath = Time.time;
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
            _anim.SetInteger(string.Format("attackCount"), value);
        }

        private bool CanAttack()
        {
            if (isAttacking) { return false; }
            if (_mainCMovement.IsCrouch) { return false; }
            if (!(Time.time > timeGraceAttackPeriod)) { return false; }

            return true;
        }
    }
}
