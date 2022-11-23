using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class StateDroneMissingTarget : StateMachineBehaviour
{
    private int attackedFromPlayerHash = Animator.StringToHash("AttackFromPlayer");


    DroneFindTarget findT = null;
    DroneAttack droneAtk = null;
    DroneBase dronebase = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<EnemyMoveToTarget>().enabled = true;
        if (findT == null)
            findT = animator.GetComponent<DroneFindTarget>();

        if (dronebase == null)
            dronebase = animator.GetComponent<DroneBase>();

        if (droneAtk == null)
            droneAtk = animator.GetComponent<DroneAttack>();

        if(dronebase.droneAnimCtrl != null)
            dronebase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Idle);
        dronebase._aipath.destination = animator.transform.position;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        findT.firstPos = animator.transform.position;
        findT.DroneMake();

        droneAtk.findAttack = true;
        findT._aipath.endReachedDistance = 0.2f;
        droneAtk._aipath.enableRotation = true;
        dronebase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Walk);

        if(dronebase.attackFromPlayer)
        {
            dronebase.attackFromPlayer = false;
            animator.SetBool(attackedFromPlayerHash, false);
            RootMain.GameManager.instance.DecreaseBattleNum();
        }

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
