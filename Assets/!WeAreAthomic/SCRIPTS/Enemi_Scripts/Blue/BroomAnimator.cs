using UnityEngine;

namespace Broom
{
    public class BroomAnimator : MonoBehaviour
    {
        Animator m_anim;

        private void Awake()
        {
            m_anim = GetComponent<Animator>();

            SetLayerWeight(2, 1f);
        }

        public void SetLayerWeight(int layerInt, float layerWeight)
        {
            m_anim.SetLayerWeight(layerInt, layerWeight);
        }

        public void SetShieldCount(int count)
        {
            m_anim.SetInteger("ShieldCount", count);
        }

        public void SetJumpAttackCount(int count)
        {
            m_anim.SetInteger("JumpAttackCount", count);
        }

        public void SetThurstCount(int count)
        {
            m_anim.SetInteger("ThurstCount", count);
        }

        public void SetMolinilloCount(int count)
        {
            m_anim.SetInteger("MolinilloCount", count);
        }

        public void SetCrossAttackCount(int count)
        {
            m_anim.SetInteger("CrossAttackCount", count);
        }

        public void SetRootMotion(bool isEnabled)
        {
            m_anim.applyRootMotion = isEnabled;
        }

        public void SetIsWalking(bool isWalking)
        {
            m_anim.SetBool("isWalking", isWalking);
        }

        public void SetIsStunned(bool isStunned)
        {
            m_anim.SetBool("isStuned", isStunned);
        }

        public void TriggerHurt()
        {
            m_anim.SetTrigger("hurt");
        }
        public void TriggerShoot()
        {
            m_anim.SetTrigger("shoot");
        }

    }
}