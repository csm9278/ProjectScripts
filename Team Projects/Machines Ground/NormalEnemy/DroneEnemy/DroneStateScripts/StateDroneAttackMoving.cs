using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class StateDroneAttackMoving : StateMachineBehaviour
{
    DroneAttack droneAttack;
    DroneBase dronebase;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (droneAttack == null)
            droneAttack = animator.GetComponent<DroneAttack>();

        if (dronebase == null)
            dronebase = animator.GetComponent<DroneBase>();

        droneAttack._aipath.endReachedDistance = 0.2f;
        droneAttack.SetAttackMovePos();

        if(dronebase.droneAnimCtrl != null)
            dronebase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Walk);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneAttack.AttackInDelayFunc();
        droneAttack.LookPlayerFunc();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneAttack._aipath.endReachedDistance = 6.0f;
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
