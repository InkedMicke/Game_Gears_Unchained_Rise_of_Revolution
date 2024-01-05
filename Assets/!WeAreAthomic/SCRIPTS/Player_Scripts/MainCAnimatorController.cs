using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCAnimatorController : MonoBehaviour
    {
        private Animator _anim;

        [SerializeField] private string attackCount = "attackCount";
        [SerializeField] private string moveSpeed = "moveSpeed";
        [SerializeField] private string isCrouch = "isCrouch";
        [SerializeField] private string isCrouchWalking = "isCrouchWalking";
        [SerializeField] private string isFalling = "isFalling";
        [SerializeField] private string isJumping = "isJumping";
        [SerializeField] private string isGrounded = "isGrounded";
        [SerializeField] private string isSliding = "isSliding";
        [SerializeField] private string pistolMoveX = "pistolMoveX";
        [SerializeField] private string pistolMoveY = "pistolMoveY";
        [SerializeField] private string hack = "hack";
        [SerializeField] private string aimOnRail = "railAiming";
        [SerializeField] private string dash = "dash";

        protected void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void SetMoveSpeed(float value)
        {
            _anim.SetFloat(moveSpeed, value);
        }

        public float GetMoveSpeed()
        {
            return _anim.GetFloat(moveSpeed);
        }

        public void SetAttackCountAnim(int value)
        {
            _anim.SetInteger(attackCount, value);
        }

        public void SetCrouch(bool condition)
        {
            _anim.SetBool(isCrouch, condition);
        }

        public void SetCrouchWalking(bool condition)
        {
            _anim.SetBool(isCrouchWalking, condition);
        }

        public void SetFalling(bool condition)
        {
            _anim.SetBool(isFalling, condition);
        }

        public void SetJumping(bool condition)
        {
            _anim.SetBool(isJumping, condition);
        }

        public void SetGrounded(bool condition)
        {
            _anim.SetBool(isGrounded, condition);
        }

        public void SetSliding(bool condition)
        {
            _anim.SetBool(isSliding, condition);
        }

        public void SetPistolMoveX(float value)
        {
            _anim.SetFloat(pistolMoveX, value);
        }

        public void SetPistolMoveY(float value)
        {
            _anim.SetFloat(pistolMoveY, value);
        }

        public void SetAimOnRail(bool condition)
        {
            _anim.SetBool(aimOnRail, condition);
        }

        public void TriggerHack()
        {
            _anim.SetTrigger(hack);
        }        
        
        public void TriggerDash()
        {
            _anim.SetTrigger(dash);
        }
    }
}