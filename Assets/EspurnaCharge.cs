using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.Events;

namespace Player
{
    
    public class EspurnaCharge : MonoBehaviour
    {
        [SerializeField] private HealthManagerSO furyManager;

        [SerializeField] private UnityEvent OnTake;
        private void OnTriggerEnter(Collider other)
        {
            // Recargar Furia
            furyManager.DecreaseHealth(+2);
            //Efecto visual y de sonido
            OnTake?.Invoke();
            
        }

    }

}

