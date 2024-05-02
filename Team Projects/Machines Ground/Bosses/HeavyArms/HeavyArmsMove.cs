using UnityEngine;

namespace HeavyArms
{
    public class HeavyArmsMove : MonoBehaviour
    {
        HeavyArmsBase haBase;
        [SerializeField] float posRandDistance;
        bool isStart = false;

        //�̵�Ÿ�̸�
        public float moveDelay;
        float curMoveDelay;
        Vector3 NextPos;
        Vector3 calcVec;
        bool isArrive = false;
        bool isPlayerChase = false;
        public float maxMoveTime = 5.0f;
        float moveTime = -.1f;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            //����� �׸������� ����
            isStart = true;

            haBase = GetComponent<HeavyArmsBase>();
            curMoveDelay = moveDelay;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            SetNextPos();
            MoveNextPos();
        }

        /// <summary>
        /// ���� ��ǥ ���ϴ� ����
        /// </summary>
        void SetNextPos()
        {
            if (isPlayerChase)  //�÷��̾��� ��ġ�� �̵�
            {
                if (curMoveDelay >= 0.0f)
                {
                    //ADebug.Log(curMoveDelay);
                    curMoveDelay -= Time.deltaTime;
                    if (curMoveDelay <= 0.0f)
                    {
                        NextPos = haBase.player.transform.position;
                        haBase._aiPath.destination = NextPos;
                        isArrive = false;
                        moveTime = maxMoveTime;
                    }
                }
            }
            else    //������ ���� ��ǥ�� �̵�
            {
                if (curMoveDelay >= 0.0f)
                {
                    curMoveDelay -= Time.deltaTime;
                    if (curMoveDelay <= 0.0f)
                    {
                        float x;
                        float y;
                        if (!haBase.isRand)
                        {
                            int rand = Random.Range(0, haBase.MovePos.Length);
                            x = Random.Range(haBase.MovePos[rand].position.x - posRandDistance, haBase.MovePos[rand].position.x + posRandDistance);
                            y = Random.Range(haBase.MovePos[rand].position.y - posRandDistance, haBase.MovePos[rand].position.y + posRandDistance);
                        }
                        else
                        {
                            int rand = Random.Range(0, haBase.randVec.Length);
                            x = Random.Range(haBase.randVec[rand].x - posRandDistance, haBase.randVec[rand].x + posRandDistance);
                            y = Random.Range(haBase.randVec[rand].y - posRandDistance, haBase.randVec[rand].y + posRandDistance);
                        }

                        NextPos = new Vector2(x, y);
                        haBase._aiPath.destination = NextPos;
                        isArrive = false;
                        moveTime = maxMoveTime;
                    }
                }
            }
        }

        /// <summary>
        /// ���� ����
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
        /// �����ð� �̵� �� ��ǥ ��Ž��
        /// </summary>
        void MoveTimeCheck()
        {
            if(moveTime >= 0.0f)
            {
                moveTime -= Time.deltaTime;
                if(moveTime <= 0.0f)
                {
                    float x;
                    float y;
                    if (!haBase.isRand)
                    {
                        int rand = Random.Range(0, haBase.MovePos.Length);
                        x = Random.Range(haBase.MovePos[rand].position.x - posRandDistance, haBase.MovePos[rand].position.x + posRandDistance);
                        y = Random.Range(haBase.MovePos[rand].position.y - posRandDistance, haBase.MovePos[rand].position.y + posRandDistance);
                    }
                    else
                    {
                        int rand = Random.Range(0, haBase.randVec.Length);
                        x = Random.Range(haBase.randVec[rand].x - posRandDistance, haBase.randVec[rand].x + posRandDistance);
                        y = Random.Range(haBase.randVec[rand].y - posRandDistance, haBase.randVec[rand].y + posRandDistance);
                    }
                    NextPos = new Vector2(x, y);
                    haBase._aiPath.destination = NextPos;
                    isArrive = false;
                    isPlayerChase = !isPlayerChase;
                    moveTime = maxMoveTime;
                }
            }
        }
    }
}