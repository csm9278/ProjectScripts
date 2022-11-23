using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateMissingTarget: StateMachineBehaviour
{
    public enum Type
    {
        Mutant,
        Mother,
    }

    public Type type = Type.Mutant;

    //Mutant타입용
    EnemyBase_2 enemyBase;
    FindNewRandomTarget findNewRTarget = null;
    EnemyAttack_2 enemyAtk = null;

    //MotherType용
    MotherMutantBase motherBase = null;
    MotherMutantFindTarget motherFindT = null;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(type == Type.Mutant)
        {
            if (enemyBase == null)
                enemyBase = animator.GetComponent<EnemyBase_2>();

            if (findNewRTarget == null)
                findNewRTarget = animator.GetComponent<FindNewRandomTarget>();

            if (enemyAtk == null)
                enemyAtk = animator.GetComponent<EnemyAttack_2>();

            if (enemyBase.m_AttackedFromPlayer)// 적을 놓쳐서 배틀 숫자 감소
            {
                enemyBase.m_AttackedFromPlayer = false;
                RootMain.GameManager.instance.DecreaseBattleNum();
            }
        }
        else if(type == Type.Mother)
        {
            if (motherBase == null)
                animator.TryGetComponent(out motherBase);

            if (motherFindT == null)
                animator.TryGetComponent(out motherFindT);


        }



    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(type == Type.Mutant)
        {
            findNewRTarget.NowPos = animator.transform.position;
            findNewRTarget.CheckNarrow();
            findNewRTarget.MakeRandPos();

            if(enemyAtk != null)
            enemyAtk.findAttack = true;
            findNewRTarget.aiPath.endReachedDistance = 0.2f;
        }
        else if(type == Type.Mother)
        {
            motherFindT.firstPos = animator.transform.position;
            motherFindT.MotherMake();

            motherBase.m_AttackedFromPlayer = false;
            RootMain.GameManager.instance.DecreaseBattleNum();
            SwitchManager.SetSwitch("isFighting", motherBase.m_AttackedFromPlayer);

            motherBase._aipath.endReachedDistance = 0.2f;
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
