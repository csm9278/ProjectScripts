using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class StateDroneMoveToTarget : StateMachineBehaviour
{
    DroneMoveToTarget droneMTT = null;
    DroneFindTarget droneFindT = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = true;
        if (droneMTT == null)
            droneMTT = animator.GetComponent<DroneMoveToTarget>();

        if (droneFindT == null)
            droneFindT = animator.GetComponent<DroneFindTarget>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneMTT.ReTargetTimerUpdate();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = false;
        droneMTT.reTargetTimer = 20.0f;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
