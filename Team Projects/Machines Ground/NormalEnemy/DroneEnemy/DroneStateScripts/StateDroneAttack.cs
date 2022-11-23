using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class StateDroneAttack : StateMachineBehaviour
{
    DroneAttack droneAttack;
    DroneChase droneChase;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (droneAttack == null)
            droneAttack = animator.GetComponent<DroneAttack>();

        if (droneChase == null)
            animator.TryGetComponent(out droneChase);

        droneChase._aipath.endReachedDistance = 6.0f;
        droneChase._aipath.enableRotation = false;
        if(droneChase.droneBase.droneAnimCtrl != null)
            droneChase.droneBase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Idle);
        droneChase._aipath.destination = animator.transform.position;
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneAttack.LookPlayerFunc();
        droneAttack.EnemyAttackFunc();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneChase._aipath.endReachedDistance = 0.2f;
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
