using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class StateDroneChase : StateMachineBehaviour
{
    DroneBase droneBase = null;
    DroneChase droneChase = null;
    DroneAttack droneAttack = null;
    int FindPlayerHash = Animator.StringToHash("FindPlayer");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (droneBase == null)
            droneBase = animator.GetComponent<DroneBase>();
        if (droneChase == null)
            droneChase = animator.GetComponent<DroneChase>();
        if (droneAttack == null)
            animator.TryGetComponent(out droneAttack);

        if(droneBase.droneAnimCtrl != null)
            droneBase.droneAnimCtrl.ChangeAnim(Drone.ChoiDroneAnimCtrl.DroneAnimType.Walk);

        droneBase.FindEnemy(); //적을 찾았소
        droneBase._aipath.endReachedDistance = 0.2f;
        droneBase._aipath.enableRotation = true;


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        droneChase.EnemySetDestination();

        if(animator.GetBool(FindPlayerHash))
        {
            droneAttack.LookPlayerFunc();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

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
