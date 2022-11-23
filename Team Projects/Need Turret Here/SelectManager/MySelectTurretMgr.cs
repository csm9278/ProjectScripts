using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Altair;

namespace Choi
{
    public class MySelectTurretMgr : MonoBehaviour
    {
        public List<GameObject> myTrList = new List<GameObject>();

        public GameObject[] trList;
        public GameObject[] trmoveList;

        public GameObject movingNode;
        public Button StartBtn;

        [SerializeField] private string sceneName;
        private void Start() => StartFunc();

        private void StartFunc()
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2((185.3f * GlobalData.choi_TurretMaxCount) + 5.0f, 272.0f);

            for(int i = 0; i < trList.Length;i++)
            {
                trList[i].GetComponent<SelectTurretNode>().selectType = SelectTurretNode.SelectType.MyPick;
                trList[i].GetComponent<SelectTurretNode>().mgr = this;

            }

            if (StartBtn != null)
                StartBtn.onClick.AddListener(StartBtnFunc);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }


        public void InitTr()
        {
            int idx = myTrList.Count - 1;   // ���� ������Ʈ ����Ʈ�� ������ �ε��� ������


            SelectTurretNode node;  // ��� �ϳ�
            node = trList[idx].GetComponent<SelectTurretNode>();    //���� ������ idx��° ����� ��ũ��Ʈ ����
            node.turretIdx = myTrList[idx].GetComponent<SelectTurretNode>().turretIdx;  //
            node.SetTurret();
            node.myPickNum = idx;
            node.turretImg.sprite = myTrList[idx].GetComponent<SelectTurretNode>().turretImg.sprite;
            myTrList[idx].GetComponent<SelectTurretNode>().myPickNum = idx;

            if (myTrList.Count >= Altair.GlobalData.choi_TurretMaxCount)
                StartBtn.interactable = true;

            //trList[idx].gameObject.SetActive(true);
        }

        public void RefreshMyTr()
        {
            // ��� trList ����
            for(int i = 0; i < trList.Length; i++)
            {
                trList[i].SetActive(false);
            }

            //List�� �ִ� ������ TrList �ϳ��� ���ָ鼭 ���� �Է�
            for (int i = 0; i < myTrList.Count; i++)
            {
                SelectTurretNode node;
                node = trList[i].GetComponent<SelectTurretNode>();
                node.turretIdx = myTrList[i].GetComponent<SelectTurretNode>().turretIdx;
                node.turretImg.sprite = myTrList[i].GetComponent<SelectTurretNode>().turretImg.sprite;
                node.SetTurret();
                node.myPickNum = i;
                trList[i].gameObject.SetActive(true);
            }

        }

        public void MovingNode(int myturretCount, Vector3 startPos, Vector3 endPos, bool isReturn)
        {
            int tridx = myTrList[myturretCount].GetComponent<SelectTurretNode>().turretIdx;
            movingNode.GetComponent<SelectTurretNode>().turretImg.sprite = myTrList[myturretCount].GetComponent<SelectTurretNode>().turretImg.sprite;

            movingNode.GetComponent<MovingNodeCtrl>().SetMovingNode(tridx, myturretCount, startPos, endPos, isReturn);
        }

        public void startBtnOffFunc()
        {
            StartBtn.interactable = false;
        }

        public void StartBtnFunc()
        {
            for(int i = 0; i < GlobalData.choi_TurretPickNums.Length; i++)
            {
                if (i >= GlobalData.choi_TurretMaxCount)
                {
                    GlobalData.choi_TurretPickNums[i] = -1;
                    continue;
                }

                GlobalData.choi_TurretPickNums[i] = myTrList[i].GetComponent<SelectTurretNode>().turretIdx;

            }

            sceneName = "stage" + (GlobalData.choi_Stage %100).ToString();
            
            SceneManager.LoadScene(sceneName);
        }
    }
}