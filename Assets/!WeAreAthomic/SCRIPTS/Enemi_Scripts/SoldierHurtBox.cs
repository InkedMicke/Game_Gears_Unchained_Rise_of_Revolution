using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{

    public class SoldierHurtBox : MonoBehaviour
    {

        public bool IsDeath;

        public void SetDeath(bool isDeath)
        {
            IsDeath = isDeath;
        }
        public bool CheckIfPlayerIsNear(float radius)
        {
            return Physics.CheckSphere(transform.position, radius);
        }
    }
}
