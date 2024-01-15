using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using Unity.VisualScripting;
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
        private FieldOfView _fovHearNear;
        private FieldOfView _fovHearFar;
        protected SoldierAnimator _soldierAnim;
        protected SoldierHurtBox _soldierHurtBox;
        protected SoldierAttack _soldierAttack;
        private C_MaterialChangeOnDetection _materialChangeOnDetection;
        private MainCHackingSystem _mainCHack;
        private MainCMovement _mainCMove;
        private Animator _exclamationAnim;

        public GameManagerSingleton.TypeOfEnemy _typeOfEnemy;
        [SerializeField] private TypeOfBehaviour _typeOfBehaviour;

        [SerializeField] private GWaypoints waypoints;

        [SerializeField] private LayerMask targetFovLayer;
        [SerializeField] private LayerMask obstacleFovLayer;

        [SerializeField] private GameObject botonPuerta;
        [SerializeField] private GameObject exclamacion;
        private GameObject _playerObj;

        private Transform currentWaypoint;
        private Transform _playerTr;

        [System.NonSerialized] public bool IsChasingPlayer;
        [System.NonSerialized] public bool IsAttacking;
        private bool _isPatrolling;
        private bool _isWaitingForPatrol;

        [SerializeField] private float patrolSpeed;
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float stoppingDistance = 3;

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
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

            if (_typeOfBehaviour == TypeOfBehaviour.Patrol)
            {
                StartPatrol();
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
            if ((_fovHearNear.canSeePlayer && !IsChasingPlayer && !IsAttacking && !_mainCMove.IsCrouch) || (_fovHearNear.canSeePlayer && _mainCMove.IsJumping))
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

        public void DisableMovement()
        {
            _agent.isStopped = true;
        }
    }
}

    
