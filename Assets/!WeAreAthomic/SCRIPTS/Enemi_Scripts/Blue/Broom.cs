using System;
using UnityEngine;
using UnityEngine.AI;

namespace Broom
{
    public class Broom : MonoBehaviour
    {
        [NonSerialized] public NavMeshAgent Agent;
        [NonSerialized] public Rigidbody Rb;
        [NonSerialized] public BroomAnimator broomAnimator;
        [NonSerialized] public BroomDefense broomDefense;
        [NonSerialized] public BroomMovement broomMove;
        [NonSerialized] public BroomDash broomDash;
        [NonSerialized] public BroomMolinillo broomMolinillo;
        [NonSerialized] public BroomHurtBox broomHurtBox;
        [NonSerialized] public BroomVFX broomVFX;


        [NonSerialized] public Transform PlayerTr;

        [NonSerialized] public bool IsChasingPlayer;
        [NonSerialized] public bool IsAttacking;

        [SerializeField] private float distanceToDash = 11f;

        private void Awake()
        {
            broomAnimator = GetComponent<BroomAnimator>();
            broomDefense = GetComponent<BroomDefense>();
            Agent = GetComponent<NavMeshAgent>();
            broomMove = GetComponent<BroomMovement>();
            broomDash = GetComponent<BroomDash>();
            broomMolinillo = GetComponent<BroomMolinillo>();
            Rb = GetComponent<Rigidbody>();
            broomHurtBox = GetComponentInChildren<BroomHurtBox>();
            broomVFX = GetComponent<BroomVFX>();

            PlayerTr = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            ChooseAttack();
        }

        protected bool _isAttacking;

        public void ChooseAttack()
        {
            var random = UnityEngine.Random.Range(0, 9);

            if (random == 0)
            {
                broomMove.ChasePlayerAtDistance(14f, () => broomDash.StartDecalToAttack());
            }
            else if (random == 5)
            {
                broomMove.ChasePlayerAtDistance(5f, () => broomMolinillo.StartAttacking());
            }
        }

        public void WaitForDecideWhatToDo()
        {
            Invoke(nameof(ChooseAttack), 3f);
        }

        public void SetIsAttacking(bool isAttacking)
        {
            IsAttacking = isAttacking;
        }    
        
        public void SetIsChasingPlayer(bool isChasingPlayer)
        {
            IsChasingPlayer = isChasingPlayer;
        }

        private bool IsCloseEnough(float distance)
        {
            return Vector3.Distance(transform.position, PlayerTr.position) < distance;
        }
    }
}