using UnityEngine;

public class stateMan : StateMachineBehaviour {

    public enumBotState myState;

    botInterface botInterf;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (botInterf == null) {
            botInterf = animator.transform.GetComponent<botInterface>();
        }

        if (stateInfo.IsName(myState.ToString()) == false) {
            //Debug.LogWarning("State ta errado aqui. " + animator.transform.name + "." + myState);
        }

        botInterf.state = myState;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}
}