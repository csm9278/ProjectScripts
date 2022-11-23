using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public Text InfoText;
    public Text ChangeSkillText;
    public Button InfoBtn;
    public Button BackBtn;
    public Button ChangeBtn;
    float soundDelay = -0.5f;

    public Button[] SkillBtns;
    public Sprite[] SkillCostImgs;

    int ChangeIdx = 0;
    public GameObject sceneChangerIn;
    public GameObject sceneChangerOut;
    int randSkill;
    private void Start() => StartFunc();

    private void StartFunc()
    {
        soundDelay = 1.0f;
        sceneChangerOut.GetComponent<SceneChanger>().ChangeStart();

        if(GlobalData.heroType == GlobalData.HeroType.Warrior)
        {
            randSkill = Random.Range(0, PlayerSkillData.warriorSkill.Count);
            ChangeSkillText.text = PlayerSkillData.warriorSkill[randSkill].skillInfo;
            InfoBtn.GetComponentsInChildren<Image>()[1].sprite = SkillCostImgs[PlayerSkillData.warriorSkill[randSkill].skillCost - 1];
        }
        else if (GlobalData.heroType == GlobalData.HeroType.Assasin)
        {
            randSkill = Random.Range(0, PlayerSkillData.thiefSkill.Count);
            ChangeSkillText.text = PlayerSkillData.thiefSkill[randSkill].skillInfo;
            InfoBtn.GetComponentsInChildren<Image>()[1].sprite = SkillCostImgs[PlayerSkillData.thiefSkill[randSkill].skillCost - 1];
        }


        for (int i = 0; i < SkillBtns.Length; i++)
        {
            int idx = i;
            SkillBtns[i].GetComponentInChildren<Text>().text = GlobalData.hero.SkillInfo[i];

            if(GlobalData.hero.SkillCost[idx] != 0)
                SkillBtns[i].GetComponentsInChildren<Image>()[1].sprite = SkillCostImgs[GlobalData.hero.SkillCost[idx] - 1];

            SkillBtns[idx].onClick.AddListener(() =>
            {
                ChangeIdx = idx;
                InfoText.text = "È¹µæÇÏ±â ½Ã " + (idx + 1) + "¹ø ½ºÅ³°ú ¹Ù²ò´Ï´Ù!";
                InfoText.gameObject.SetActive(true);
                ChangeBtn.interactable = true;
            });
        }

        BackBtn.onClick.AddListener(() =>
        {
            sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("StageScene");
        });

        ChangeBtn.onClick.AddListener(() =>
        {
            if(GlobalData.heroType == GlobalData.HeroType.Warrior)
                GlobalData.hero.ChangeSkill(PlayerSkillData.warriorSkill[randSkill].SkillEff, PlayerSkillData.warriorSkill[randSkill].skillInfo, PlayerSkillData.warriorSkill[randSkill].skillCost, ChangeIdx);
            else if(GlobalData.heroType == GlobalData.HeroType.Assasin)
                GlobalData.hero.ChangeSkill(PlayerSkillData.thiefSkill[randSkill].SkillEff, PlayerSkillData.thiefSkill[randSkill].skillInfo, PlayerSkillData.thiefSkill[randSkill].skillCost, ChangeIdx);


            SoundMgr.Instance.PlayEffSound("NextScene");
            sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("StageScene");
        });

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        SoundDelayFunc();
    }

    void SoundDelayFunc()
    {
        if (soundDelay >= 0.0f)
        {
            soundDelay -= Time.deltaTime;
            if (soundDelay <= 0.0f)
                SoundMgr.Instance.PlayBGM("EventSound");
        }
    }
}