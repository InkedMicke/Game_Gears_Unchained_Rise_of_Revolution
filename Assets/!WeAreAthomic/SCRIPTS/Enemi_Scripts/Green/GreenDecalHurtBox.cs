using System;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Green
{
    public class GreenDecalHurtBox : MonoBehaviour
    {
        [NonSerialized] public bool IsPlayerInside;
        [NonSerialized] public bool HasPlayerLeft;


        private void OnTriggerEnter(Collider other)
        {
            IsPlayerInside = true;
            HasPlayerLeft = false;
        }

        private void OnTriggerExit(Collider other)
        {
            IsPlayerInside = false;
            HasPlayerLeft = true;
        }
    }
}
