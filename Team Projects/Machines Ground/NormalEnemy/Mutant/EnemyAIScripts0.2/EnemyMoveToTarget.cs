using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class EnemyMoveToTarget : MonoBehaviour
    {

        Vector3 CalcVec = Vector3.zero;
        EnemyBase_2 m_base = null;
        FindNewTarget m_FindT = null;
        public float DistanceFromWayPoint = 0.0f;
        public Transform[] m_Poses;
        public int m_Idx = 0;
        //Animator m_Ani;

        //-- 대비용 변수
        float m_ReDestTime = 2.0f;
        AIPath m_Path = null;


        //// Start is called before the first frame update
        void Start()
        {
            m_FindT = GetComponent<FindNewTarget>();
            m_base = GetComponent<EnemyBase_2>();
            m_Poses = m_base.m_Poses;
            //m_Ani = m_base._animator;
            m_Path = m_base._aipath;
        }

        //// Update is called once per frame
        void Update()
        {
            CalcVec = m_Poses[m_Idx].position - this.transform.position;
            DistanceFromWayPoint = CalcVec.magnitude;
            //m_Ani.SetFloat("DistanceFromWayPoint", DistanceFromWayPoint);
        }

    }

}