using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class MotherMutantFindTarget : MonoBehaviour
    {
        public MotherMutantBase motherBase = null;
        public MotherMutantMoveToTarget motherMove = null;
        public bool debugMode = true;

        //랜덤값 저장을 위한 리스트, 랜덤 범위 지정
        public List<Vector2> randPoses;
        public float maxRange;

        //계산용 변수
        [HideInInspector] public Vector2 firstPos;
        [HideInInspector] public Vector2 goPos;

        //애니메이션 해쉬
        [HideInInspector] int soundedHash = Animator.StringToHash("Sounded");

        //랜덤값 받기위한 변수
        float randX, randY;
        Vector2 cacVec = Vector2.zero;

        //소리들었는지
        bool isSounded = false;

        int obMask; //Obstacle 레이어 마스크

        private void Start() => StartFunc();

        private void StartFunc()
        {
            motherBase = GetComponent<MotherMutantBase>();
            this.TryGetComponent(out motherMove);

            obMask = 1 << LayerMask.NameToLayer("Obstacle");
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
        /// 마더뮤턴트 랜덤좌표 생성 함수
        /// </summary>
        public void MotherMake()
        {
            if(isSounded == true)
            {
                //Debug.Log("소리듣고 움직여서 발동안함");
                isSounded = false;
                return;
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
                motherBase._aipath.destination = goPos;

                if(motherBase.mutantList.Length > 0)
                {
                    for(int j = 0; j < motherBase.mutantList.Length; j++)
                    {
                        AIDestinationSetter _aiSetter = null;
                        motherBase.mutantList[j].TryGetComponent(out _aiSetter);
                        if(_aiSetter != null)
                        {
                            if(motherBase.babyFollowPoses.Length > j)
                                _aiSetter.target = motherBase.babyFollowPoses[j];
                            else
                                _aiSetter.target = motherBase.babyFollowPoses[j % 2];
                            _aiSetter.enabled = true;
                        }

                        //FindNewRandomTarget findRT = null;
                        //motherBase.mutantList[j].TryGetComponent(out findRT);
                        //if(findRT != null)
                        //{
                        //    //findRT.NowPos = goPos;
                        //    //findRT.m_RandPos.Clear();
                        //    //findRT.CheckNarrow();
                        //    //findRT.MakeRandPos();

                        //}
                    }
                }

                return;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            RootMain.Breakable_Wall_Ctrl breakwall;
            if(collision.gameObject.TryGetComponent(out breakwall))
            {
                motherBase._aipath.destination = firstPos;
                motherMove.reTargetTimer = 20.0f;
                goPos = firstPos;
            }
        }

        //public void SoundCheck(Vector3 soundFrom)
        //{
        //    // 사운드 발생이 20 거리보다 멀리에 있으면 리턴
        //    if ((soundFrom - this.transform.position).magnitude >= 20.0f)
        //        return;

        //    isSounded = true;   //소리들었음

        //    float maxpos = 5.0f;

        //    for (int i = 0; i < 100; i++)
        //    {
        //        randX = Random.Range(soundFrom.x - maxpos, soundFrom.x + maxpos);
        //        randY = Random.Range(soundFrom.y - maxpos, soundFrom.y + maxpos);

        //        cacVec = new Vector2(randX, randY);
        //        Collider2D check = Physics2D.OverlapCircle(cacVec, 1.2f, obMask);

        //        if (check != null)
        //            continue;

        //        //Debug.Log(cacVec);
        //        goPos = cacVec;
        //        motherBase._aipath.destination = goPos;
        //        motherBase._animator.SetTrigger(soundedHash);
        //        return;
        //    }
        //}
    }
}
