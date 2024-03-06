using Generics.Collision;
using UnityEngine;

namespace Props
{
    public class BreatherHurtBox : HurtBox
    {
        private Breather _breather;

        [SerializeField] private ParticleSystem _healRay;

        protected override void Awake()
        {
            base.Awake();
            _breather = GetComponentInParent<Breather>();
        }

        public override void GotEnterCollision(Collider col)
        {
            _breather.StartHeal();
            _healRay.Play();
            base.GotEnterCollision(col);
        }

        public override void GotExitCollision(Collider col)
        {
            _breather.EndHeal();
            _healRay.Stop();
            base.GotExitCollision(col);
        }
    }
}