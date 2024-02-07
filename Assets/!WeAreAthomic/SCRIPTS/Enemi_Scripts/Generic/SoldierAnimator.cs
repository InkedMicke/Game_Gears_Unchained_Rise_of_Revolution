using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic
{
    public class SoldierAnimator : MonoBehaviour
    {
        private Animator _anim;

        [SerializeField] private string walkingAnimString = "isWalking";
        [SerializeField] private string backWalkingAnimString = "isBackWalking";
        [SerializeField] private string shootAnimString = "shoot";
        [SerializeField] private string redCount = "redCount";
        [SerializeField] private string greenCount = "ShootCount";

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

        public void SetAnimRootMotion(bool condition)
        {
            _anim.applyRootMotion = condition;
        }

        public void ShootTrigger()
        {
            _anim.SetTrigger(shootAnimString);
        }

        public void SetAnimatorSpeed(float speed)
        {
            _anim.speed = speed;
        }

        public void SetRedCount(int redcount)
        {
            _anim.SetInteger(redCount, redcount);
        }

        public void SetGreenCount(int greenCounts)
        {
            _anim.SetInteger(greenCount, greenCounts);
        }
    }
}
