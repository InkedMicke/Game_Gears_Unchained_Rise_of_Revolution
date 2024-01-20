using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using UnityEngine;
using UnityEngine.UI;

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
