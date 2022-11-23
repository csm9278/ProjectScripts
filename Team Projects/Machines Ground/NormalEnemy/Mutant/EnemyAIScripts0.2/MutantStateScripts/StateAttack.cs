using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateAttack : StateMachineBehaviour
{
    EnemyBase_2 enemybase;
    EnemyAttack_2 enemyAttack;
    MutantChase enemyChase;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemybase == null)
            animator.TryGetComponent(out enemybase);

        if (enemyAttack == null)
            enemyAttack = animator.GetComponent<EnemyAttack_2>();
        if (enemyChase == null)
            animator.TryGetComponent(out enemyChase);

        enemybase._aipath.endReachedDistance = 2.0f;
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemybase.type == EnemyBase_2.MutantType.Armor)
            return;

        if(enemybase.type == EnemyBase_2.MutantType.Slow)
        {
            enemybase._aipath.destination = animator.transform.position;

            enemyAttack.LookPlayerFunc();
            enemyAttack.ShootSlowWeb();
            return;
        }
        

        enemyAttack.EnemyAttackFunc();
        if(enemyChase.dashState == MutantChase.DashState.DashBefore)
        enemyAttack.LookPlayerFunc();
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemybase._aipath.endReachedDistance = 0.2f;
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
