using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Button BackBtn;

    public GameObject sceneChangerIn;
    public GameObject sceneChangerOut;

    float soundDelay = -0.1f;

    [Header("--- ItemNode ---")]
    public Transform mainCanvas;
    public GameObject itemBtnPrefab;
    public Button[] itemBtns;

    HeroStatueMgr mgr;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        soundDelay = 1.0f;
        sceneChangerOut.GetComponent<SceneChanger>().ChangeStart();

        mgr = GameObject.FindObjectOfType<HeroStatueMgr>();
        mgr.HeroStatusInit();

        BackBtn.onClick.AddListener(() =>
        {
            sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("StageScene");
        });

        itemBtns = new Button[3];

        for(int i = 0; i < 3; i++)
        {
            int idx = i;

            GameObject obj = Instantiate(itemBtnPrefab, mainCanvas, true);
            obj.transform.localPosition = new Vector2(-300 + (i * 300), 50);
            obj.GetComponent<ShopItemRootMgr>().InitItem(idx);
            itemBtns[i] = obj.GetComponent<Button>();

            itemBtns[idx].onClick.AddListener(() =>
            {
                itemBtns[idx].GetComponent<ShopItemRootMgr>().ItemEff();
                RefreshBtns();
            });
        }
        RefreshBtns();

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
                SoundMgr.Instance.PlayBGM("ShopSound", 0.2f);
        }
    }

    public void RefreshBtns()
    {
        mgr.HeroStatusInit();

        Debug.Log("ÃÊ±âÈ­");
        for(int i = 0; i < 3; i++)
        {
            if (itemBtns[i].GetComponent<ShopItemRootMgr>().itemCost > GlobalData.hero.gold)
            {
                itemBtns[i].interactable = false;
            }
            else
                itemBtns[i].interactable = true;
        }
    }    
}