using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateFindRandTarget : StateMachineBehaviour
{
    FindNewRandomTarget m_FindRT;

    AlienCreatureCharacter alienCtrl = null;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_FindRT == null) m_FindRT = animator.GetComponent<FindNewRandomTarget>();
        //m_FindNT.enabled = true;
        if (alienCtrl == null)
            alienCtrl = animator.GetComponentInChildren<AlienCreatureCharacter>();

        if (alienCtrl != null) 
            alienCtrl.Move(1, 0);

        m_FindRT.SetNextPos();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_FindNT.enabled = false;
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
