using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class MejoraBastetHurtBox : HurtBox
    {
        [SerializeField] private UnityEvent _unityEvent;
         private Transform _playerTr;
        private CheckIfEnoughMejoras _pp;

        private void Start()
        {
            
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            _pp = _playerTr.parent.GetComponent<CheckIfEnoughMejoras>();
        }
        protected override void GotEnterCollision(Collider col)
        {
            _unityEvent.Invoke();
            _pp.CheckIfYouHaveEnoughToBuy();
            base.GotEnterCollision(col);
           
        }
    }

}

