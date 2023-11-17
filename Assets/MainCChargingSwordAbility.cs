using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCChargingSwordAbility : MainCMouseController
    {
        private void Update()
        {
            ChargeAttack();
        }

        private void ChargeAttack()
        {

        }


        private bool CanChargeAttack()
        {
            if(_typeOfAttack != TypeOfAttack.ChargedAttack)
            {
                return false;
            }

            return true;

        }
    }
}
