using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class DroneMoveToTarget : MonoBehaviour
    {
        Vector3 calcVec = Vector3.zero;
        public DroneBase dronebase = null;
        public DroneFindTarget findT = null;
        public float distanceFromWayPoint = 0.0f;
        public float distanceFromLastPoint = 0.0f;
        public int idx = 0;
        //Animator _animator;

        //--- �缳�� Ÿ�̸�
        public float reTargetTimer = 20.0f;    //�����ð� �� ������������ ���� �缳�� Ÿ�̸�

        //Door�� ������ �� ����� ���� ����
        RaycastHit2D doorhit;
        public LayerMask doorLayer;

        //// Start is called before the first frame update
        void Start()
        {
            findT = GetComponent<DroneFindTarget>();
            dronebase = GetComponent<DroneBase>();
            //_animator = dronebase._animator;
        }

        //// Update is called once per frame
        void Update()
        {
            CheckDistanceWayPoint();
        }

        public void CheckDistanceWayPoint()
        {
            calcVec = (Vector3)findT.goPos - this.transform.position;
            distanceFromWayPoint = calcVec.magnitude;
            dronebase._animator.SetFloat("DistanceFromWayPoint", distanceFromWayPoint);
        }

        public void CheckDistanceLastPoint()
        {
            calcVec = (Vector3)dronebase.lastSeePos - this.transform.position;
            distanceFromLastPoint = calcVec.magnitude;
            dronebase._animator.SetFloat("DistanceFromLastPoint", distanceFromLastPoint);
        }


        /// <summary>
        /// �����ð� �� ���� ���ҽ� ��ġ ������ Ÿ�̸�
        /// </summary>
        public void ReTargetTimerUpdate()
        {
            if (reTargetTimer >= 0.0f)
            {
                reTargetTimer -= Time.deltaTime;
                if (reTargetTimer <= 0.0f)
                {
                    dronebase._animator.SetTrigger("ReTargetTrigger");
                }
            }
        }

        public void CheckDoor()
        {
            float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
            Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
            doorhit = Physics2D.Raycast(this.transform.position, vec, 1.0f, doorLayer);
            if (doorhit)
            {
                dronebase.lastSeePos = this.transform.position;
            }
        }
    }
}
