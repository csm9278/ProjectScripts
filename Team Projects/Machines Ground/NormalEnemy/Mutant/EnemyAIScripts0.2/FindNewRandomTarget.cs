using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace Mutant
{
    public class FindNewRandomTarget : MonoBehaviour
    {   //���� ��������Ʈ�� �������� �����ϴ� ��ũ��Ʈ�Դϴ�.

        //�����ؾ� �� �κ�
        //1. ���� �� ��ġ�ɶ� ���� ����Ʈ ������ ���ϴ� ���� ����
        //2. ü�̽� ���¿��� ���ư��� �� ��ġ�� ���� ���� ����Ʈ ����
        //3. ó���� ��ġ���� �ʹ� �־����� ���ư��� Ÿ�� / 

        // EnemyBase
        public EnemyBase_2 enemyBase = null;
        EnemyMoveToRandTarget enemyMove = null;
        public AIPath aiPath;
        public int posIdx;

        //������ ������ ���� ����Ʈ, ���� ���� ����
        public List<Vector2> m_RandPos;
        public float MaxRange;

        //���� ����
        [HideInInspector] public Vector2 NowPos;    //���� ��ġ
        [HideInInspector] public Vector2 firstPos;
        Transform CacTr;

        //������ �ޱ����� ����
        float RandX, RandY;
        Vector2 CacVec = Vector2.zero;
        int SafeIndex;
        bool IsNarrow = false;

        int ObMask; //Obstacle ���̾� ����ũ

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
            if (m_RandPos.Count <= 0)   //����Ǿ��ִ� Pos�� ������ ����
                return;

            if (aiPath == null)
                return;

            posIdx++;
            if (posIdx >= m_RandPos.Count)
            {
                posIdx = 0;
                MakeRandPos();
            }

            if (m_RandPos.Count <= 0)   //��ã�⸦ �� ������ ��Ȳ
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
            if (MaxRange < 3.5f)   //�ּ� �Ÿ��� 3.5�� �س��ұ� ������ �̰ź��� ������ ���ѷε�..
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

        void NarrowMake()   //�������� ���� ���� ��ǥ ����
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

        void NormalMake()   //���� ���� ���� ���� ��ǥ ����
        {
            for (int i = 0; i < 3; i++)  //3�������� ������ǥ ����
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
                        //Debug.Log("�Ÿ��� 3.5�̸���");
                        i--;
                        continue;
                    }

                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vecdir, vecdir.magnitude, ObMask);
                    if (hit)
                    {
                        Debug.DrawRay(this.transform.position, vecdir, Color.red, 0.3f);
                        //Debug.Log("���� �ɸ�");
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
                        //Debug.Log("�Ÿ��� 3.5�̸���");
                        i--;
                        continue;
                    }

                    RaycastHit2D hit = Physics2D.Raycast(m_RandPos[i - 1], vecdir, vecdir.magnitude, ObMask);
                    if (hit)
                    {
                        Debug.DrawRay(m_RandPos[i - 1], vecdir, Color.red, 0.3f);
                        //Debug.Log("���� �ɸ�");
                        i--;
                        continue;
                    }
                }

                if (CheckColl != null)
                {
                    //Debug.Log("���� �ɸ�");
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