using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
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
