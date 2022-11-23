using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDroneIdle : StateMachineBehaviour
{
    Drone.DroneIdle droneIdle = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (droneIdle == null)
            animator.TryGetComponent(out droneIdle);

        droneIdle.SetIdleTimer();

        if(droneIdle._droneBase.droneAnimCtrl != null)
        droneIdle._droneBase.droneAnimCtrl.ChangeAnim(Drone.ChoiDroneAnimCtrl.DroneAnimType.Idle);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneIdle.ScrollDrone();
        droneIdle.CheckScorllDelay();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneIdle.scrollOk = false;
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
