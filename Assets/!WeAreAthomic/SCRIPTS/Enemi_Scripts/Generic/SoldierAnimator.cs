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
        [SerializeField] private string orangeCount = "orangeCount";
        [SerializeField] private string hurtAnimString = "Hurt";

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
        public void HurtTrigger()
        {
            _anim.SetTrigger(hurtAnimString);
        }

        public void SetAnimatorSpeed(float speed)
        {
            _anim.speed = speed;
        }

        public void SetRedCount(int redcount)
        {
            _anim.SetInteger(redCount, redcount);
        }

        public void SetGreenCount(int greenCount)
        {
            _anim.SetInteger(this.greenCount, greenCount);
        }        
        
        public void SetOrangeCount(int orangeCount)
        {
            _anim.SetInteger(this.orangeCount, orangeCount);
        }
    }
}
