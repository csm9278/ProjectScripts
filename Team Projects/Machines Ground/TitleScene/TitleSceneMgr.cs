using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RootMain
{
    public class TitleSceneMgr : MonoBehaviour
    {
        public Button startBtn;
        public Button settingBtn;
        public Button collectionBtn;
        public Button exitBtn;

        public GameObject FirstRoot;
        public GameObject GameStartRoot;
        public Button[] difficultyBtns;

        public Text infoText;
        public GameObject FadeinPanel;

        public Button gameStartBackBtn;
        public Button difStartBtn;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (startBtn != null)
                startBtn.onClick.AddListener(() =>
                {
                    FirstRoot.gameObject.SetActive(false);
                    GameStartRoot.gameObject.SetActive(true);
                });

            if (gameStartBackBtn != null)
                gameStartBackBtn.onClick.AddListener(() =>
                {
                    FirstRoot.gameObject.SetActive(true);
                    GameStartRoot.gameObject.SetActive(false);
                });

            if (difStartBtn != null)
                difStartBtn.onClick.AddListener(GameStartFunc);

            for(int i = 0; i < difficultyBtns.Length;i++)
            {
                int idx = i;
                difficultyBtns[idx].onClick.AddListener(() =>
                {
                    if(difStartBtn.interactable == false)
                        difStartBtn.interactable = true;
                    SetInfoText(idx);
                });
            }
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        public void SetInfoText(int idx)
        {
            switch(idx)
            {
                case 0:
                    infoText.text = "���� ���� ���̵� \n\nü�� ���ĳ�\nźâ ���ĳ�\n���� �ȳ��ĳ�";
                    break;
                case 1:
                    infoText.text = "���� ���̵� \n\nü�� ������\nźâ ������\n���� ������";
                    break;
                case 2:
                    infoText.text = "������ �÷��̸� ���ϴ� ������� ���� ���̵� \n\nü�� ������\nźâ ���ڶ�\n���� ����� ����";
                    break;
                case 3:
                    infoText.text = "��ģ ���� ���̵� \n\nü�� �ʴ��ʴ���\nźâ �׻� ���ڶ�\n���� �ſ� ����";
                    break;
                case 4:
                    infoText.text = "�����ڰ� ���� �������� ��������� �ñ������� ���̵� \n\n������ �´��� �ǹ�\n���⸦ ���� �������\n������ �ұ�";
                    break;
                case 5:
                    infoText.text = "������ ü���غ��� ���� ������� ���̵� \n\n���� �Ǹ�\nźâ�� ����?\n�ֺ��� ���ۿ� �Ⱥ���";
                    break;
            }
        }

        public void GameStartFunc()
        {
            FadeinPanel.SetActive(true);
        }
    }
}