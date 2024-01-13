using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class Enemy : MonoBehaviour
    {
        private enum TypeOfBehaviour
        {
            Patrol,
            Fighter
        }

        protected NavMeshAgent _agent;
        private FieldOfView _fov;
        private FieldOfViewHear _fovHear;
        protected SoldierAnimator _soldierAnim;
        protected SoldierHurtBox _soldierHurtBox;
        protected SoldierAttack _soldierAttack;
        private C_MaterialChangeOnDetection _materialChangeOnDetection;
        private MainCHackingSystem _mainCHack;
        private MainCMovement _mainCMove;
        private Animator _exclamationAnim;

        public GameManagerSingleton.TypeOfEnemy _typeOfEnemy;

        [SerializeField] private GWaypoints waypoints;

        [SerializeField] private GameObject botonPuerta;
        [SerializeField] private GameObject exclamacion;
        private GameObject _playerObj;

        private Transform currentWaypoint;
        private Transform _playerTr;

        [System.NonSerialized] public bool IsChasingPlayer;
        [System.NonSerialized] public bool IsAttacking;
        [SerializeField] private bool useWaypoint;
        private bool _isPatrolling;
        private bool _isWaitingForPatrol;

        [SerializeField] private float patrolSpeed;
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float stoppingDistance = 3;

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _fov = GetComponent<FieldOfView>();
            _fovHear = GetComponent<FieldOfViewHear>();
            _soldierAnim = GetComponent<SoldierAnimator>();
            _soldierAttack = GetComponent<SoldierAttack>();
            _soldierHurtBox = GetComponentInChildren<SoldierHurtBox>();
            _materialChangeOnDetection = GetComponentInChildren<C_MaterialChangeOnDetection>();
            _exclamationAnim = exclamacion.GetComponent<Animator>();
        }

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _playerTr = _playerObj.transform;
            _mainCHack = _playerObj.GetComponent<MainCHackingSystem>();
            _mainCMove = _playerObj.GetComponent<MainCMovement>();

            if (useWaypoint)
            {
                StartPatrol();
            }
        }

        protected virtual void Update()
        {
            if (!_soldierHurtBox.IsDeath)
            {
                CheckIfPlayerIsInSight();
                CheckIfPlayerHeared();
                FollowPath();
                ChasePlayer();
            }
        }

        private void StartPatrol()
        {
            _isPatrolling = true;
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            _agent.SetDestination(currentWaypoint.position);
            _soldierAnim.SetWalking(true);
            _agent.stoppingDistance = 0f;
            _agent.autoBraking = false;
            _agent.speed = patrolSpeed;
        }

        private void FollowPath()
        {
            if (_isPatrolling)
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
            if (_fov.canSeePlayer && !IsChasingPlayer && !IsAttacking)
            {
                ChangingValuesToChase();
            }
        }

        private void CheckIfPlayerHeared()
        {
            if ((_fovHear.canSeePlayer && !IsChasingPlayer && !IsAttacking && !_mainCMove.IsCrouch) || (_fovHear.canSeePlayer && _mainCMove.IsJumping))
            {
                ChangingValuesToChase();
            }
        }

        public void ChasePlayer()
        {
            if (IsChasingPlayer)
            {
                _materialChangeOnDetection.CatchDecal();
                _agent.SetDestination(_playerTr.position);
                _soldierAnim.SetWalking(true);
                var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

                if (distanceToPlayer < 5f && !IsAttacking && !_soldierHurtBox.IsDeath)
                {
                    _agent.isStopped = true;
                    _soldierAnim.SetWalking(false);
                    StartDecalToAttack();
                    IsChasingPlayer = false;
                    _isPatrolling = false;
                }
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
            _isPatrolling = false;
            _mainCHack.StopHack();
            exclamacion.SetActive(true);
            _exclamationAnim.SetTrigger("IsCatch");
        }

        protected virtual void StartDecalToAttack()
        {

        }
    }
}

    
