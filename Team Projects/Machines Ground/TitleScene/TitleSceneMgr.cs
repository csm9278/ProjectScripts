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
                    infoText.text = "즐기기 위한 난이도 \n\n체력 넘쳐남\n탄창 넘쳐남\n적들 안넘쳐남";
                    break;
                case 1:
                    infoText.text = "보통 난이도 \n\n체력 적당함\n탄창 적당함\n적들 적당함";
                    break;
                case 2:
                    infoText.text = "위험한 플레이를 원하는 사람들을 위한 난이도 \n\n체력 적당함\n탄창 모자람\n적들 상당히 많음";
                    break;
                case 3:
                    infoText.text = "미친 게임 난이도 \n\n체력 너덜너덜함\n탄창 항상 모자람\n적들 매우 많음";
                    break;
                case 4:
                    infoText.text = "개발자가 무슨 생각으로 만들었는지 궁금해지는 난이도 \n\n차량이 맞는지 의문\n무기를 가끔 쏠수있음\n적들의 소굴";
                    break;
                case 5:
                    infoText.text = "종말을 체험해보고 싶은 사람들의 난이도 \n\n거의 맨몸\n탄창이 뭐지?\n주변에 적밖에 안보임";
                    break;
            }
        }

        public void GameStartFunc()
        {
            FadeinPanel.SetActive(true);
        }
    }
}