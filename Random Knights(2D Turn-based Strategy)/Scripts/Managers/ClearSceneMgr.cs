using UnityEngine;
using UnityEngine.UI;

public class ClearSceneMgr : MonoBehaviour
{

    public GameObject SceneChangerIn;
    public GameObject SceneChangerOut;
    float titleSoundDelay = -0.5f;

    public Button goTitleBtn;

    public Image heroIcon;

    public Sprite[] heroIconSps;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        SceneChangerOut.SetActive(true);
        titleSoundDelay = 1.0f;

        if (GlobalData.heroType == GlobalData.HeroType.Warrior)
            heroIcon.sprite = heroIconSps[0];
        else if (GlobalData.heroType == GlobalData.HeroType.Assasin)
            heroIcon.sprite = heroIconSps[1];
        else if (GlobalData.heroType == GlobalData.HeroType.Mage)
            heroIcon.sprite = heroIconSps[2];

        if (goTitleBtn != null)
            goTitleBtn.onClick.AddListener(() =>
            {
                switch(GlobalData.heroType)
                {
                    case GlobalData.HeroType.Warrior:
                        if(!GlobalData.isWarriorClear)
                        {
                            UnlockMgr.unlocktype = UnlockMgr.UnlockType.Thief;
                            GlobalData.isWarriorClear = true;
                            PlayerPrefs.SetInt("WarriorClear", 1);
                            SoundMgr.Instance.PlayBGM("");
                            SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                            SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("UnlockScene");
                        }
                        else
                        {
                            SoundMgr.Instance.PlayBGM("");
                            SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                            SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("titleScene");
                        }
                        break;

                    case GlobalData.HeroType.Assasin:
                        if (!GlobalData.isThiefClear)
                        {
                            GlobalData.isThiefClear = true;
                            PlayerPrefs.SetInt("ThiefClear", 1);
                            UnlockMgr.unlocktype = UnlockMgr.UnlockType.Mage;
                            SoundMgr.Instance.PlayBGM("");
                            SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                            SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("UnlockScene");
                        }
                        else
                        {
                            SoundMgr.Instance.PlayBGM("");
                            SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                            SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("titleScene");
                        }
                        break;

                    case GlobalData.HeroType.Mage:
                        SoundMgr.Instance.PlayBGM("");
                        SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                        SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("titleScene");
                        break;
                }
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (titleSoundDelay >= 0.0f)
        {
            titleSoundDelay -= Time.deltaTime;
            if (titleSoundDelay <= 0.0f)
            {
                SoundMgr.Instance.PlayBGM("ClearSound", 0.2f);
                SceneChangerOut.GetComponent<SceneChanger>().ChangeStart("");
            }
        }
    }
}