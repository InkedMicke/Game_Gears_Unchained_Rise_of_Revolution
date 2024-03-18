using System.Collections;
using UnityEngine;

namespace Player
{
    public class MainCAnimatorController : MonoBehaviour
    {
        private Animator m_anim;

        [SerializeField] private string attackCount = "attackCount";
        [SerializeField] private string moveSpeed = "moveSpeed";
        [SerializeField] private string isCrouch = "isCrouch";
        [SerializeField] private string isCrouchWalking = "isCrouchWalking";
        [SerializeField] private string isFalling = "isFalling";
        [SerializeField] private string isJumping = "isJumping";
        [SerializeField] private string isGrounded = "isGrounded";
        [SerializeField] private string isSliding = "isSliding";
        [SerializeField] private string isCrouchSliding = "isCrouchSliding";
        [SerializeField] private string hack = "hack";
        [SerializeField] private string aimOnRail = "railAiming";
        [SerializeField] private string dash = "dash";
        [SerializeField] private string hit = "hit";    
        [SerializeField] private string shield = "shield";    
        [SerializeField] private string heal = "heal";
        string m_currentAnimation;
        public const string CROUCH_IDLE = "CrouchingIdle";
        public const string CROUCH_TO_IDLE = "CrouchedToIdle";
        public const string IDLE_TO_CROUCH = "IdleToCrouched";
        public const string CROUCH_WALKING = "CrouchedWalking";

        protected void Awake()
        {
            m_anim = GetComponent<Animator>();
        }

        public void ChangeAnimation(string animation, float crossfade = 0.2f, float time = 0f)
        {
            if (time > 0) StartCoroutine(Wait());
            else Validate();

            IEnumerator Wait()
            {
                yield return new WaitForSeconds(time);
                Validate();
            }

            void Validate()
            {
                if (m_currentAnimation == animation) return;

                m_currentAnimation = animation;
                m_anim.CrossFade(animation, crossfade);
            }

        }

        public void SetMoveSpeed(float value)
        {
            m_anim.SetFloat(moveSpeed, value);
        }

        public float GetMoveSpeed()
        {
            return m_anim.GetFloat(moveSpeed);
        }

        public void SetAttackCountAnim(int value)
        {
            m_anim.SetInteger(attackCount, value);
        }

        public void SetCrouch(bool condition)
        {
            m_anim.SetBool(isCrouch, condition);
        }

        public void SetCrouchWalking(bool condition)
        {
            m_anim.SetBool(isCrouchWalking, condition);
        }

        public void SetFalling(bool condition)
        {
            m_anim.SetBool(isFalling, condition);
        }

        public void SetJumping(bool condition)
        {
            m_anim.SetBool(isJumping, condition);
        }

        public void SetGrounded(bool condition)
        {
            m_anim.SetBool(isGrounded, condition);
        }

        public void SetSliding(bool condition)
        {
            m_anim.SetBool(isSliding, condition);
        }
        public void SetSlidingCrouch(bool condition)
        {
            m_anim.SetBool(isCrouchSliding, condition);
        }
        public void SetAimOnRail(bool condition)
        {
            m_anim.SetBool(aimOnRail, condition);
        }

        public void TriggerHack()
        {
            m_anim.SetTrigger(hack);
        }        
        
        public void TriggerDash()
        {
            m_anim.SetTrigger(dash);
        }        
        
        public void TriggerHit()
        {
            m_anim.SetTrigger(hit);
        }

        public void TriggerShield() 
        {
            m_anim.SetTrigger(shield);
        }
        public void TriggerHeal()
        {
            m_anim.SetTrigger(heal);
        }

        public void SetRootMotion(bool condition)
        {
            m_anim.applyRootMotion = condition;
        }

        public void AnimEnabled(bool condition)
        {
            m_anim.enabled = condition;
        }
    }
}