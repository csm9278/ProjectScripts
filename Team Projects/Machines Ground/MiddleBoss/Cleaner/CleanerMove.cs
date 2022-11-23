using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Cleaner
{
    public class CleanerMove : MonoBehaviour
    {
        CleanerBase cleanerBase;
        [SerializeField] float posRandDistance;

        [DllImport("Altair_VectorCalc")]
        private static extern float GetAngle(Vector3 dir, bool b = true);

        //이동타이머
        public float moveDelay;
        float curMoveDelay;
        Vector3 NextPos;
        Vector3 calcVec;
        bool isArrive = false;
        bool isPlayerChase = false;
        public float maxMoveTime = 5.0f;
        float moveTime = -.1f;

        //스킬기능
        [Header("--- SKill1(Dash) ---")]
        public Animator[] bladeAnimator; 
        public float dashSkillDelay = 0.0f;
        public float dashSkillTime = 0.0f;
        float curDashSkillDealy;
        bool isSkillCasting = false;
        WaitForSeconds dashWaitSec = new WaitForSeconds(0.1f);

        private void Start() => StartFunc();

        private void StartFunc()
        {
            cleanerBase = GetComponent<CleanerBase>();
            curMoveDelay = moveDelay;
            curDashSkillDealy = dashSkillDelay;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            SkillFunc();

            if (isSkillCasting)
                return;

            SetNextPos();
            MoveNextPos();
        }

        /// <summary>
        /// 다음 좌표 구하는 공식
        /// </summary>
        void SetNextPos()
        {
            if (isPlayerChase)  //플레이어의 위치로 이동
            {
                if (curMoveDelay >= 0.0f)
                {
                    curMoveDelay -= Time.deltaTime;
                    if (curMoveDelay <= 0.0f)
                    {
                        NextPos = cleanerBase.player.transform.position;
                        cleanerBase._aiPath.destination = NextPos;
                        isArrive = false;
                        moveTime = maxMoveTime;
                    }
                }
            }
            else    //지정된 랜덤 좌표로 이동
            {
                if (curMoveDelay >= 0.0f)
                {
                    curMoveDelay -= Time.deltaTime;
                    if (curMoveDelay <= 0.0f)
                    {
                        int rand = Random.Range(0, cleanerBase.MovePos.Length);
                        float x = Random.Range(cleanerBase.MovePos[rand].position.x - posRandDistance, cleanerBase.MovePos[rand].position.x + posRandDistance);
                        float y = Random.Range(cleanerBase.MovePos[rand].position.y - posRandDistance, cleanerBase.MovePos[rand].position.y + posRandDistance);
                        NextPos = new Vector2(x, y);
                        cleanerBase._aiPath.destination = NextPos;
                        isArrive = false;
                        moveTime = maxMoveTime;
                    }
                }
            }
        }

        /// <summary>
        /// 도착 판정
        /// </summary>
        void MoveNextPos()
        {
            if (isArrive)
                return;

            calcVec = NextPos - this.transform.position;
            if (calcVec.magnitude <= 0.5f)
            {
                curMoveDelay = moveDelay;
                isArrive = true;
                isPlayerChase = !isPlayerChase;
                moveTime = -0.1f;
            }

            MoveTimeCheck();
        }

        /// <summary>
        /// 일정시간 이동 후 좌표 재탐색
        /// </summary>
        void MoveTimeCheck()
        {
            if (moveTime >= 0.0f)
            {
                moveTime -= Time.deltaTime;
                if (moveTime <= 0.0f)
                {
                    int rand = Random.Range(0, cleanerBase.MovePos.Length);
                    float x = Random.Range(cleanerBase.MovePos[rand].position.x - posRandDistance, cleanerBase.MovePos[rand].position.x + posRandDistance);
                    float y = Random.Range(cleanerBase.MovePos[rand].position.y - posRandDistance, cleanerBase.MovePos[rand].position.y + posRandDistance);
                    NextPos = new Vector2(x, y);
                    cleanerBase._aiPath.destination = NextPos;
                    isArrive = false;
                    isPlayerChase = !isPlayerChase;
                    moveTime = maxMoveTime;
                }
            }
        }

        void SkillFunc()
        {
            if(curDashSkillDealy >= 0.0f)
            {
                curDashSkillDealy -= Time.deltaTime;
                if(curDashSkillDealy <= 0.0f)
                {
                    isSkillCasting = true;
                    cleanerBase.sawTrigger.isSawActive = true;
                    StartCoroutine(DashSKillCo());
                }
            }    
        }

        public void LookPlayer()
        {
            Vector2 dir = cleanerBase.player.transform.position - transform.position;
            float angle = GetAngle(dir);
            Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, angleAxis, 0.3f);
        }

        IEnumerator DashSKillCo()
        {
            Debug.Log("스킬 시전");
            for(int i = 0; i < bladeAnimator.Length; i++)
            {
                bladeAnimator[i].enabled = true;
            }

            cleanerBase._aiPath.maxSpeed = 6.0f;
            yield return StartCoroutine(DashReadyCo());


            Debug.Log("대쉬 시작");
            for (float i = 0; i < dashSkillTime; i+=0.1f)   //대쉬 시작
            {
                cleanerBase._aiPath.destination = cleanerBase.player.transform.position;
                yield return dashWaitSec;
            }

            for (int i = 0; i < bladeAnimator.Length; i++)
            {
                bladeAnimator[i].enabled = false;
            }

            cleanerBase._aiPath.maxSpeed = 4.0f;
            isSkillCasting = false;
            cleanerBase.sawTrigger.isSawActive = false;
            curDashSkillDealy = dashSkillDelay;
            yield break;
        }

        IEnumerator DashReadyCo()
        {
            float readyTime = 3.0f;
            while(true)
            {
                readyTime -= Time.deltaTime;
                LookPlayer();

                yield return new WaitForEndOfFrame();

                if (readyTime <= 0.0f)
                    yield break;
            }
        }
    }
}