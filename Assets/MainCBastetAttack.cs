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
    private GameObject _currentEnemy;

    [SerializeField] private Transform bastetPos;

    private bool _isBastetAttacking;
    
    [SerializeField] private int maxShoots = 4;
    public int currentShoots;
    private int _randomPos;
    private int _lastRandomPos;

    public List<Transform> _bastetPositions;

    private void Awake()
    {
        _bastetController = bastetObj.GetComponent<BastetController>();
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.BastetAttack.performed += CheckEnemyFirst;
        _playerInputActions.Player.SecondBastetAttack.performed += CheckEnemySecond;
    }

    private void CheckEnemyFirst(InputAction.CallbackContext context)
    {
        var ray = new Ray(mainCameraObj.transform.position, mainCameraObj.transform.forward);
        if (Physics.Raycast(ray, out var hit,10f, enemyLayer))
        {
            _currentEnemy = hit.collider.gameObject;
            StartAttacking();
        }
    }
    
    private void CheckEnemySecond(InputAction.CallbackContext context)
    {
        var ray = new Ray(mainCameraObj.transform.position, mainCameraObj.transform.forward);
        if (Physics.Raycast(ray, out var hit,10f, enemyLayer))
        {
            _currentEnemy = hit.collider.gameObject;
            SecondStartAttacking();
        }
    }


    private void StartAttacking()
    {
        var parent1 = _currentEnemy.transform.parent;
        var lastChild = parent1.transform.GetChild(parent1.transform.childCount - 1);
        var allChildren = lastChild.GetComponentsInChildren<Transform>();
        foreach (var child in allChildren)
        {
            _bastetPositions.Add(child);
        }
        
        _bastetPositions.RemoveAt(0);

        _randomPos = RandomPos();
        _lastRandomPos = _randomPos;
        bastetObj.SetActive(true);
        _bastetController.HideScanner();
        _bastetController.PosRightHand();
        _bastetController.StartMoveToAttackBastetPos(_bastetPositions[_randomPos].transform, _currentEnemy);

    }

    private void SecondStartAttacking()
    {
        bastetObj.SetActive(true);
        _bastetController.HideScanner();
        _bastetController.PosRightHand();
        _bastetController.StartMoveToBastetPos(_currentEnemy);
    }

    public void NextPos()
    {
        if (currentShoots < maxShoots)
        {
            _randomPos = RandomPos();
            if (_randomPos == _lastRandomPos)
            {
                _randomPos = RandomPos();
            }
            
            if (_randomPos == _lastRandomPos)
            {
                _randomPos = RandomPos();
            }

            _bastetController.StartMoveToAttackBastetPos(_bastetPositions[_randomPos].transform, _currentEnemy);
        }
        else
        {
            bastetObj.SetActive(false);
        }
    }

    public void SetCurrentShoots(int value)
    {
        currentShoots = value;
    }

    private int RandomPos()
    {
        return Random.Range(0, _bastetPositions.Count);
    }
}
