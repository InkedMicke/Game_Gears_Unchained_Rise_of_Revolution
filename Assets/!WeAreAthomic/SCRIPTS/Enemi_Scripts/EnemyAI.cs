using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class EnemyAI : MonoBehaviour
    {
        protected NavMeshAgent _agent;

        protected bool _isShooting;

        protected float distanceToSlow = 6f;
        protected float distanceToAttack = 3f;
        protected float originalSpeed;
        protected float patrolSpeed = 2f;
    }
}

    
