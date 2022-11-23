using UnityEngine;
using UnityEngine.UI;

public class InGameConfig : MonoBehaviour
{

    public Button BackBtn;
    public Button SoundConfigBtn;
    public Button GoTitleBtn;

    public GameObject SceneChangerIn;
    public GameObject soundConfigBox;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });

        if (SoundConfigBtn != null)
            SoundConfigBtn.onClick.AddListener(() =>
            {
                soundConfigBox.SetActive(true);
            });

        if (GoTitleBtn != null)
            GoTitleBtn.onClick.AddListener(() =>
            {
                SoundMgr.Instance.PlayBGM("");
                SoundMgr.Instance.PlayEffSound("NextScene");
                GlobalData.Reset();
                SceneChangerIn.GetComponent<SceneChanger>().ChangeStart("TitleScene");
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}