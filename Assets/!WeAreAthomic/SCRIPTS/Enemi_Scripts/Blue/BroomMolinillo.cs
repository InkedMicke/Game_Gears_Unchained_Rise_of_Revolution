using System.Collections;
using UnityEngine;

namespace Broom
{
    public class BroomMolinillo : Broom
    {
        [SerializeField] private MolinilloHitBox hitBox;

        public void StartAttacking()
        {
            _isAttacking = true;
            _broomAnimator.SetMolinilloCount(1);
            StartCoroutine(CheckForStopAttacking());
        }   

        private IEnumerator CheckForStopAttacking()
        {
            while(true)
            {
                yield return new WaitForSeconds(1.5f);
                if(!hitBox.GotCollision)
                {
                    _isAttacking = false;
                    _broomAnimator.SetMolinilloCount(2);
                    break;
                }
            }

            WaitForDecideWhatToDo();
        }
    }
}