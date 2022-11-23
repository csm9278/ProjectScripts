using UnityEngine;
using UnityEngine.UI;

public enum MapKind
{
    Battle,
    Chest,
    Shop,
    Heal,
    BossBattle,
    maxCount
}

public class StageManager : MonoBehaviour
{
    public GameObject sceneChangerIn;
    public GameObject sceneChangerOut;
    public GameObject stageBtn;
    public GameObject canvas;
    public GameObject pausePanel;
    public Button pauseBtn;

    public Button leftBtn;
    public Button rightBtn;
    public Button bossBtn;

    public Text floorText;

    HeroStatueMgr heroStatusmgr;

    float soundDelay = -0.5f;
    public bool isPick = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {

        heroStatusmgr = FindObjectOfType<HeroStatueMgr>();

        floorText.text = "진행도 : " + GlobalData.stage1.floor.ToString() + " / " + GlobalData.stage1.Maxfloor.ToString();

        MakeBtn();
        sceneChangerOut.GetComponent<SceneChanger>().ChangeStart();
        soundDelay = 1.0f;

        if(leftBtn != null)
            leftBtn.onClick.AddListener(() =>
            {
                if (isPick)
                    return;

                isPick = true;
                leftBtn.GetComponent<StageBtnFunc>().BtnFunc();
            });

        if (rightBtn != null)
            rightBtn.onClick.AddListener(() =>
            {
                if (isPick)
                    return;

                isPick = true;
                rightBtn.GetComponent<StageBtnFunc>().BtnFunc();
            });

        if (pauseBtn != null)
            pauseBtn.onClick.AddListener(() =>
            {
                pausePanel.SetActive(true);
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
                SoundMgr.Instance.PlayBGM("StageSound");
        }
    }

    void MakeBtn()
    {
        if (GlobalData.stage1.floor >= GlobalData.stage1.Maxfloor)
        {
            GameObject btn = Instantiate(stageBtn, canvas.transform);
            btn.transform.localPosition = new Vector3(0.0f, 0, 0);
            btn.GetComponent<StageBtnFunc>()._stageMgr = this;
            leftBtn = btn.GetComponent<Button>();
            return;
        }

        if (GlobalData.stage1.floor + 1 == GlobalData.stage1.Maxfloor)
        {
            GameObject btn = Instantiate(stageBtn, canvas.transform);
            btn.transform.localPosition = new Vector3(0.0f, 0, 0);
            btn.GetComponent<StageBtnFunc>()._stageMgr = this;
            leftBtn = btn.GetComponent<Button>();
            return;
        }


        for(int i = 0; i < 2; i++)
        {
            GameObject btn = Instantiate(stageBtn, canvas.transform);
            btn.transform.localPosition = new Vector3(-250.0f + (500.0f * i), 0, 0);
            btn.GetComponent<StageBtnFunc>()._stageMgr = this;
            if (i == 0)
                leftBtn = btn.GetComponent<Button>();
            else
                rightBtn = btn.GetComponent<Button>();
        }
    }

    public void ChangeBtn()
    {
        heroStatusmgr.HeroStatusInit();
        floorText.text = "진행도 : " + GlobalData.stage1.floor.ToString() + " / " + GlobalData.stage1.Maxfloor.ToString();


        if (GlobalData.stage1.floor >= GlobalData.stage1.Maxfloor)
        {
            leftBtn.GetComponent<StageBtnFunc>().ChangeBtnFunc();
            leftBtn.transform.localPosition = new Vector3(0, 0, 0);
            if(rightBtn != null)
            rightBtn.gameObject.SetActive(false);
            return;
        }

        if (GlobalData.stage1.floor + 1 == GlobalData.stage1.Maxfloor)
        {
            leftBtn.GetComponent<StageBtnFunc>().ChangeBtnFunc();
            leftBtn.transform.localPosition = new Vector3(0, 0, 0);
            if(rightBtn != null)
            rightBtn.gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
                leftBtn.GetComponent<StageBtnFunc>().ChangeBtnFunc();
            else
                rightBtn.GetComponent<StageBtnFunc>().ChangeBtnFunc();
        }

        isPick = false;
    }

}