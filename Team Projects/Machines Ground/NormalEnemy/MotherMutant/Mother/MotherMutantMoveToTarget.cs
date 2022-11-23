using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mutant
{
    public class MotherMutantMoveToTarget : MonoBehaviour
    {
        Vector3 calcVec = Vector3.zero;
        MotherMutantBase motherBase = null;
        MotherMutantFindTarget findRT = null;
        public float DistanceFromWayPoint = 0.0f;
        public float distanceFromLastPoint = 0.0f;

        //--- 재설정 타이머
        public float reTargetTimer = 5.0f;    //지정시간 내 도착하지못할 시의 재설정 타이머


        //// Start is called before the first frame update
        void Start()
        {
            findRT = GetComponent<MotherMutantFindTarget>();
            motherBase = GetComponent<MotherMutantBase>();
            //motherBase._animator = motherBase._animator;
        }

        //// Update is called once per frame
        void Update()
        {
            CheckDistanceWayPoint();
        }

        public void CheckDistanceWayPoint()
        {
            calcVec = (Vector3)findRT.goPos - this.transform.position;
            DistanceFromWayPoint = calcVec.magnitude;
            motherBase._animator.SetFloat("DistanceFromWayPoint", DistanceFromWayPoint);
        }

        public void CheckDistanceLastPoint()
        {
            calcVec = (Vector3)motherBase.lastSeePos - this.transform.position;
            distanceFromLastPoint = calcVec.magnitude;
            motherBase._animator.SetFloat("DistanceFromLastPoint", distanceFromLastPoint);
        }

        public void ReTargetTimerUpdate()
        {
            if (reTargetTimer >= 0.0f)
            {
                reTargetTimer -= Time.deltaTime;
                if (reTargetTimer <= 0.0f)
                {
                    motherBase._animator.SetTrigger("ReTargetTrigger");
                }
            }
        }
    }
}
