using UnityEngine;
using UnityEngine.UI;
using Altair;

namespace Choi
{
    public class SelectTurretNode : MonoBehaviour
    {
        public enum SelectType
        {
            MyPick,
            UnPick,
            Moving
        }

        [HideInInspector] public string trName;
        [HideInInspector] public int trCost;
        public int myPickNum = -1;
        bool isFront = false;

        public string IconRSC = null;

        public Image turretImg;

        public int turretIdx;

        public Text trNameText;
        public Text trCostText;

        Button nodeBtn;
        Color selectColor = new Color32(159, 158, 153, 255);
        Color firstColor;

        public MySelectTurretMgr mgr;

        public SelectType selectType = SelectType.UnPick;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            nodeBtn = GetComponent<Button>();

            firstColor = turretImg.color;

            if (IconRSC != null && selectType == SelectType.UnPick)
            {
                turretImg.sprite = Resources.Load<Sprite>(IconRSC);
            }

            nodeBtn.onClick.AddListener(NodeBtnFunc);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (mgr.movingNode.GetComponent<MovingNodeCtrl>().moving == MovingNodeCtrl.Moving.Moving)
            {
                if (Input.GetMouseButtonDown(0))
                    mgr.movingNode.GetComponent<MovingNodeCtrl>().ArriveFunc();
            }
        }

        public void NodeBtnFunc()
        {
            if (selectType == SelectType.Moving)
                return;

            if (mgr.movingNode.GetComponent<MovingNodeCtrl>().moving == MovingNodeCtrl.Moving.Moving)
            {
                mgr.movingNode.GetComponent<MovingNodeCtrl>().ArriveFunc();
            }

            if (selectType == SelectType.UnPick)    //여기가 터렛추가하는 버튼을 눌렀을 때
            {
                //내 업글 슬롯 칸보다 더 넣으려고 할시 리턴
                if (mgr.myTrList.Count >= GlobalData.choi_TurretMaxCount)
                {
                    return;
                }
                
                mgr.myTrList.Add(this.gameObject);  // 터렛 매니저 게임오브젝트 리스트에 누른 터렛의 게임오브젝트 추가
                mgr.InitTr();

                mgr.MovingNode(myPickNum, this.transform.position, mgr.trList[myPickNum].transform.position, false);
                ChangeColor(false);
            }
            else    //내 터렛 리스트에서 제거할 때
            {

                if(mgr.StartBtn.interactable == true)   //버튼을 사용할 수 없게 막음
                    mgr.startBtnOffFunc();

                isFront = false;
                mgr.trList[myPickNum].gameObject.SetActive(false);
                mgr.MovingNode(myPickNum, this.transform.position, mgr.myTrList[myPickNum].transform.position, true);

            }
        }

        public void SetTurret()
        {
            trNameText.text = GlobalData.choi_TurretNameList[turretIdx];
            trCostText.text = GlobalData.choi_m_TrList[turretIdx].m_cost.ToString() + "$";
 
        }

        public void ChangeColor(bool isPick)
        {
            if(isPick)
            {
                nodeBtn.interactable = true;
                turretImg.color = firstColor;
            }
            else
            {
                nodeBtn.interactable = false;
                turretImg.color = selectColor;
            }
        }
    }
}