using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;
using DG.Tweening;

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
        [SerializeField] HitBoxAngleView hitBoxAngleView;

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
        private bool m_canMoveALittle;
        private bool m_canMoveToEnemy;


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
                    RotateToCameraForward();
                    WhatToDoBasedOnIfGotCol();
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
                    //GCameraShake.Instance.ShakeCamera(1f, .1f);
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

/*        public void MoveToEnemy()
        {
            var currentEnemy = GetEnemyToMove();
            if (currentEnemy != null)
            {
                if (CheckIfEnemyToMoveIsOnAngleView(currentEnemy))
                {
                    var currentEnemyPos = currentEnemy.transform.position;
                    currentEnemyPos.y = transform.position.y;
                    if (Vector3.SqrMagnitude(currentEnemyPos - transform.position) > 1f)
                    {
                        transform.LookAt(currentEnemyPos);
                        Debug.DrawRay(currentEnemyPos, Vector3.up * 20f, Color.green, 20f);
                        StartCoroutine(MoveToEnemyCoroutine(currentEnemyPos));
                        _mainG.EnableTrail();
                    }
                    _wrenchHitBox.SetGotHit(true);
                }
            }
        }*/

        private IEnumerator MoveToEnemyCoroutine(Vector3 enemyPos)
        {
            IsMovingToEnemy = true;
            while (true)
            {
                var currentEnemyPos = enemyPos;
                currentEnemyPos.y = transform.position.y;
                var direction = currentEnemyPos - transform.position;
                direction.y = transform.position.y;
                if (Vector3.SqrMagnitude(currentEnemyPos - transform.position) < 2f)
                {
                    IsMovingToEnemy = false;
                    break;
                }

                _cc.Move(15f * Time.deltaTime * direction.normalized);

                IsMovingToEnemy = false;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator MoveALittleForward(float displacement, float speed)
        {
            var startPos = transform.position;
            while (Mathf.Abs(Vector3.SqrMagnitude(startPos - transform.position)) < displacement)
            {
                _cc.Move(speed * Time.deltaTime * transform.forward.normalized);
                yield return new WaitForEndOfFrame();
            }
        }

        public void MoveALittle(int index)
        {
            EnableWeaponCollision();
            float dis = 0;
            float speed = 0;
            switch (index)
            {
                case 0:
                    dis = 1.5f;
                    speed = 15;
                    break;
                case 1:
                    dis = 1.7f;
                    speed = 3f;
                    break;
                case 2:
                    break;
            }

            if (m_canMoveALittle)
            {
                StartCoroutine(MoveALittleForward(dis, speed));
                m_canMoveALittle = false;
            }
            else if(m_canMoveToEnemy)
            {
                StartCoroutine(MoveToEnemyCoroutine(hitBoxAngleView.colliderList[0].transform.position));
            }
                
        }

        private void NextCombo(InputAction.CallbackContext context)
        {
            if (_canNextAttack && IsAttacking)
            {
                WhatToDoBasedOnIfGotCol();
                RotateToCameraForward();
                //GCameraShake.Instance.ShakeCamera(1f, .1f);
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

        public void FinalAttackShake() { /*GCameraShake.Instance.ShakeCamera(2f, .1f);*/ }

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

        public void ApplyDamageHitBox() => _wrenchHitBox.ApplyDamage();

        public void ShowWeapon() => weaponObj.SetActive(true);

        public void HideWeapon() => weaponObj.SetActive(false);

        public void EnableCanAttack() => _canAttack = true;

        public void DisableCanAttack() => _canAttack = false;

        private void RotateToCameraForward()
        {
            Vector3 cameraForward = new Vector3(cameraBase.transform.forward.x, 0f, cameraBase.transform.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }

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

        private bool CheckIfEnemyToMoveIsOnAngleView(Collider col)
        {
            var directionEnemy = col.transform.position - transform.position;
            var cameraForward = new Vector3(cameraBase.transform.forward.x, transform.position.y, cameraBase.transform.forward.z);
            var angle = Vector3.Angle(cameraForward, directionEnemy);
            if (angle < nearEnemieToGoAngle)
            {
                return true;
            }

            return false;
        }

        private Collider[] GetColsOnRadius(Vector3 position, float radius, LayerMask layer) => Physics.OverlapSphere(position, radius, layer);

        private void WhatToDoBasedOnIfGotCol()
        {
/*
            if (hitBoxAngleView.colliderList.Count == 0)
            {
                m_canMoveALittle = true;
            }
            else
            {
                var enemyPos = hitBoxAngleView.colliderList[0].transform.position;
                enemyPos.y = transform.position.y;
                if (Vector3.SqrMagnitude(enemyPos - transform.position) > 2f)
                {
                    StartCoroutine(MoveToEnemyCoroutine(hitBoxAngleView.colliderList[0].transform.position));
                    LookAtSomething(hitBoxAngleView.colliderList[0].transform.position);
                }
                else
                {
                    m_canMoveToEnemy = true;
                }
            }*/
        }

        private void LookAtSomething(Vector3 pos)
        {
            pos.y = transform.position.y;
            transform.LookAt(pos);
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
            if (cameraBase != null)
            {
                Handles.color = Color.white;
                Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, nearEnemieToGoRadius);

                Vector3 viewAngle01 = DirectionFromAngle(cameraBase.transform.eulerAngles.y, -nearEnemieToGoAngle / 2);
                Vector3 viewAngle02 = DirectionFromAngle(cameraBase.transform.eulerAngles.y, nearEnemieToGoAngle / 2);

                Handles.color = Color.yellow;
                Handles.DrawLine(transform.position + transform.right, transform.position + viewAngle01 * nearEnemieToGoRadius);
                Handles.DrawLine(transform.position - transform.right, transform.position + viewAngle02 * nearEnemieToGoRadius);
            }
        }
#endif

        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }

}
