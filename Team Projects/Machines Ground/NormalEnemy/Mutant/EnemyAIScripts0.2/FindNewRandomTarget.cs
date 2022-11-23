using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace Mutant
{
    public class FindNewRandomTarget : MonoBehaviour
    {   //적의 순찰포인트를 랜덤으로 지정하는 스크립트입니다.

        //수정해야 할 부분
        //1. 좁은 길 배치될때 랜덤 포인트 구하지 못하는 현상 수정
        //2. 체이스 상태에서 돌아갈때 그 위치로 부터 랜덤 포인트 지정
        //3. 처음의 위치에서 너무 멀어질시 돌아가는 타입 / 

        // EnemyBase
        public EnemyBase_2 enemyBase = null;
        EnemyMoveToRandTarget enemyMove = null;
        public AIPath aiPath;
        public int posIdx;

        //랜덤값 저장을 위한 리스트, 랜덤 범위 지정
        public List<Vector2> m_RandPos;
        public float MaxRange;

        //계산용 변수
        [HideInInspector] public Vector2 NowPos;    //현재 위치
        [HideInInspector] public Vector2 firstPos;
        Transform CacTr;

        //랜덤값 받기위한 변수
        float RandX, RandY;
        Vector2 CacVec = Vector2.zero;
        int SafeIndex;
        bool IsNarrow = false;

        int ObMask; //Obstacle 레이어 마스크

        private void Awake()
        {

        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            enemyBase = GetComponent<EnemyBase_2>();
            if (enemyBase != null)
            {
                aiPath = enemyBase._aipath;
            }
            this.TryGetComponent(out enemyMove);

            firstPos = this.transform.position;

            ObMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("MaintanaceArea");
            NowPos = this.gameObject.transform.position;

            CheckNarrow();
            MakeRandPos();
        }

        private void OnDrawGizmos()
        {
            if (m_RandPos.Count > 0)
            {
                for (int i = 0; i < m_RandPos.Count; i++)
                {
                    if (i != m_RandPos.Count - 1)
                        Debug.DrawLine(m_RandPos[i], m_RandPos[i + 1], Color.blue);
                    Gizmos.DrawWireSphere(m_RandPos[i], 0.7f);
                }
            }
        }

        //private void Update() => UpdateFunc();

        //private void UpdateFunc()
        //{
        //    if(Input.GetKeyDown(KeyCode.Space) && MaxRange > 3.5f && SafeIndex < 100)
        //    {
        //        MakeRandPos();
        //    }
        //}

        public void SetNextPos()
        {
            if (m_RandPos.Count <= 0)   //저장되어있는 Pos가 없으면 리턴
                return;

            if (aiPath == null)
                return;

            posIdx++;
            if (posIdx >= m_RandPos.Count)
            {
                posIdx = 0;
                MakeRandPos();
            }

            if (m_RandPos.Count <= 0)   //길찾기를 할 수없는 상황
                return;

            aiPath.destination = m_RandPos[posIdx];
        }

        public void CheckNarrow()
        {
            Collider2D CheckR = Physics2D.OverlapCircle(NowPos, MaxRange, ObMask);
            if (CheckR == null)
                IsNarrow = false;
            else
            {
                IsNarrow = true;
            }
        }

        public void MakeRandPos()
        {
            if (MaxRange < 3.5f)   //최소 거리를 3.5로 해놓았기 때문에 이거보다 작으면 무한로딩..
                return;

            m_RandPos.Clear();

            if (IsNarrow)
            {
                NarrowMake();
            }
            else
            {
                NormalMake();
            }
        }

        void NarrowMake()   //좁은지형 전용 랜덤 좌표 생성
        {
            for (int i = 0; i < 2; i++)
            {
                RandX = Random.Range(NowPos.x - (MaxRange * .5f), NowPos.x + (MaxRange * .5f));
                RandY = Random.Range(NowPos.y - (MaxRange * .5f), NowPos.y + (MaxRange * .5f));

                CacVec = new Vector2(RandX, RandY);

                Collider2D CheckColl = Physics2D.OverlapCircle(CacVec, 1.2f, ObMask);

                if (i == 0)
                {
                    Vector2 vecdir = CacVec - (Vector2)this.transform.position;

                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vecdir, vecdir.magnitude, ObMask);
                    if (hit)
                    {
                        Debug.DrawRay(this.transform.position, vecdir, Color.red, 0.3f);
                        i--;
                        continue;
                    }
                }
                else
                {
                    Vector2 vecdir = CacVec - m_RandPos[i - 1];

                    RaycastHit2D hit = Physics2D.Raycast(m_RandPos[i - 1], vecdir, vecdir.magnitude, ObMask);
                    if (hit)
                    {
                        Debug.DrawRay(m_RandPos[i - 1], vecdir, Color.red, 0.3f);
                        i--;
                        continue;
                    }
                }

                if (CheckColl != null)
                {
                    i--;
                    continue;
                }

                m_RandPos.Add(CacVec);
            }


        }

        void NormalMake()   //넓은 지형 전용 랜덤 좌표 생성
        {
            for (int i = 0; i < 3; i++)  //3개정도의 랜덤좌표 생성
            {
                RandX = Random.Range(NowPos.x - MaxRange, NowPos.x + MaxRange);
                RandY = Random.Range(NowPos.y - MaxRange, NowPos.y + MaxRange);

                CacVec = new Vector2(RandX, RandY);

                Collider2D CheckColl = Physics2D.OverlapCircle(CacVec, 1.2f, ObMask);

                if (i == 0)
                {
                    Vector2 vecdir = CacVec - (Vector2)this.transform.position;

                    if (vecdir.magnitude <= 3.5f)
                    {
                        Debug.DrawRay(this.transform.position, vecdir, Color.green, 0.3f);
                        //Debug.Log("거리가 3.5미만임");
                        i--;
                        continue;
                    }

                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vecdir, vecdir.magnitude, ObMask);
                    if (hit)
                    {
                        Debug.DrawRay(this.transform.position, vecdir, Color.red, 0.3f);
                        //Debug.Log("벽에 걸림");
                        i--;
                        continue;
                    }
                }
                else
                {
                    Vector2 vecdir = CacVec - m_RandPos[i - 1];

                    if (vecdir.magnitude <= 3.5f)
                    {
                        Debug.DrawRay(m_RandPos[i - 1], vecdir, Color.green, 0.3f);
                        //Debug.Log("거리가 3.5미만임");
                        i--;
                        continue;
                    }

                    RaycastHit2D hit = Physics2D.Raycast(m_RandPos[i - 1], vecdir, vecdir.magnitude, ObMask);
                    if (hit)
                    {
                        Debug.DrawRay(m_RandPos[i - 1], vecdir, Color.red, 0.3f);
                        //Debug.Log("벽에 걸림");
                        i--;
                        continue;
                    }
                }

                if (CheckColl != null)
                {
                    //Debug.Log("벽에 걸림");
                    i--;
                    continue;
                }

                m_RandPos.Add(CacVec);
            }
            SafeIndex = 0;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            RootMain.Breakable_Wall_Ctrl breakwall;
            if (collision.gameObject.TryGetComponent(out breakwall))
            {
                m_RandPos.Clear();
                posIdx = 0;
                m_RandPos.Add(firstPos);
                aiPath.destination = m_RandPos[posIdx];
                enemyMove.reTargetTimer = 5.0f;
            }

        }
    }

}