using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class MutantChase : MonoBehaviour
    {
        public enum DashState
        {
            DashBefore,
            DashCharge,
            Dash,
            DashAfter
        }

        public EnemyBase_2 enemyBase = null;
        public GameObject m_Player = null;
        EnemySight enemysight = null;
        EnemyAttack_2 attack;

        EnemySight sight;

        //대쉬형 돌진 관련 변수
        float chargeTimer = 2.0f;
        float maxChargeTimer = 5.0f;
        float dashCharge = 2.0f;
        float maxDashCharge = 1.0f;
        public Transform dashPos;
        Vector3 dashCurPos = Vector3.zero;

        public DashState dashState = DashState.DashBefore;
        private void Start() => StartFunc();

        private void StartFunc()
        {
            enemyBase = GetComponent<EnemyBase_2>();

            m_Player = GameObject.Find("Player");
            enemysight = GetComponent<EnemySight>();
            attack = GetComponent<EnemyAttack_2>();

            sight = GetComponent<EnemySight>();
            if (enemyBase.type == EnemyBase_2.MutantType.Dash)
            {
                chargeTimer = maxChargeTimer;
            }
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        }

        public void EnemySetDestination()
        {
            if (sight.attackTarget == null)
                return;

            //if (dashState != DashState.DashBefore)
            //    return;

            enemyBase._aipath.destination = sight.attackTarget.transform.position;
        }

        public void DashFunc()
        {
            if (enemyBase.type != EnemyBase_2.MutantType.Dash)
                return;

            if (dashPos == null)
                return;

            switch(dashState)
            {
                case DashState.DashBefore:
                    chargeTimer -= Time.deltaTime;
                    if (chargeTimer <= 0.0f)
                    {
                        dashState = DashState.DashCharge;
                        enemyBase._aipath.canMove = false;
                        enemyBase._aipath.enableRotation = false;
                        enemyBase.isnotKnockBack = true;
                        enemysight.ignoreSight = true;
                        SetDashPoint();
                    }
                    break;

                case DashState.DashCharge:
                    dashCharge -= Time.deltaTime;
                    if (dashCharge <= 0.0f && sight.inSight)
                    {
                        enemyBase._aipath.destination = dashCurPos;
                        enemyBase._aipath.maxSpeed = 20.0f;
                        enemyBase._aipath.canMove = true;
                        dashState = DashState.Dash;
                    }
                    break;

                case DashState.Dash:
                    Vector3 dashVec = dashCurPos - this.transform.position;
                    dashVec.z = 0;
                    if (dashVec.magnitude <= 2.0f)
                    {
                        enemyBase._aipath.maxSpeed = 3.5f;
                        chargeTimer = maxChargeTimer;
                        dashCharge = maxDashCharge;
                        enemyBase._aipath.destination = enemyBase.m_Player.transform.position;
                        dashState = DashState.DashAfter;
                    }
                    break;

                case DashState.DashAfter:
                    chargeTimer = maxChargeTimer;
                    enemyBase._aipath.enableRotation = true;
                    enemysight.ignoreSight = false;
                    enemyBase.isnotKnockBack = false;
                    dashState = DashState.DashBefore;
                    break;
            }
        }

        void SetDashPoint()
        {
            Vector3 dashVec = dashPos.position - this.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dashVec, 8.0f, attack.obstacleMask);
            if(hit)
            {
                dashCurPos = hit.point;
            }
            else
            {
                dashCurPos = dashPos.position;
            }
        }

    }
}