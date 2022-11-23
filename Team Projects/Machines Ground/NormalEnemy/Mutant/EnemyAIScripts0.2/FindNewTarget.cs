using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class FindNewTarget : MonoBehaviour
    {   //적의 순찰포이트를 지정해서 움직이는 방식의 스크립트입니다.

        public EnemyBase_2 m_Base = null;

        public Transform[] m_Poses;
        public int m_PosIdx = 1;
        public AIPath m_Path;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < m_Poses.Length - 1; i++)
            {
                Debug.DrawLine(m_Poses[i].transform.position, m_Poses[i + 1].transform.position);
            }
        }

        private void Awake()
        {

        }

        //// Start is called before the first frame update
        void Start()
        {
            m_Base = GetComponent<EnemyBase_2>();

            m_Poses = m_Base.m_Poses;
            m_Path = m_Base._aipath;
        }

        //// Update is called once per frame
        ////void Update()
        ////{

        ////}

        public void SetNewTarget()
        {
            if (m_Poses != null && m_Path != null)
            {
                m_PosIdx++;
                if (m_PosIdx >= m_Poses.Length)
                    m_PosIdx = 0;

                if (m_Path != null)
                    m_Path.destination = m_Poses[m_PosIdx].position;
            }
            else
            {
                //Debug.Log("Null");
            }
        }

        public void ReturnTarget()
        {
            if (m_Path != null)
                m_Path.destination = m_Poses[m_PosIdx].position;

        }
    }
}