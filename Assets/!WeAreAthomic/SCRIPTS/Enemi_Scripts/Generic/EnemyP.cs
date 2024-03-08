using Player;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using Generics;

public enum TypeOfBehaviour
{
    Patrol,
    Fighter
}

namespace Enemy
{
    public class EnemyP : MonoBehaviour
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
        protected EnemyBrain _enemyBrain;
        protected CharacterController _cc;
        protected SoldierWarnings _soldierWarnings;

        public TypeOfEnemy _typeOfEnemy;
        public TypeOfBehaviour typeOfBehaviour;

        [SerializeField] protected ButtonInteractable _buttonInt;

        [SerializeField] EnemyValues values;

        [SerializeField] private GWaypoints waypoints;

        [NonSerialized] public Action OnPatrol;
        [NonSerialized] public Action OnWarning;
        [NonSerialized] public Action OnCached;

        [SerializeField] protected DecalProjector indicator;

        [SerializeField] private LayerMask targetFovLayer;
        [SerializeField] private LayerMask obstacleFovLayer;

        [SerializeField] private GameObject botonPuerta;

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
        bool m_firstTime = true;

        private int _searchingPlayerTimes;

        private float m_patrolSpeed;
        private float m_chaseSpeed;
        private float m_stoppingDistance;
        [SerializeField] private float waitForNextAttack;
        private float m_knockbackForce;
        private float m_knockbackDisplacement;
        private float m_timeToGetCached;
        private float _totalTimeCached;

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _soldierAnim = GetComponent<SoldierAnimator>();
            _cc = GetComponent<CharacterController>();
            _soldierHurtBox = GetComponentInChildren<SoldierHurtBox>();
            _materialChangeOnDetection = decalDetection.GetComponent<C_MaterialChangeOnDetection>();

            var fovs = GetComponents<FieldOfView>();
            _fov = fovs[0];
            _fovHearNear = fovs[1];
            _fovHearFar = fovs[2];
        }

        private void OnEnable()
        {
            _soldierHurtBox.OnDeath += DisableCC;
            _soldierHurtBox.OnDeath += DisableMovement;
            _soldierHurtBox.OnHurt += Knockback;
            _soldierHurtBox.OnHurtedSmallerTwo += StopAttackDueToHurt;
        }

        private void OnDisable()
        {
            _soldierHurtBox.OnDeath -= DisableCC;
            _soldierHurtBox.OnDeath -= DisableMovement;
            _soldierHurtBox.OnHurt -= Knockback;
            _soldierHurtBox.OnHurtedSmallerTwo -= StopAttackDueToHurt;
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
                if (currentWaypoint != null)
                {
                    StartPatrol();
                }
            }

            if (typeOfBehaviour == TypeOfBehaviour.Fighter)
            {
                decalDetection.SetActive(false);
                StartChasingPlayer();
                botonPuerta.SetActive(false);
            }

            UpdateValues();
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

        private void UpdateValues()
        {
            _agent.speed = values.MoveSpeed;
            m_patrolSpeed = values.PatrolSpeed;
            m_chaseSpeed = values.ChaseSpeed;
            m_stoppingDistance = values.StoppingDistance;
            m_knockbackDisplacement = values.KnockbackDisplacement;
            m_knockbackForce = values.KnockbackForce;
            m_timeToGetCached = values.TimeToGetCached;

            _fov.radius = values.fovRadius;
            _fov.angle = values.fovAngle;
            _fov.targetMask = targetFovLayer;
            _fov.obstructionMask = obstacleFovLayer;
            _fov.radiusColor = values.fovRadiusColor;

            _fovHearNear.radius = values.fovHearFarRadius;
            _fovHearNear.angle = values.fovHearNearAngle;
            _fovHearNear.targetMask = targetFovLayer;
            _fovHearNear.radiusColor = values.fovHearNearRadiusColor;

            _fovHearFar.radius = values.fovHearFarRadius;
            _fovHearFar.angle = values.fovHearFarAngle;
            _fovHearFar.targetMask = targetFovLayer;
            _fovHearFar.radiusColor = values.fovHearFarRadiusColor;
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
            _agent.speed = m_patrolSpeed;
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
                        OnWarning?.Invoke();
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

            if (_totalTimeCached > m_timeToGetCached)
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
                OnCached?.Invoke();
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
            _agent.speed = m_chaseSpeed;
            _agent.stoppingDistance = m_stoppingDistance;
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
        }

        public void DisableMovement()
        {
            _agent.isStopped = true;
        }

        public void DisableCC() => _cc.enabled = false;

        public virtual void Knockback()
        {
            StartCoroutine(PushBack());
        }

        private IEnumerator PushBack()
        {
            var startPos = transform.position;

            while (Mathf.Abs(Vector3.SqrMagnitude(startPos - transform.position)) < m_knockbackDisplacement)
            {
                _cc.Move(m_knockbackForce * Time.deltaTime * _playerObj.transform.forward);

                yield return new WaitForEndOfFrame();
            }
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


