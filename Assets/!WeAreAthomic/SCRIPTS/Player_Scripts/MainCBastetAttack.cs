using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MainCBastetAttack : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private BastetController _bastetController;

    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private GameObject mainCameraObj;
    [SerializeField] private GameObject bastetObj;

    private bool _isBastetAttacking;
    
    [SerializeField] private int maxShoots = 4;
    public int currentShoots;
    private int _randomPos;
    private int _lastRandomPos;

    [SerializeField] private float radiusCheck = 10f;
    [SerializeField] private float attackCooldown = 10f;
    private float _totalCooldown = 0f;

    private void Awake()
    {
        _bastetController = bastetObj.GetComponent<BastetController>();
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.BastetAttack.performed += CheckEnemy;
    }

    private void CheckEnemy(InputAction.CallbackContext context)
    {
        if (!_isBastetAttacking && Time.time > _totalCooldown)
        {
            Debug.Log("hola2");
            var collider = Physics.OverlapSphere(transform.position, radiusCheck, enemyLayer);
            if(collider.Length > 0)
            {
                Debug.Log("hola2");
                StartAttacking(collider[0].gameObject);
                _isBastetAttacking = true;
            }
            
            if(collider.Length > 1)
            {
                StartAttacking(collider[0].gameObject);
                _isBastetAttacking = true;
            }
        }
       
    }

    public void DisableBastetAttacking()
    {
        _isBastetAttacking = false;
        _totalCooldown = attackCooldown + Time.time;
    }

    private void StartAttacking(GameObject enemy)
    {
        bastetObj.SetActive(true);
        _bastetController.HideScanner();
        _bastetController.PosRightHand();
        _bastetController.StartMoveToBastetPos(enemy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusCheck);
    }

    public void SetCurrentShoots(int value)
    {
        currentShoots = value;
    }
}
