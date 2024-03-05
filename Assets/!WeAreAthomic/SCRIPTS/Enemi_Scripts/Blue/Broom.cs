using System;
using UnityEngine;
using UnityEngine.AI;

namespace Broom
{
    public class Broom : MonoBehaviour
    {
        [NonSerialized] public NavMeshAgent Agent;
        [NonSerialized] public CharacterController CC;
        [NonSerialized] public BroomAnimator broomAnimator;
        [NonSerialized] public BroomDefense broomDefense;
        [NonSerialized] public BroomMovement broomMove;
        [NonSerialized] public BroomDash broomDash;
        [NonSerialized] public BroomMolinillo broomMolinillo;
        [NonSerialized] public BroomHurtBox broomHurtBox;
        [NonSerialized] public BroomVFX broomVFX;
        [NonSerialized] public G_MeshTrail MeshTrail;


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
            broomHurtBox = GetComponentInChildren<BroomHurtBox>();
            broomVFX = GetComponent<BroomVFX>();
            CC = GetComponent<CharacterController>();
            MeshTrail = GetComponent<G_MeshTrail>();

            PlayerTr = GameObject.FindGameObjectWithTag("Player").transform;

        }

        private void Start()
        {
            ChooseAttack();
        }

        public void ChooseAttack()
        {
            var random = UnityEngine.Random.Range(0, 8);
            broomMove.EnableMovement();
            broomAnimator.SetLayerWeight(2, 1f);
            if (random != 50)
            {
                broomMove.SetAgentSpeed(broomMove.StartSpeed);
                broomMove.ChasePlayerAtDistance(9f, () => broomDash.StartDecalToAttack());
            }
            else if (random == 50)
            {
                broomMove.SetAgentSpeed(6);
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