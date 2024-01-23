using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Red
{
    public class RedSoldier : Enemy
    {
        [SerializeField] private ParticleSystem _particlesTornadoDash;
        protected override void Awake()
        {

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}