using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Drone
{
    public class DroneFindTarget : MonoBehaviour, ICheckPlace
    {


        // EnemyBase
        public DroneBase droneBase = null;
        public AIPath _aipath;
        public Animator _animator;
        public DroneMoveToTarget droneMove = null;
        public int posIdx;
        public bool debugMode = true;

        //랜덤값 저장을 위한 리스트, 랜덤 범위 지정
        public List<Vector2> randPoses;
        public float maxRange;

        //계산용 변수
        [HideInInspector] public Vector2 firstPos;
        [HideInInspector] public Vector2 goPos;

        //애니메이션 해쉬
        [HideInInspector] int soundedHash = Animator.StringToHash("Sounded");
        [HideInInspector] int findPlayerHash = Animator.StringToHash("FindPlayer");

        //랜덤값 받기위한 변수
        float randX, randY;
        Vector2 cacVec = Vector2.zero;

        //소리들었는지
        bool isSounded = false;

        LayerMask obMask; //Obstacle 레이어 마스크

        private void Start() => StartFunc();

        private void StartFunc()
        {
            droneBase = GetComponent<DroneBase>();
            this.TryGetComponent(out droneMove);
            if (droneBase != null)
            {
                _aipath = droneBase._aipath;
                _animator = droneBase._animator;
            }

            obMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("MaintanaceArea");
            firstPos = this.gameObject.transform.position;
            debugMode = false;
        }

        private void OnDrawGizmos()
        {
            if(debugMode)
                Gizmos.DrawWireSphere(this.transform.position, maxRange);
            else
                Gizmos.DrawWireSphere(firstPos, maxRange);

            Gizmos.DrawSphere(goPos, 1.2f);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }

        /// <summary>
        /// 드론 전용 랜덤좌표 생성 함수
        /// </summary>
        public void DroneMake()
        {
            if(isSounded == true)
            {
                isSounded = false;
                return;
            }    

            if(droneBase.droneType == DroneBase.DroneType.Minimum)
            {
                int findRand = Random.Range(0, 100);
                if(findRand < GlobalData.minimumDroneChasePercent)   // 기본 25%확률로 플레이어 한테 이동
                {
                    droneBase.HittedEnemy();
                    return;
                }
            }


            for(int i = 0; i < 100; i++)
            {
                randX = Random.Range(firstPos.x - (maxRange * .5f), firstPos.x + (maxRange * .5f));
                randY = Random.Range(firstPos.y - (maxRange * .5f), firstPos.y + (maxRange * .5f));

                cacVec = new Vector2(randX, randY);

                Collider2D CheckColl = Physics2D.OverlapCircle(cacVec, 1.2f, obMask);

                if(CheckColl != null)
                {
                    continue;
                }

                goPos = cacVec;
                _aipath.destination = goPos;

                return;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            RootMain.Breakable_Wall_Ctrl breakwall;
            if(collision.gameObject.TryGetComponent(out breakwall))
            {
                _aipath.destination = firstPos;
                droneMove.reTargetTimer = 20.0f;
                goPos = firstPos;
            }
        }

        public void SoundCheck(Vector3 soundFrom)
        {
            // 사운드 발생이 20 거리보다 멀리에 있으면 리턴
            if ((soundFrom - this.transform.position).magnitude >= 20.0f)
                return;
            if (_animator.GetBool(findPlayerHash) == true)
                return;
            if (isSounded)
                return;

            isSounded = true;   //소리들었음

            float maxpos = 5.0f;

            for (int i = 0; i < 100; i++)
            {
                randX = Random.Range(soundFrom.x - maxpos, soundFrom.x + maxpos);
                randY = Random.Range(soundFrom.y - maxpos, soundFrom.y + maxpos);

                cacVec = new Vector2(randX, randY);
                Collider2D check = Physics2D.OverlapCircle(cacVec, 1.2f, obMask);

                if (check != null)
                    continue;

                goPos = cacVec;
                _aipath.destination = goPos;
                _animator.SetTrigger(soundedHash);
                return;
            }
        }
    }
}
