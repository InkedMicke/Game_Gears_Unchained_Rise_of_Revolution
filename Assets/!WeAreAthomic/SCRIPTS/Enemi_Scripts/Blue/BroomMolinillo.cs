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
            m_broom.broomAnimator.SetMolinilloCount(2);
            Invoke(nameof(EndMolinillo), timeChasing);
            m_broom.broomMove.ChasePlayerWithTime(timeChasing);
        }   

        public void EndMolinillo()
        {
            m_broom.broomAnimator.SetMolinilloCount(0);
            m_broom.WaitForDecideWhatToDo();
        }
    }
}