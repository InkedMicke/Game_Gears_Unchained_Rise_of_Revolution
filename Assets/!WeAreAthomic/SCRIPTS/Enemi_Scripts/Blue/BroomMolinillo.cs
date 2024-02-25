using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomMolinillo : MonoBehaviour
    {
        Broom m_broom;
        [SerializeField] private MolinilloHitBox hitBox;

        private void Awake()
        {
            m_broom = GetComponent<Broom>();
        }

        public void StartAttacking()
        {
            m_broom.SetIsAttacking(true);
            m_broom.broomAnimator.SetRootMotion(true);
            m_broom.broomAnimator.SetMolinilloCount(1);
            StartCoroutine(CheckForStopAttacking());
        }   

        private IEnumerator CheckForStopAttacking()
        {
            while(true)
            {
                yield return new WaitForSeconds(1.5f);
                if(!hitBox.GotCollision)
                {
                    m_broom.SetIsAttacking(false);
                    m_broom.broomAnimator.SetMolinilloCount(2);
                    break;
                }
            }
        }

        public void EndMolinillo()
        {
            m_broom.broomAnimator.SetRootMotion(false);
            m_broom.WaitForDecideWhatToDo();
        }
    }
}