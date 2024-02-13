using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

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
        private MainCJump _mainCJump;
        private MainCCrouch _mainCCrouch;
        private Animator _exclamationAnim;
        protected EnemyBrain _enemyBrain;
        [NonSerialized] public Rigidbody Rb;

        public TypeOfEnemy _typeOfEnemy;
        public TypeOfBehaviour typeOfBehaviour;

        [SerializeField] protected ButtonInteractable _buttonInt;

        [SerializeField] private GWaypoints waypoints;

        [SerializeField] protected DecalProjector indicator;

        [SerializeField] private LayerMask targetFovLayer;
        [SerializeField] private LayerMask obstacleFovLayer;

        [SerializeField] private GameObject botonPuerta;
        [SerializeField] private GameObject exclamacion;
        [SerializeField] private GameObject interrogacion;

        [SerializeField] private GameObject decalDetection;

        private GameObject _playerObj;

        private Transform currentWaypoint;
        [NonSerialized] public Transform PlayerTr;

        private Vector3 _startPosSearchingPlayer;

        [NonSerialized] public bool IsChasingPlayer;
        [NonSerialized] public bool IsAttacking;
        [NonSerialized] public bool IsOnWarning;
        [NonSerialized] public bool CanAttack;
        protected bool isPatrolling;

        private bool _isWaitingForPatrol;
        private bool _playerHeared;



        private int _searchingPlayerTimes;

        [SerializeField] private float patrolSpeed;
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float stoppingDistance = 3;
        private float timeToGetCached = .4f;
        private float _totalTimeCached;
        [SerializeField] private float waitForNextAttack;
        [SerializeField] private float pushForce = 20f;
        public float mass;

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _soldierAnim = GetComponent<SoldierAnimator>();
            _soldierHurtBox = GetComponentInChildren<SoldierHurtBox>();
            Rb = GetComponent<Rigidbody>();
            _materialChangeOnDetection = decalDetection.GetComponent<C_MaterialChangeOnDetection>();
            _exclamationAnim = exclamacion.GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            PlayerTr = _playerObj.transform;
            _mainCHack = _playerObj.GetComponent<MainCHackingSystem>();
            _mainCJump = _playerObj.GetComponent<MainCJump>();
            _mainCCrouch = _playerObj.GetComponent<MainCCrouch>();

            var enemyBrainObject = GameObject.FindGameObjectWithTag("EnemyBrain");
            //_enemyBrain = enemyBrainObject.GetComponent<EnemyBrain>();

            if (typeOfBehaviour == TypeOfBehaviour.Patrol)
            {
                StartPatrol();
            }

            if (typeOfBehaviour == TypeOfBehaviour.Fighter)
            {
                decalDetection.SetActive(false);
                StartChasingPlayer();
                botonPuerta.SetActive(false);
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
                if (typeOfBehaviour == TypeOfBehaviour.Patrol)
                {
                    CheckIfPlayerIsInSight();
                    CheckIfPlayerHearedNear();
                    CheckIfPlayerHeardFar();
                    FollowPath();
                    ChasePlayer();
                }
                else
                {
                    ChasePlayer();
                }
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
            var random = UnityEngine.Random.Range(1.5f, 3f);
            yield return new WaitForSeconds(random);
            _isWaitingForPatrol = false;
            if (!IsOnWarning && !IsAttacking)
            {
                StartPatrol();
            }
        }

        public void CheckIfPlayerIsInSight()
        {
            if (!IsChasingPlayer && !IsAttacking)
            {
                if (_fov.canSeePlayer)
                {
                    if (!IsOnWarning)
                    {
                        _materialChangeOnDetection.WarningDecal();
                        IsOnWarning = true;
                    }
                    CheckWarning();

                }
                else
                {
                    if (!IsOnWarning)
                    {
                        _totalTimeCached = 0;
                        if (!IsAttacking)
                        {
                            _materialChangeOnDetection.PatrolDecal();
                        }
                    }
                }
            }
        }

        private void CheckWarning()
        {
            _totalTimeCached += Time.deltaTime;

            if (_totalTimeCached > timeToGetCached)
            {
                ChangingValuesToChase();
            }
        }

        private void CheckIfPlayerHearedNear()
        {
            if ((_fovHearNear.canSeePlayer && !IsChasingPlayer && !IsAttacking && !_mainCCrouch.IsCrouch) || (_fovHearNear.canSeePlayer && _mainCJump.IsJumping))
            {
                if (!IsChasingPlayer && !IsAttacking)
                {
                    ChangingValuesToChase();
                }
            }

        }

        private void CheckIfPlayerHeardFar()
        {
            if ((_fovHearFar.canSeePlayer && !IsChasingPlayer && !IsAttacking && !_mainCCrouch.IsCrouch) || (_fovHearFar.canSeePlayer && _mainCJump.IsJumping && !IsChasingPlayer && !IsAttacking))
            {
                if (!IsOnWarning && !IsAttacking)
                {
                    IsOnWarning = true;
                    _materialChangeOnDetection.WarningDecal();
                    GoToPlayerPosition();
                }

                //ChangingValuesToChase();
            }
        }

        private void GoToPlayerPosition()
        {
            _buttonInt.DisableCanHack();
            _soldierAnim.SetWalking(true);
            _agent.SetDestination(PlayerTr.position);
            StartCoroutine(RandomPosition());
        }

        /// <summary>
        /// After going to the player pos, we do a search-player stuff, getting a random pos on a sphere around the last pos
        /// </summary>
        private IEnumerator RandomPosition()
        {
            while (!IsChasingPlayer && IsOnWarning)
            {
                if (IsAttacking)
                {
                    break;
                }
                if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending)
                {
                    _startPosSearchingPlayer = transform.position;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            while (_searchingPlayerTimes < 4 && IsOnWarning)
            {
                if (IsAttacking)
                {
                    break;
                }
                _soldierAnim.SetWalking(false);
                var random = UnityEngine.Random.Range(1f, 2f);
                yield return new WaitForSeconds(random);
                _soldierAnim.SetWalking(true);
                Vector3 offsetAleatorio = UnityEngine.Random.insideUnitSphere * 5f;
                Vector3 posicionAleatoria = _startPosSearchingPlayer + offsetAleatorio;
                _agent.SetDestination(posicionAleatoria);

                while (!(_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending))
                {
                    if (IsAttacking)
                    {
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }


                _searchingPlayerTimes++;
                yield return new WaitForEndOfFrame();
            }

            _searchingPlayerTimes = 0;
            if (typeOfBehaviour == TypeOfBehaviour.Patrol && !IsAttacking && IsOnWarning)
            {
                StartPatrol();
                _buttonInt.EnableCanHack();
                IsOnWarning = false;
            }

        }

        private void ChasePlayer()
        {
            if (IsChasingPlayer && !IsAttacking)
            {
                _materialChangeOnDetection.CatchDecal();
                _agent.SetDestination(PlayerTr.position);
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

        public virtual void StopAttackDueToHurt()
        {

        }

        protected void ChangingValuesToChase()
        {
            IsChasingPlayer = true;
            if (typeOfBehaviour == TypeOfBehaviour.Patrol)
            {
                _buttonInt.DisableCanHack();
            }

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

        public virtual void Knockback()
        {
            Rb.AddForce(PlayerTr.transform.forward * pushForce, ForceMode.Impulse);
        }

        protected IEnumerator WaitUntilNextAttack()
        {
            yield return new WaitForSeconds(waitForNextAttack);
            IsAttacking = false;
            StartChasingPlayer();
        }

        public void SetIsAttacking(bool isAttacking)
        {
            IsAttacking = isAttacking;
        }
    }
}


