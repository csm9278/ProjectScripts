using UnityEngine;
using UnityEngine.UI;

public class UnlockMgr : MonoBehaviour
{
    public enum UnlockType
    {
        Thief,
        Mage,
        None
    }

    public static UnlockType unlocktype = UnlockType.None; 
    public GameObject SceneChangerIn;
    public GameObject SceneChangerOut;
    float titleSoundDelay = -0.5f;

    public Button goTitleBtn;

    public Text infoText;
    public Image heroIcon;

    public Sprite[] heroIcons;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        SceneChangerOut.SetActive(true);
        titleSoundDelay = 1.0f;

        switch(unlocktype)
        {
            case UnlockType.Thief:
                infoText.text = "이제부터\n 도적을 사용할 수 있습니다!";
                heroIcon.sprite = heroIcons[0];
                break;

            case UnlockType.Mage:
                infoText.text = "이제부터\n 마법사를 사용할 수 있습니다!\n(마법사 미개발)";
                heroIcon.sprite = heroIcons[1];
                break;
        }

        if (goTitleBtn != null)
            goTitleBtn.onClick.AddListener(() =>
            {
                unlocktype = UnlockType.None;
                SoundMgr.Instance.PlayBGM("");
                SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("titleScene");
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