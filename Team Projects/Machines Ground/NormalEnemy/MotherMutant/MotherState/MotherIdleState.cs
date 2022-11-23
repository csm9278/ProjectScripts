using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Mutant;

public class MotherIdleState : StateMachineBehaviour
{
    MotherMutantIdle motherIdle = null;
    MotherMutantBase motherBase = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (motherIdle == null)
            animator.TryGetComponent(out motherIdle);

        if (motherBase == null)
            animator.TryGetComponent(out motherBase);

        motherIdle.enabled = true;

        if(motherBase.mutantList.Length > 0)
        {
            for(int i = 0; i < motherBase.mutantList.Length; i++)
            {
                if(motherBase.mutantList[i].TryGetComponent(out FindNewRandomTarget findRT))
                {
                    findRT.NowPos = motherBase.mutantList[i].transform.position;
                }

                if(motherBase.mutantList[i].TryGetComponent(out AIDestinationSetter aiSetter))
                {
                    aiSetter.enabled = false;
                }
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        motherIdle.enabled = false;
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
