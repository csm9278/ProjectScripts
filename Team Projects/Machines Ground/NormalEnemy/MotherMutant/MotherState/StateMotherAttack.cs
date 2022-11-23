using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateMotherAttack : StateMachineBehaviour
{
    MotherMutantAttack motherAttack;
    MotherMutantBase motherBase;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (motherAttack == null)
            animator.TryGetComponent(out motherAttack);

        if (motherBase == null)
            animator.TryGetComponent(out motherBase);

        motherBase._aipath.endReachedDistance = 20.0f;

        motherBase.AiDestSetter(false);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        motherAttack.LookPlayerFunc();
        motherAttack.AttackOrder();
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        motherBase._aipath.endReachedDistance = 0.2f;
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
