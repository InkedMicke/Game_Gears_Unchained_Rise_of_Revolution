using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public enum TypeOfBehaviour
{
    Patrol,
    Fighter
}

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic
{
    public class Enemy : MonoBehaviour
    {
        protected NavMeshAgent _agent;
        private FieldOfView _fov;
        private FieldOfView _fovHearNear;
        private FieldOfView _fovHearFar;
        protected SoldierAnimator _soldierAnim;
        protected SoldierHurtBox _soldierHurtBox;
        private C_MaterialChangeOnDetection _materialChangeOnDetection;
        private MainCHackingSystem _mainCHack;
        private MainCMovement _mainCMove;
        private Animator _exclamationAnim;

        public GameManagerSingleton.TypeOfEnemy _typeOfEnemy;
        public TypeOfBehaviour typeOfBehaviour;

        [SerializeField] private GWaypoints waypoints;

        [SerializeField] private LayerMask targetFovLayer;
        [SerializeField] private LayerMask obstacleFovLayer;

        [SerializeField] private GameObject botonPuerta;
        [SerializeField] private GameObject exclamacion;
        [SerializeField] private GameObject interrogacion;

        [SerializeField] private GameObject decalDetection;
        private GameObject _playerObj;

        private Transform currentWaypoint;
        protected Transform _playerTr;

        private Vector3 _startPosSearchingPlayer;

        [System.NonSerialized] public bool IsChasingPlayer;
        [System.NonSerialized] public bool IsAttacking;
        protected bool isPatrolling;
        private bool _isWaitingForPatrol;
        private bool _playerHeared;
        private bool _isOnWarning;

        private int _searchingPlayerTimes;

        [SerializeField] private float patrolSpeed;
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float stoppingDistance = 3;
        private float timeToGetCached = .4f;
        private float _totalTimeCached;

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _soldierAnim = GetComponent<SoldierAnimator>();
            _soldierHurtBox = GetComponentInChildren<SoldierHurtBox>();
            _materialChangeOnDetection = decalDetection.GetComponent<C_MaterialChangeOnDetection>();
            _exclamationAnim = exclamacion.GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _playerTr = _playerObj.transform;
            _mainCHack = _playerObj.GetComponent<MainCHackingSystem>();
            _mainCMove = _playerObj.GetComponent<MainCMovement>();

            if (typeOfBehaviour == TypeOfBehaviour.Patrol)
            {
                StartPatrol();
            }       
            
            if (typeOfBehaviour == TypeOfBehaviour.Fighter)
            {
                decalDetection.SetActive(false);
                StartChasingPlayer();
                Destroy(botonPuerta);
            }

            // Configuracion de Fields Of Views

            _fov = transform.AddComponent<FieldOfView>();
            _fov.radius = 6.96f;
            _fov.angle = 90;
            _fov.targetMask = targetFovLayer;
            _fov.obstructionMask = obstacleFovLayer;
            _fov.radiusColor = Color.white;

            _fovHearNear = transform.AddComponent<FieldOfView>();
            _fovHearNear.radius = 6.96f;
            _fovHearNear.angle = 360;
            _fovHearNear.targetMask = targetFovLayer;
            _fovHearNear.radiusColor = Color.white;

            _fovHearFar = transform.AddComponent<FieldOfView>();
            _fovHearFar.radius = 10f;
            _fovHearFar.angle = 360;
            _fovHearFar.targetMask = targetFovLayer;
            _fovHearFar.radiusColor = Color.green;
        }

        protected virtual void Update()
        {
            if (!_soldierHurtBox.IsDeath)
            {
                CheckIfPlayerIsInSight();
                CheckIfPlayerHearedNear();
                CheckIfPlayerHearedFar();
                FollowPath();
                ChasePlayer();
            }
        }

        private void StartPatrol()
        {
            isPatrolling = true;
            _materialChangeOnDetection.PatrolDecal();
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            _agent.SetDestination(currentWaypoint.position);
            _soldierAnim.SetWalking(true);
            _agent.stoppingDistance = 0f;
            _agent.autoBraking = false;
            _agent.speed = patrolSpeed;
        }

        private void FollowPath()
        {
            if (isPatrolling)
            {
                if (Vector3.Distance(transform.position, currentWaypoint.position) < 0.5f && !_isWaitingForPatrol)
                {
                    StartCoroutine(WaitForPatrol());
                }
            }
        }

        private IEnumerator WaitForPatrol()
        {
            _isWaitingForPatrol = true;
            _soldierAnim.SetWalking(false);
            var random = Random.Range(1.5f, 3f);
            yield return new WaitForSeconds(random);
            _isWaitingForPatrol = false;
            StartPatrol();
        }

        public void CheckIfPlayerIsInSight()
        {
            if (!IsChasingPlayer && !IsAttacking)
            {

                if (_fov.canSeePlayer && !IsChasingPlayer)
                {
                    _materialChangeOnDetection.WarningDecal();
                    _totalTimeCached += Time.deltaTime;
                    _isOnWarning = true;
                    if (_totalTimeCached > timeToGetCached)
                    {
                        ChangingValuesToChase();
                        _isOnWarning = false;
                    }
                }
                else
                {
                    if (!_isOnWarning)
                    {
                        _materialChangeOnDetection.PatrolDecal();
                        _totalTimeCached = 0;
                    }
                }
            }
        }

        private void CheckIfPlayerHearedNear()
        {
            if ((_fovHearNear.canSeePlayer && !IsChasingPlayer && !IsAttacking && !_mainCMove.IsCrouch) || (_fovHearNear.canSeePlayer && _mainCMove.IsJumping))
            {
                //_playerHeared = true;
                //_materialChangeOnDetection.WarningDecal();
                //GoToPlayerPosition();


                //ChangingValuesToChase();
            }

        }

        private void CheckIfPlayerHearedFar()
        {
            if ((_fovHearFar.canSeePlayer && !IsChasingPlayer && !IsAttacking && !_mainCMove.IsCrouch) || (_fovHearFar.canSeePlayer && _mainCMove.IsJumping && !IsChasingPlayer && !IsAttacking))
            {
                if(!_isOnWarning)
                {
                    _isOnWarning = true;
                    _materialChangeOnDetection.WarningDecal();
                    GoToPlayerPosition();
                }

                //ChangingValuesToChase();
            }
        }

        private void GoToPlayerPosition()
        {
            _soldierAnim.SetWalking(true);
            _agent.SetDestination(_playerTr.position);
            StartCoroutine(RandomPosition());
        }

        /// <summary>
        /// After going to the player pos, we do a search-player stuff, getting a random pos on a sphere around the last pos
        /// </summary>
        private IEnumerator RandomPosition()
        {
            while(!IsChasingPlayer && _isOnWarning)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending)
                {
                    _soldierAnim.SetWalking(false);
                    _startPosSearchingPlayer = transform.position;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            while (_searchingPlayerTimes < 4)
            {
                _soldierAnim.SetWalking(false);
                var random = Random.Range(1f, 2f);
                yield return new WaitForSeconds(random);
                if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending)
                {
                    _soldierAnim.SetWalking(false);
                    Vector3 offsetAleatorio = Random.insideUnitSphere * 5f;
                    Vector3 posicionAleatoria = _startPosSearchingPlayer + offsetAleatorio;
                    _soldierAnim.SetWalking(true);
                    _agent.SetDestination(posicionAleatoria);

                    _searchingPlayerTimes++;
                }

                yield return new WaitForEndOfFrame();
            }

            _searchingPlayerTimes = 0;
            if (typeOfBehaviour == TypeOfBehaviour.Patrol)
            {
                StartPatrol();
            }

        }

        public void ChasePlayer()
        {
            if (IsChasingPlayer)
            {
                _materialChangeOnDetection.CatchDecal();
                _agent.SetDestination(_playerTr.position);
                _soldierAnim.SetWalking(true);
            }

        }

        public virtual void StartChasingPlayer()
        {
            ChangingValuesToChase();
        }

        private void AgentValuesToChase()
        {
            _agent.speed = chaseSpeed;
            _agent.stoppingDistance = stoppingDistance;
        }

        private void ChangingValuesToChase()
        {
            IsChasingPlayer = true;
            _agent.isStopped = false;
            botonPuerta.SetActive(false);
            AgentValuesToChase();
            isPatrolling = false;
            _mainCHack.StopHack();
            exclamacion.SetActive(true);
            _exclamationAnim.SetTrigger("IsCatch");
        }

        public void DisableMovement()
        {
            _agent.isStopped = true;
        }
    }
}


