using UnityEngine;
using UnityEngine.AI;

namespace Broom
{
    public class Broom : MonoBehaviour
    {
        protected NavMeshAgent _agent;
        protected BroomAnimator _broomAnimator;
        protected BroomDefense _broomDefense;
        protected BroomMovement _broomMove;
        protected BroomDash _broomDash;
        protected BroomMolinillo _broomMolinillo;

        protected Transform _playerTr;

        protected bool _isChasingPlayer;

        private void Awake()
        {
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            _broomMove.ChasePlayer(() => ChooseAttack(), 9f);
        }

        protected bool _isAttacking;

        private void OnValidate()
        {
            _broomAnimator = GetComponent<BroomAnimator>();
            _broomDefense = GetComponent<BroomDefense>();
            _agent = GetComponent<NavMeshAgent>();
            _broomMove = GetComponent<BroomMovement>();
            _broomDash = GetComponent<BroomDash>();
            _broomMolinillo = GetComponent<BroomMolinillo>();
        }

        public void ChooseAttack()
        {
            var random = Random.Range(0, 1);

            if(random == 0)
            {
                _broomDash.StartDecalToAttack();
            }
            else
            {
                _broomMove.ChasePlayer(() => _broomMolinillo.StartAttacking(), 2f);
            }
        }
    }
}