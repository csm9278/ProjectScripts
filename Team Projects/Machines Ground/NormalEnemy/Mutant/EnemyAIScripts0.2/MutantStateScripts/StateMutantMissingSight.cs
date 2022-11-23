
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutant;

public class StateMutantMissingSight : StateMachineBehaviour
{
    public enum Type
    {
        Mutant,
        Mother
    }

    public Type type = Type.Mutant;

    private int attackedFromPlayerHash = Animator.StringToHash("AttackedFromPlayer");

    EnemyBase_2 enemyBase = null;
    EnemyMoveToRandTarget enemyMT = null;

    //스텔스 객체
    Stealth stealth = null;
    bool haveStealth = true;

    //마더 뮤턴트 변수
    MotherMutantBase motherBase = null;
    MotherMutantMoveToTarget motherMove = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(type == Type.Mutant)
        {
            if (enemyBase == null)
                animator.TryGetComponent(out enemyBase);

            if (enemyMT == null)
                animator.TryGetComponent(out enemyMT);
            if (stealth == null && haveStealth == true)
                haveStealth = animator.TryGetComponent(out stealth);

            enemyBase.lastSeePos = enemyBase.m_Player.transform.position;

            enemyBase._aipath.destination = enemyBase.lastSeePos;
            enemyBase._aipath.endReachedDistance = 0.2f;

            animator.SetBool(attackedFromPlayerHash, false);
        }
        else if(type == Type.Mother)
        {
            if (motherBase == null)
                animator.TryGetComponent(out motherBase);

            if (motherMove == null)
                animator.TryGetComponent(out motherMove);

            //motherBase.lastSeePos = motherBase.player.transform.position;
            //motherBase._aipath.destination = motherBase.lastSeePos;
            //motherBase._aipath.endReachedDistance = 0.2f;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(type == Type.Mutant)
        {
            enemyMT.CheckDoor();
            enemyMT.CheckDistanceLastPoint();
            enemyBase._aipath.destination = enemyBase.lastSeePos;
        }
        else if(type == Type.Mother)
        {
            
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(type == Type.Mutant)
        {
            if (stealth != null)
            {
                stealth.eStealthState = Stealth.StealthState.ChangeAfter;    //스텔스 상태를 일반상태 변환
            }
        }
        if(type == Type.Mother) // 하위객체들 본 위치로 복귀
        {
            if(motherBase != null)
            {
                motherBase.AiDestSetter(true);
                motherBase._animator.SetBool("AttackedFromPlayer", false);
            }
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
