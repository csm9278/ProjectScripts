using UnityEngine;
using UnityEngine.UI;

public class TutorialPanelMgr : MonoBehaviour
{
    public enum TutorialType
    {
        Stage,
        Battle,
        Shop,
        Equip
    }


    public GameObject[] tutorialObjs;
    public GameObject tutorialPanel;
    public Button NextBtn;
    public Button BackBtn;

    public Button OkBtn;

    int idx = 0;

    public TutorialType tutorialType = TutorialType.Battle;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        TutorialOnOff();

        if (OkBtn != null)
            OkBtn.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });

        if (NextBtn != null)
            NextBtn.onClick.AddListener(() =>
            {
                if (tutorialObjs.Length >= idx + 1)
                {
                    tutorialObjs[idx].gameObject.SetActive(false);
                    idx++;
                    tutorialObjs[idx].gameObject.SetActive(true);

                    if (idx + 1 == tutorialObjs.Length)
                    {
                        NextBtn.gameObject.SetActive(false);
                        OkBtn.gameObject.SetActive(true);
                    }

                    if (idx != 0)
                        BackBtn.gameObject.SetActive(true);
                }
            });

        if (BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                if (0 <= idx - 1)
                {
                    tutorialObjs[idx].gameObject.SetActive(false);
                    idx--;
                    tutorialObjs[idx].gameObject.SetActive(true);

                    if (idx + 1 < tutorialObjs.Length)
                    {
                        NextBtn.gameObject.SetActive(true);
                        OkBtn.gameObject.SetActive(false);
                    }

                    if (idx == 0)
                        BackBtn.gameObject.SetActive(false);
                }
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public void TutorialOnOff()
    {
        switch(tutorialType)
        {
            case TutorialType.Stage:
                if (GlobalData.StagetutorialOn)
                {
                    tutorialPanel.gameObject.SetActive(true);
                    NextBtn.gameObject.SetActive(true);
                    GlobalData.StagetutorialOn = false;
                    PlayerPrefs.SetInt("StageTutorial", 1);
                }
                else
                    this.gameObject.SetActive(false);
                break;

            case TutorialType.Battle:
                if (GlobalData.BattletutorialOn)
                {
                    tutorialPanel.gameObject.SetActive(true);
                    NextBtn.gameObject.SetActive(true);
                    GlobalData.BattletutorialOn = false;
                    PlayerPrefs.SetInt("BattleTutorial", 1);
                }
                else
                    this.gameObject.SetActive(false);
                break;

            case TutorialType.Shop:
                if (GlobalData.ShoptutorialOn)
                {
                    tutorialPanel.gameObject.SetActive(true);
                    NextBtn.gameObject.SetActive(true);
                    GlobalData.ShoptutorialOn = false;
                    PlayerPrefs.SetInt("ShopTutorial", 1);
                }
                else
                    this.gameObject.SetActive(false);
                break;

            case TutorialType.Equip:
                if (GlobalData.EquiptutorialOn)
                {
                    tutorialPanel.gameObject.SetActive(true);
                    NextBtn.gameObject.SetActive(true);
                    GlobalData.EquiptutorialOn = false;
                    PlayerPrefs.SetInt("EquipTutorial", 1);
                }
                else
                    this.gameObject.SetActive(false);
                break;
        }
    }
}