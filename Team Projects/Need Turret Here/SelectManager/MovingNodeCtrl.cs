using UnityEngine;
using UnityEngine.UI;
using Altair;

namespace Choi
{
    public class MovingNodeCtrl : MonoBehaviour
    {

        public enum Moving
        {
            Moving,
            Idle,
        }

        public Text nameText;
        public Text costText;
        public Image image;

        public int turretIdx;
        public int onIdx;
        MySelectTurretMgr mgr;
        bool returnType = false;

        public Vector3 arrivePos;
        public Vector3 goPos;

        public Moving moving = Moving.Idle;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            mgr = FindObjectOfType<MySelectTurretMgr>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }

        private void FixedUpdate()
        {
            goPos = arrivePos - this.transform.position;


            //this.transform.position += goPos.normalized * Time.deltaTime * 0.00001f;

            //this.transform.Translate(goPos.normalized * 10.0f);

            this.transform.position = Vector2.Lerp(this.transform.position, arrivePos, 0.15f);


            if (goPos.magnitude <= 3.0f)
            {
                ArriveFunc();
            }
        }

        public void SetMovingNode(int idx, int myturretCount, Vector3 startpos, Vector3 endpos, bool isReturn)
        {
            this.transform.position = startpos;

            onIdx = myturretCount;
            nameText.text = GlobalData.choi_TurretNameList[idx];
            costText.text = GlobalData.choi_m_TrList[idx].m_cost.ToString() + "$";
            //이미지 나중에

            returnType = isReturn;

            arrivePos = endpos;
            this.gameObject.SetActive(true);
            moving = Moving.Moving;
        }

        public void ArriveFunc()
        {
            if (returnType)
            {
                this.gameObject.SetActive(false);

                mgr.myTrList[onIdx].GetComponent<SelectTurretNode>().ChangeColor(true);


                mgr.myTrList.RemoveAt(onIdx);
                mgr.RefreshMyTr();
                moving = Moving.Idle;
            }
            else
            {
                this.gameObject.SetActive(false);
                mgr.trList[onIdx].SetActive(true);
                moving = Moving.Idle;
            }
        }
    }
}