using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateMoveToRandTarget : StateMachineBehaviour
{
    EnemyMoveToRandTarget enemyMoveToRTarget = null;
    FindNewRandomTarget findNewRTarget = null;
    AlienCreatureCharacter alienCtrl = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = true;
        if (enemyMoveToRTarget == null)
            enemyMoveToRTarget = animator.GetComponent<EnemyMoveToRandTarget>();

        if (findNewRTarget == null)
            findNewRTarget = animator.GetComponent<FindNewRandomTarget>();

        if (alienCtrl == null)
            alienCtrl = animator.GetComponentInChildren<AlienCreatureCharacter>();

        enemyMoveToRTarget.enemyBase.m_Idx = findNewRTarget.posIdx;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyMoveToRTarget.ReTargetTimerUpdate();

        if (alienCtrl != null) 
            alienCtrl.Move(1, 0);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = false;
        enemyMoveToRTarget.reTargetTimer = 5.0f;
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
