using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateMotherMoveToTarget : StateMachineBehaviour
{
    MotherMutantMoveToTarget motherMTT = null;
    MotherMutantFindTarget motherFindT = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = true;
        if (motherMTT == null)
            motherMTT = animator.GetComponent<MotherMutantMoveToTarget>();

        if (motherFindT == null)
            motherFindT = animator.GetComponent<MotherMutantFindTarget>();

        motherFindT.MotherMake();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        motherMTT.ReTargetTimerUpdate();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = false;
        motherMTT.reTargetTimer = 10.0f;
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
