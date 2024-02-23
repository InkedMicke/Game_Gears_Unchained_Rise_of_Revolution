using UnityEngine;

namespace Broom
{
    public class BroomMolinillo : Broom
    {
        public void StartAttacking()
        {
            _broomAnimator.SetMolinilloCount(1);
        }   
    }
}