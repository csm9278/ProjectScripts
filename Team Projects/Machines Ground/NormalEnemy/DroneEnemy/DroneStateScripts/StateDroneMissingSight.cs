using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class StateDroneMissingSight : StateMachineBehaviour
{

    DroneBase dronebase = null;
    DroneMoveToTarget droneMove = null;
  

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dronebase == null)
            animator.TryGetComponent(out dronebase);

        if (droneMove == null)
            animator.TryGetComponent(out droneMove);

        dronebase.lastSeePos = dronebase.playerRef.transform.position;
        dronebase._aipath.destination = dronebase.lastSeePos;
        dronebase._aipath.enableRotation = true;

        if(dronebase.droneAnimCtrl != null)
            dronebase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Walk);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneMove.CheckDoor();
        droneMove.CheckDistanceLastPoint();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state


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
