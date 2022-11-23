using UnityEngine;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public Button GameStartBtn;
    public Button ConfigBtn;
    public Button GameEndBtn;
    public GameObject ConfigPanel;

    public GameObject SceneChangerIn = null;
    public GameObject SceneChangerOut = null;
    float titleSoundDelay = -0.5f;

    public GameObject cheatPanel;
    public Button cheatModeBtn;

    public GameObject clearDataPanel;
    public Button clearDataBtn;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        SceneChangerOut.SetActive(true);
        titleSoundDelay = 1.0f;

        DataLoad();

        if (GameStartBtn != null)
            GameStartBtn.onClick.AddListener(() =>
            {
                SoundMgr.Instance.PlayBGM("");
                SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("SelectHeroScene");
            });

        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(() =>
            {
                ConfigPanel.SetActive(true);
            });

        if (GameEndBtn != null)
            GameEndBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        if (cheatModeBtn != null)
            cheatModeBtn.onClick.AddListener(() =>
            {
                cheatPanel.SetActive(true);
            });

        if (clearDataBtn != null)
            clearDataBtn.onClick.AddListener(() =>
            {
                clearDataPanel.SetActive(true);
            });

        PlayerSkillData.SkillInit();

        if(MonsterSkillData.MonSkillList.Count <= 0)
        MonsterSkillData.InitMonSKillData();


        //GlobalData.hero.ChangeSkill(new GlobalData.SkillPointer(PlayerSkillData.warriorSkill[9].SkillEff), PlayerSkillData.warriorSkill[9].skillInfo, PlayerSkillData.warriorSkill[9].skillCost, 2);
        //GlobalData.hero.ChangeSkill(new GlobalData.SkillPointer(PlayerSkillData.warriorSkill[PlayerSkillData.warriorSkill.Count - 1].SkillEff),
        //    PlayerSkillData.warriorSkill[PlayerSkillData.warriorSkill.Count - 1].skillInfo,
        //    PlayerSkillData.warriorSkill[PlayerSkillData.warriorSkill.Count - 1].skillCost, 3);


        //if(PlayerSkillData.warriorSkill.Count > 0)
        //{
        //    for(int i =0; i < PlayerSkillData.warriorSkill.Count; i++)
        //    {
        //        Debug.Log("SkillName : " + PlayerSkillData.warriorSkill[i].skillName +
        //                  "SKillOption : " + PlayerSkillData.warriorSkill[i].skillOption +
        //                  "MinDmg : " + PlayerSkillData.warriorSkill[i].minDmg +
        //                  "MaxDmg : " + PlayerSkillData.warriorSkill[i].maxDmg);
        //    }
        //}

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (titleSoundDelay >= 0.0f)
        {
            titleSoundDelay -= Time.deltaTime;
            if(titleSoundDelay <= 0.0f)
            {
                SoundMgr.Instance.PlayBGM("TitleMusic", 0.2f);
                SceneChangerOut.GetComponent<SceneChanger>().ChangeStart("");
            }
        }
    }


    void DataLoad()
    {
        //저장데이터 로드
        if (PlayerPrefs.GetInt("WarriorClear") == 0)
            GlobalData.isWarriorClear = false;
        else
            GlobalData.isWarriorClear = true;

        if (PlayerPrefs.GetInt("ThiefClear") == 0)
            GlobalData.isThiefClear = false;
        else
            GlobalData.isThiefClear = true;

        if (PlayerPrefs.GetInt("StageTutorial") == 1)
            GlobalData.StagetutorialOn = false;
        else
            GlobalData.StagetutorialOn = true;

        if (PlayerPrefs.GetInt("BattleTutorial") == 1)
            GlobalData.BattletutorialOn = false;
        else
            GlobalData.BattletutorialOn = true;

        if (PlayerPrefs.GetInt("ShopTutorial") == 1)
            GlobalData.ShoptutorialOn = false;
        else
            GlobalData.ShoptutorialOn = true;

        if (PlayerPrefs.GetInt("EquipTutorial") == 1)
            GlobalData.EquiptutorialOn = false;
        else
            GlobalData.EquiptutorialOn = true;
    }
}