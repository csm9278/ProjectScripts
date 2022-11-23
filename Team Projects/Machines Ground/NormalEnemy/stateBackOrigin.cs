using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;
using Drone;

namespace RootMain
{
    public class stateBackOrigin : StateMachineBehaviour
    {
        public enum MonsterType
        {
            Mutant,
            Drone
        }

        public MonsterType montype = MonsterType.Mutant;
        EnemyMoveToRandTarget _enemyMTRT;
        DroneMoveToTarget _droneMTT;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(montype == MonsterType.Mutant)
            {
                if (_enemyMTRT == null)
                    animator.TryGetComponent(out _enemyMTRT);

                _enemyMTRT.enemyBase.lastSeePos = _enemyMTRT.findRT.firstPos;
                _enemyMTRT.enemyBase._aipath.endReachedDistance = 0.2f;
                _enemyMTRT.enemyBase.backOriginPos = false;
            }
            else if (montype == MonsterType.Drone)
            {
                if (_droneMTT == null)
                    animator.TryGetComponent(out _droneMTT);

                _droneMTT.dronebase.lastSeePos = _droneMTT.findT.firstPos;
                _droneMTT.dronebase._aipath.endReachedDistance = 0.2f;
                _droneMTT.dronebase._aipath.enableRotation = true;
                _droneMTT.dronebase.backOriginPos = false;

            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(montype == MonsterType.Mutant)
            {
                _enemyMTRT.CheckDistanceLastPoint();
                _enemyMTRT.enemyBase._aipath.destination = _enemyMTRT.enemyBase.lastSeePos;
            }
            else if(montype == MonsterType.Drone)
            {
                _droneMTT.CheckDistanceLastPoint();
                _droneMTT.dronebase._aipath.destination = _droneMTT.dronebase.lastSeePos;
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
}
