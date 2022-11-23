using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class EnemyMoveToRandTarget : MonoBehaviour
    {
        Vector3 calcVec = Vector3.zero;
        [HideInInspector] public EnemyBase_2 enemyBase = null;
        public FindNewRandomTarget findRT = null;
        public float DistanceFromWayPoint = 0.0f;
        public float distanceFromLastPoint = 0.0f;
        //public int m_Idx = 0;
        //Animator _animator;

        //--- 재설정 타이머
        public float reTargetTimer = 5.0f;    //지정시간 내 도착하지못할 시의 재설정 타이머

        //Door에 막혔을 시 기능을 위한 변수
        RaycastHit2D doorhit;
        public LayerMask doorLayer;


        //// Start is called before the first frame update
        void Start()
        {
            findRT = GetComponent<FindNewRandomTarget>();
            enemyBase = GetComponent<EnemyBase_2>();
            //_animator = enemyBase._animator;
        }

        //// Update is called once per frame
        void Update()
        {
            CheckDistanceWayPoint();
        }

        public void CheckDistanceWayPoint()
        {
            if (findRT.m_RandPos.Count <= 0 || findRT.m_RandPos.Count <= enemyBase.m_Idx)
                return;

            calcVec = (Vector3)findRT.m_RandPos[enemyBase.m_Idx] - this.transform.position;
            DistanceFromWayPoint = calcVec.magnitude;
            enemyBase._animator.SetFloat("DistanceFromWayPoint", DistanceFromWayPoint);
        }

        public void CheckDistanceLastPoint()
        {
            calcVec = (Vector3)enemyBase.lastSeePos - this.transform.position;
            distanceFromLastPoint = calcVec.magnitude;
            enemyBase._animator.SetFloat("DistanceFromLastPoint", distanceFromLastPoint);
        }

        public void ReTargetTimerUpdate()
        {
            if (reTargetTimer >= 0.0f)
            {
                reTargetTimer -= Time.deltaTime;
                if (reTargetTimer <= 0.0f)
                {
                    enemyBase._animator.SetTrigger("ReTargetTrigger");
                }
            }
        }

        public void CheckDoor()
        {
            float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
            Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
            doorhit = Physics2D.Raycast(this.transform.position, vec, 1.0f, doorLayer);
            if(doorhit)
            {
                enemyBase.lastSeePos = this.transform.position;
            }
        }

    }

}