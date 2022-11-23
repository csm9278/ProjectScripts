using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateChase : StateMachineBehaviour
{
    EnemyBase_2 enemyBase = null;
    MutantChase m_Chase = null;
    EnemyAttack_2 enemyattack = null;

    //스텔스 객체
    Stealth stealth = null;
    bool haveStealth = true;

    int FindPlayerHash = Animator.StringToHash("FindPlayer");

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyBase == null)
            enemyBase = animator.GetComponent<EnemyBase_2>();
        if (m_Chase == null)
            m_Chase = animator.GetComponent<MutantChase>();
        if (enemyattack == null)
            enemyattack = animator.GetComponent<EnemyAttack_2>();
        if (stealth == null && haveStealth == true)
            haveStealth = animator.TryGetComponent(out stealth);

        enemyBase.FindEnemy(); //적을 찾았소
        if(stealth != null)
            stealth.eStealthState = Stealth.StealthState.FindPlayer;    //스텔스 상태를 공격상태로 변환
        enemyBase._aipath.endReachedDistance = 0.2f;
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        m_Chase.DashFunc();
        if(animator.GetBool(FindPlayerHash) && m_Chase.dashState == MutantChase.DashState.DashBefore)
        {
            m_Chase.EnemySetDestination();
            enemyattack.LookPlayerFunc();
        }
        
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

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
