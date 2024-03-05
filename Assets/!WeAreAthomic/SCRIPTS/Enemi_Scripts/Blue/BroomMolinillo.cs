using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomMolinillo : MonoBehaviour
    {
        Broom m_broom;
        [SerializeField] private MolinilloHitBox hitBox;

        [SerializeField] float timeChasing = 10;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();
        }

        public void StartAttacking()
        {
            m_broom.SetIsAttacking(true);
            m_broom.broomAnimator.SetIsWalking(false);
            m_broom.broomAnimator.SetLayerWeight(1, 1f);
            m_broom.broomAnimator.SetLayerWeight(2, 0f);
            m_broom.broomAnimator.SetLayerWeight(3, 1f);
            m_broom.broomAnimator.SetMolinilloCount(2);
            m_broom.broomMove.ChasePlayerWithTime(timeChasing, () => EndMolinillo());
            StartCoroutine(RotateSpine());
        }   

        IEnumerator RotateSpine()
        {
            while(m_broom.IsAttacking)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public void EndMolinillo()
        {
            m_broom.broomAnimator.SetMolinilloCount(0);
            m_broom.SetIsAttacking(false);
            m_broom.broomAnimator.SetLayerWeight(1, 0f);
            m_broom.broomAnimator.SetLayerWeight(3, 0f);
            m_broom.broomMove.SetAgentSpeed(0);
            m_broom.WaitForDecideWhatToDo();
        }
    }
}