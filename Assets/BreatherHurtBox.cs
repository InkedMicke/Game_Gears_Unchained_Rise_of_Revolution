using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;

public class BreatherHurtBox : HurtBox
{
    private Breather _breather;

    [SerializeField] private ParticleSystem _healRay;

    private void Awake()
    {
        _breather = GetComponentInParent<Breather>();
    }

    protected override void GotEnterCollision(Collider col)
    {
        _breather.StartHeal();
        _healRay.Play();
        base.GotEnterCollision(col);
    }

    protected override void GotExitCollision(Collider col)
    {
        _breather.EndHeal();
        _healRay.Stop();
        base.GotExitCollision(col);
    }
}
