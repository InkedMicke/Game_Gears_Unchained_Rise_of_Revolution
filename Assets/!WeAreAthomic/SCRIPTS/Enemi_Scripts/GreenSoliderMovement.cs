using UnityEngine;
using UnityEngine.AI;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class GreenSoliderMovement : EnemyAI
    {
        private GreenSoliderAttack _soldierAttack;
        private SoldierAnimator _soldierAnim;
        private SoldierHealthManager _healthManager;
        private FieldOfView _fov;

        private GameManagerSingleton.TypeOfEnemy _typeOfEnemy;

        [SerializeField] private GWaypoints waypoints;

        private Transform currentWaypoint;
        private Transform _playerTr => GameObject.FindGameObjectWithTag("Player").transform;

        [SerializeField] private bool useWaypoint;
        private bool _isChasingPlayer;
        private bool _isPatrolling;

        private float initalStoppingDistance;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _soldierAttack = GetComponent<GreenSoliderAttack>();
            _soldierAnim = GetComponent<SoldierAnimator>();
            _fov = GetComponent<FieldOfView>();
            _healthManager = GetComponentInChildren<SoldierHealthManager>();

            originalSpeed = _agent.speed;
            initalStoppingDistance = _agent.stoppingDistance;

            _typeOfEnemy = GameManagerSingleton.TypeOfEnemy.GreenSoldier;
        }

        private void Start() {
            if(useWaypoint) 
            {
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                _agent.SetDestination(currentWaypoint.position);
                _soldierAnim.SetWalking(true);
                _isPatrolling = true;
                _agent.stoppingDistance = 0f;
                _agent.autoBraking = false;
            }
        }

        void Update()
        {

            CheckIfPlayerIsInSight();
            FollowPath();
            ChasePlayer();
/*            if (!_soldierAttack.IsAttacking && !_soldierAttack.IsShooting && !_healthManager.IsDeath)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_playerTr.position);
                _soldierAnim.SetWalking(true);
            }

            var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

            if (distanceToPlayer < 5f && !_soldierAttack.IsAttacking && Time.time > _soldierAttack.totalColdown && !_healthManager.IsDeath)
            {
                _agent.isStopped = true;
                _soldierAnim.SetWalking(false);
                _soldierAttack.StartDecal();
            }*/

        }

        public void FollowPath() 
        {
            if(_isPatrolling) 
            {
                if(Vector3.Distance(transform.position, currentWaypoint.position) < 0.5f)
                {
                    currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                    _agent.SetDestination(currentWaypoint.position);
                }
            }
        }

        public void CheckIfPlayerIsInSight() 
        {
            if(_fov.canSeePlayer && !_isChasingPlayer) 
            {
                _isChasingPlayer = true;
                AgentValuesToChase();
                _isPatrolling = false;
            }
        }

        public void AgentValuesToChase()
        {
            _agent.stoppingDistance = initalStoppingDistance;
            _agent.autoBraking = true;
            _agent.speed = originalSpeed;
        }

        public void ChasePlayer()
        {
            if(_isChasingPlayer)
            {
                _agent.SetDestination(_playerTr.position);

                var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

                if (distanceToPlayer < 5f && !_soldierAttack.IsAttacking && !_healthManager.IsDeath)
                {
                    _agent.isStopped = true;
                    _soldierAnim.SetWalking(false);
                    _soldierAttack.StartDecal();
                    _isChasingPlayer = false;
                    _isPatrolling = false;
                }
            }
        }


        public void SetChasePlayer(bool chase)
        {
            _isChasingPlayer = chase;
            _isPatrolling = !chase;
            if(chase)
            {
                AgentValuesToChase();
            }
        }
    }
}
