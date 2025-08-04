using UnityEngine;

public class PunishAnim : StateMachineBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Idle") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.gameObject.SetActive(false);
        }
    }
}
