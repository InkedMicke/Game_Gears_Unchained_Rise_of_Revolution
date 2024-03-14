using Player;
using UnityEngine;

public class OnFinishCrouch : StateMachineBehaviour
{
    [SerializeField] string animation;
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<MainCAnimatorController>().ChangeAnimation(animation, 0.2f, stateInfo.length);
        Debug.Log("hola1");
    }
}
