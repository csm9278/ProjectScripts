using UnityEngine;
using UnityEngine.UI;

public class SelectHeroMgr : MonoBehaviour
{
    public GameObject sceneChangerIn;
    public GameObject sceneChangerOut;

    float LobbySoundDelay = 1.0f;

    public Button selectHeroBtn;
    public Button goTitleBtn;

    public Button warriorBtn;
    public Button thiefBtn;
    public Button mageBtn;

    public Text thiefInfoText;
    public Image thiefIcon;

    public Text mageInfoText;
    public Image mageIcon;

    public Button TutorialClear;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        sceneChangerOut.GetComponent<SceneChanger>().ChangeStart();

        if (GlobalData.isWarriorClear)
        {
            thiefBtn.interactable = true;
            thiefInfoText.text = "독에 능숙한 영웅";
            thiefIcon.color = new Color32(255, 255, 255, 255);
        }

        if (GlobalData.isThiefClear)
        {
            //mageBtn.interactable = true;
            mageInfoText.text = "마법사\nComming Soon...";
            thiefIcon.color = new Color32(255, 255, 255, 255);
        }

        if (goTitleBtn != null)
            goTitleBtn.onClick.AddListener(() =>
            {
                SoundMgr.Instance.PlayBGM("");
                SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("TitleScene");
            });

        if (selectHeroBtn != null)
            selectHeroBtn.onClick.AddListener(() =>
            {
                GlobalData.Reset();

                if(GlobalData.heroType == GlobalData.HeroType.Warrior)
                {
                    GlobalData.hero.ChangeSkill(new GlobalData.SkillPointer(PlayerSkillData.warriorSkill[0].SkillEff), PlayerSkillData.warriorSkill[0].skillInfo, PlayerSkillData.warriorSkill[0].skillCost, 0);
                    GlobalData.hero.ChangeSkill(new GlobalData.SkillPointer(PlayerSkillData.warriorSkill[1].SkillEff), PlayerSkillData.warriorSkill[1].skillInfo, PlayerSkillData.warriorSkill[1].skillCost, 1);
                    //GlobalData.hero.ChangeSkill(new GlobalData.SkillPointer(PlayerSkillData.warriorSkill[PlayerSkillData.warriorSkill.Count - 1].SkillEff), PlayerSkillData.warriorSkill[PlayerSkillData.warriorSkill.Count - 1].skillInfo,
                    //    PlayerSkillData.warriorSkill[PlayerSkillData.warriorSkill.Count - 1].skillCost, 2);
                }
                else if(GlobalData.heroType == GlobalData.HeroType.Assasin)
                {
                    GlobalData.hero.ChangeSkill(new GlobalData.SkillPointer(PlayerSkillData.thiefSkill[0].SkillEff), PlayerSkillData.thiefSkill[0].skillInfo, PlayerSkillData.thiefSkill[0].skillCost, 0);
                }   

                SoundMgr.Instance.PlayBGM("");
                SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("StageScene");
            });

        if (warriorBtn != null)
            warriorBtn.onClick.AddListener(() =>
            {
                GlobalData.heroType = GlobalData.HeroType.Warrior;
                GlobalData.heroName = "전사";
                selectHeroBtn.interactable = true;
            });

        if (thiefBtn != null)
            thiefBtn.onClick.AddListener(() =>
            {
                GlobalData.heroType = GlobalData.HeroType.Assasin;
                GlobalData.heroName = "도적";
                selectHeroBtn.interactable = true;
            });

        if (TutorialClear != null)
            TutorialClear.onClick.AddListener(() =>
            {
                GlobalData.StagetutorialOn = true;
                GlobalData.ShoptutorialOn = true;
                GlobalData.EquiptutorialOn = true;
                GlobalData.BattletutorialOn = true;
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(LobbySoundDelay >= 0.0f)
        {
            LobbySoundDelay -= Time.deltaTime;
            if (LobbySoundDelay <= 0.0f)
                SoundMgr.Instance.PlayBGM("CharacterSelect");
        }
    }
}