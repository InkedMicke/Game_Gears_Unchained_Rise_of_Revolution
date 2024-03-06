using UnityEngine;
using UnityEngine.Events;
using PPS;
using Generics.Collision;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class BastetGearHurtBox : HurtBox
    {
        [SerializeField] private UnityEvent _unityEvent;
         private Transform _playerTr;
        private CheckIfEnoughMejoras _pp;

        private void Start()
        {
            
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            _pp = _playerTr.parent.GetComponent<CheckIfEnoughMejoras>();
        }
        public override void GotEnterCollision(Collider col)
        {
            _unityEvent.Invoke();
            _pp.CheckIfYouHaveEnoughToBuy();
            base.GotEnterCollision(col);
           
        }
    }

}

