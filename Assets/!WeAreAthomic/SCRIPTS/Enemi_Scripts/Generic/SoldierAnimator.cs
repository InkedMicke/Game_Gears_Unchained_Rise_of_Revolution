using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic
{
    public class SoldierAnimator : MonoBehaviour
    {
        private Animator _anim;

        [SerializeField] private string walkingAnimString = "isWalking";
        [SerializeField] private string backWalkingAnimString = "isBackWalking";
        [SerializeField] private string shootAnimString = "shoot";

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void SetWalking(bool condition)
        {
            _anim.SetBool(walkingAnimString, condition);
        }

        public void SetBackWalking(bool condition)
        {
            _anim.SetBool(backWalkingAnimString, condition);
        }

        public void ShootTrigger()
        {
            _anim.SetTrigger(shootAnimString);
        }

        public void SetAnimatorSpeed(float speed)
        {
            _anim.speed = speed;
        }
    }
}
