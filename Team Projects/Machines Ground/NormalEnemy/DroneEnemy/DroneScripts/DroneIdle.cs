using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class DroneIdle : MonoBehaviour
    {
        float idleDelayTime = 0.0f;
        Animator _animator;
        public DroneBase _droneBase;


        [HideInInspector] public int idleDelayTimeHash = Animator.StringToHash("IdleDelayTime");

        //드론 스크롤 정찰 변수
        float scrollTime = 2.0f;
        float scrollIdleTime = 3.0f;
        float scrolllSpeed = 40.0f;
        public bool scrollOk = false;
        bool scrollright = true;
        int scrollNum = 0;



        // Start is called before the first frame update
        void Start()
        {
            _droneBase = GetComponent<DroneBase>();
            idleDelayTime = Random.Range(2.0f, 3.0f);
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_droneBase.isNotPatrol)
                return;

            if (idleDelayTime >= 0.0f && scrollOk )
            {
                idleDelayTime -= Time.deltaTime;
                _animator.SetFloat(idleDelayTimeHash, idleDelayTime);
            }
        }

        /// <summary>
        /// Idle 대기시간 랜덤 설정 함수
        /// </summary>
        public void SetIdleTimer()
        {
            idleDelayTime = Random.Range(2.0f, 3.0f);
            _animator.SetFloat(idleDelayTimeHash, idleDelayTime);
        }


        public void ScrollDrone()
        {
            if (_droneBase.isNotPatrol)
                return;

            if (scrollIdleTime >= 0.0f)
                return;

            if (scrollright)
            {
                if (scrollTime >= 0.0f)
                {
                    this.transform.Rotate(Vector3.forward * scrolllSpeed * Time.deltaTime);

                    scrollTime -= Time.deltaTime;
                    if (scrollTime <= 0.0f)
                    {
                        if(_droneBase.droneAnimCtrl != null)
                            _droneBase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Idle);
                        scrollTime = Random.Range(2.0f, 3.0f);
                        scrollIdleTime = Random.Range(4.0f, 5.0f);
                        scrollright = false;
                    }
                }
            }
            else
            {
                if (scrollTime >= 0.0f)
                {
                    this.transform.Rotate(-Vector3.forward * scrolllSpeed * Time.deltaTime);

                    scrollTime -= Time.deltaTime;
                    if (scrollTime <= 0.0f)
                    {
                        if(_droneBase.droneAnimCtrl != null)
                        _droneBase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Idle);
                        scrollTime = Random.Range(2.0f, 3.0f);
                        scrollright = true;
                        scrollIdleTime = Random.Range(4.0f, 5.0f);
                    }
                }
            }
        }

        public void CheckScorllDelay()
        {
            if (_droneBase.isNotPatrol)
                return;

            if (scrollIdleTime >= 0.0f)
            {
                scrollIdleTime -= Time.deltaTime;
                if(scrollIdleTime <= 0.0f)
                {
                    if (_droneBase.droneAnimCtrl != null)
                    {
                        if (scrollright)
                            _droneBase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Turn_R);
                        else
                            _droneBase.droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Turn_L);
                    }

                    scrollNum++;
                    if(scrollNum > 2)
                    {
                        scrollNum = 0;
                        scrollOk = true;
                    }
                }
            }
        }
    }
}
