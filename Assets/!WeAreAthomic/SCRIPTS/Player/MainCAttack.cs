using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainCAttack : MonoBehaviour
{
    private MainCMovement _mainCMovement;
    private PlayerInputActions _playerInputActions;
    private MainCLayers _mainCLayers;
    private Animator _anim;
    private BoxCollider _weaponBC;
    private CharacterController _cc;

    [SerializeField] private GameObject weaponObj;

    [SerializeField] private Transform middlePosTr;
    private Transform _closestObject;

    public LayerMask enemyHurtBox;

    [System.NonSerialized] public bool isAttacking;
    [System.NonSerialized] public bool isFinalAttacking;
    [System.NonSerialized] public bool canDealDamage;
    private bool _clickedOnTime;
    private bool _canNextAttack;

    public int attackCount;

    [SerializeField] private float timeNextAttack = 0.5f;
    [SerializeField] private float nearEnemieToGoFloat = 2.5f;
    [SerializeField] private float rotationNearEnemie = 8f;
    private float _timeGraceAttackPeriod;

    private void Awake()
    {
        _mainCMovement = GetComponent<MainCMovement>();
        _mainCLayers = GetComponent<MainCLayers>();
        _anim = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Attack.performed += Attack;
        _playerInputActions.Player.Attack.performed += CanNextCombo;

        _weaponBC = weaponObj.GetComponent<BoxCollider>();
    }

    /*private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Running.performed += Attack;
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
        _playerInputActions.Player.Running.performed -= Attack;
    }*/

    private void Attack(InputAction.CallbackContext context)
    {
        if (CanAttack())
        {
            CheckNearEnemieToGo();
            attackCount++;
            weaponObj.GetComponent<WrenchHitBox>().ClearList();
            _canNextAttack = false;
            if (_mainCLayers.isJumpLayerActive)
            {
                _mainCLayers.DisableJumpLayer();
            }
            isAttacking = true;
            _mainCLayers.EnableAttackLayer();
            _anim.SetInteger(string.Format("attackCount"), attackCount);
            _timeGraceAttackPeriod = Time.time + timeNextAttack;

            _canNextAttack = false;
            isFinalAttacking = true;
            _anim.SetInteger(string.Format("attackCount"), attackCount);
        
        }
    }

    private void CanNextCombo(InputAction.CallbackContext context)
    {
        if (_clickedOnTime)
        {
            _canNextAttack = true;
        }
    }

    private void CheckNearEnemieToGo()
    {
        var colliders = Physics.OverlapSphere(middlePosTr.position, nearEnemieToGoFloat, enemyHurtBox);

        _closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            Transform objectTransform = collider.transform;
            float distance = Vector3.Distance(transform.position, objectTransform.position);

            if (distance < closestDistance)
            {
                _closestObject = objectTransform;
                closestDistance = distance;
            }
        }

        if (_closestObject != null)
        {
            StartCoroutine(nameof(MoveToNearEnemie));

        }
    }

    private IEnumerator MoveToNearEnemie()
    {
        while (Vector3.Distance(transform.position, _closestObject.position) > 1.3f)
        {
            var desiredPos = new Vector3(_closestObject.position.x, transform.position.y, _closestObject.position.z);
            var moveDirection = (desiredPos - transform.position).normalized;
            var moveSpeed = 5f;
            _cc.Move(moveDirection * moveSpeed * Time.deltaTime);

            transform.LookAt(desiredPos);
            //transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotationNearEnemie * Time.deltaTime);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(middlePosTr.position, nearEnemieToGoFloat);
    }

    public void NextCombo()
    {
        if (_canNextAttack)
        {
            attackCount++;
            _anim.SetInteger(string.Format("attackCount"), attackCount);
            _canNextAttack = false;
        }
    }

    public void EnableNextAttack()
    {
        _clickedOnTime = true;
    }

    public void DisableNextAttack()
    {
        if (!_canNextAttack)
        {
            _clickedOnTime = false;
        }
    }

    private void EndAttack()
    {
        isFinalAttacking = false;
        isAttacking = false;
        attackCount = 0;
        _anim.SetInteger(string.Format("attackCount"), attackCount);
        _mainCLayers.DisableAttackLayer();
        _timeGraceAttackPeriod = Time.time + timeNextAttack;
        
    }

    private void EndFinalAttack()
    {
        _mainCLayers.DisableFinalAttackLayer();
        isFinalAttacking = false;
        isAttacking = false;
        attackCount = 0;
        _anim.SetInteger(string.Format("attackCount"), attackCount);
        _mainCLayers.DisableAttackLayer();

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

    private bool CanAttack()
    {
        if (!_mainCMovement.IsGrounded()) { return false; }
        if (isAttacking) { return false; }
        if (_mainCMovement.isCrouch) { return false; }
        if (!(Time.time > _timeGraceAttackPeriod)) { return false; }

        return true;
    }
}
