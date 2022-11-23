using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombinePanelManager : MonoBehaviour
{
    //아이템 설정
    [Header("--- Item Select---")]
    public Button FirstItem;
    public Button SecondItem;
    Image firstImage;
    Image SecondImage;
    public Text combineInfoText;
    public Text plusText;
    public Text firstText;
    public Text secondText;
    public Text ResultText;
    ParticleSystem[] changeParticles = new ParticleSystem[2];

    int nowIdx;
    public int[] itemIdxs = new int[2];
    Item[] nowItems = new Item[2];

    [Header("--- Btns ---")]
    public Button combineBtn;
    public Button cancleBtn;
    public Button OkBtn;

    //연출용 이미지들
    [Header("--- Eff Images ---")]
    public Image[] effImages;
    public ParticleSystem combineParticle;
    public GameObject weaponResult;
    public ParticleSystem combineEndEff;

    [HideInInspector] public EquipPanelManager equipMgr;

    [HideInInspector] public bool isCombining = false;
    public bool isReady = true;

    int combineCost = 0;

    private void Awake()
    {

    }

    private void OnEnable() 
    {
        isCombining = true; //조합모드 활성화 인벤에서 클릭시 조합창으로 넘어옴
        nowIdx = 0;
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {

        firstImage = FirstItem.GetComponentsInChildren<Image>()[1];
        SecondImage = SecondItem.GetComponentsInChildren<Image>()[1];
        changeParticles[0] = FirstItem.GetComponentInChildren<ParticleSystem>();
        changeParticles[1] = SecondItem.GetComponentInChildren<ParticleSystem>();
        equipMgr = FindObjectOfType<EquipPanelManager>();
        equipMgr.invenMgr.combineManager = this;
        isCombining = false;
        this.gameObject.SetActive(false);

        if (FirstItem != null)
            FirstItem.onClick.AddListener(() =>
            {
                if(nowItems[0] != null)
                {
                    ReturnItem(0);
                    if(nowItems[1] != null)
                        ReturnItem(1);
                    equipMgr.invenMgr.SortList(Item.ItemType.None); //조합 창이 켜지면 전체 정렬한번

                    nowIdx = 0; //첫번째 부터 등록

                    firstText.text = "";
                    secondText.text = "";
                }
                combineBtn.gameObject.SetActive(false);
                equipMgr.invenMgr.SortBtnActive(true);
            });


        if (SecondItem != null)
            SecondItem.onClick.AddListener(() =>
            {
                if(nowItems[1] != null)
                {
                    ReturnItem(1);  //2번째 등록된 아이템의 정보를 인벤토리에 돌려줌
                    equipMgr.invenMgr.SortList(nowItems[0].itemType, (int)nowItems[0].rankType);    //1번째 장착된 아이템들로 정렬
                    secondText.text = "";
                }
                combineBtn.gameObject.SetActive(false);
                combineInfoText.gameObject.SetActive(false);
            });

        if (combineBtn != null)
            combineBtn.onClick.AddListener(() =>
            {
                if (GlobalData.playerGold < combineCost)
                    return;

                if (nowItems[0] == null || nowItems[1] == null)
                    return;

                StartCoroutine(equipMgr.DecreaseGoldFunc(combineCost));

                FirstItem.gameObject.SetActive(false);
                SecondItem.gameObject.SetActive(false);
                for(int i = 0; i < effImages.Length; i++)
                {
                    effImages[i].gameObject.SetActive(true);
                    effImages[i].GetComponentsInChildren<Image>()[1].sprite = nowItems[i].itemImage;
                }

                combineParticle.Play();

                plusText.gameObject.SetActive(false);
                combineBtn.gameObject.SetActive(false);

                //조합중일때 인벤 버튼 누르지 못하게
                isReady = false;

                StartCoroutine(ItemCombineCo());
            });

        if (cancleBtn != null)
            cancleBtn.onClick.AddListener(() =>
            {
                if (!isReady)
                    return;

                if (nowItems[0] != null)
                    ReturnItem(0);

                if (nowItems[1] != null)
                    ReturnItem(1);

                equipMgr.invenMgr.SortList(Item.ItemType.None);

                isCombining = false;
                this.gameObject.SetActive(false);
                equipMgr.ActiveBtns(true);
            });

        if (OkBtn != null)
            OkBtn.onClick.AddListener(ResetFunc);
    }

    void ChangeInfoText(int rankNum)
    {
        switch(rankNum)
        {
            case 1:
                combineInfoText.text = "조합 성공 확률 : 80% \n조합 비용 : 200";
                combineCost = 200;
                break;

            case 2:
                combineInfoText.text = "조합 성공 확률 : 50% \n조합 비용 : 500";
                combineCost = 500;
                break;

            case 3:
                combineInfoText.text = "조합 성공 확률 : 20% \n조합 비용 : 1000";
                combineCost = 1000;
                break;
        }
    }

    /// <summary>
    /// 다른거 꼈을때 바로 되돌리기
    /// </summary>
    public void ReturnItem(int returnidx)
    {
        equipMgr.invenMgr.AddItenFunc(nowItems[returnidx]);
        nowItems[returnidx] = null;

        if (returnidx == 0)
            firstImage.sprite = GlobalData.nonImage;
        else
            SecondImage.sprite = GlobalData.nonImage;
    }

    public void SetCombineInfo(Item item, int idx)
    {
        if(nowIdx == 0) //첫번째 조합 아이템 등록 상황
        {
            if(nowItems[0] != null)
                ReturnItem(0);
            equipMgr.invenMgr.SortBtnActive(false);

            firstImage.sprite = item.itemImage;
            nowItems[0] = item;
            itemIdxs[1] = item.WeaponIdx;
            firstText.text = item.itemName + "\n" + item.infoString;


            GlobalData.items.RemoveAt(idx);
            equipMgr.invenMgr.ClearBtn(idx);
            equipMgr.invenMgr.SortList(item.itemType, (int)item.rankType);
            nowIdx = 1;
            changeParticles[0].Play();
        }
        else    //두번째 조합 아이템 등록 상황
        {
            if (nowItems[1] != null)
                ReturnItem(1);


            SecondImage.sprite = item.itemImage;
            nowItems[1] = item;
            itemIdxs[1] = item.WeaponIdx;
            secondText.text = item.itemName + "\n" +  item.infoString;

            GlobalData.items.RemoveAt(idx);
            equipMgr.invenMgr.ClearBtn(idx);
            changeParticles[1].Play();

            combineBtn.gameObject.SetActive(true);

            combineInfoText.gameObject.SetActive(true);
            ChangeInfoText((int)nowItems[0].rankType);
        }
    }

    /// <summary>
    /// 아이템 조합 후 연출용 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator ItemCombineCo()
    {
        Item combineitem = new Item(true, nowItems[0]);

        yield return new WaitForSeconds(2.5f);

        firstText.gameObject.SetActive(false);
        secondText.gameObject.SetActive(false);

        ResultText.gameObject.SetActive(true);
        ResultText.text = combineitem.itemName + "\n" + combineitem.infoString;

        for (int i = 0; i < effImages.Length; i++)
        {
            effImages[i].gameObject.SetActive(false);
        }

        //조합 재료 아이템 파괴
        for(int i = 0; i < itemIdxs.Length; i++)
        {
            GlobalData.items.Remove(nowItems[i]);
        }

        //조합 아이템 추가
        equipMgr.invenMgr.AddItenFunc(combineitem);

        nowItems[0] = null;
        nowItems[1] = null;

        OkBtn.gameObject.SetActive(true);
        weaponResult.SetActive(true);
        weaponResult.GetComponentsInChildren<Image>()[1].sprite = combineitem.itemImage;
        combineEndEff.Play();
    }

    void ResetFunc()
    {
        plusText.gameObject.SetActive(true);

        cancleBtn.gameObject.SetActive(true);
        FirstItem.gameObject.SetActive(true);
        SecondItem.gameObject.SetActive(true);
        firstImage.sprite = GlobalData.nonImage;
        SecondImage.sprite = GlobalData.nonImage;

        firstText.gameObject.SetActive(true);
        secondText.gameObject.SetActive(true);
        firstText.text = "";
        secondText.text = "";


        equipMgr.invenMgr.SortList(Item.ItemType.None);
        combineInfoText.gameObject.SetActive(false);

        weaponResult.gameObject.SetActive(false);
        OkBtn.gameObject.SetActive(false);
        ResultText.gameObject.SetActive(false);

        cancleBtn.gameObject.SetActive(true);
        itemIdxs[0] = -1;
        itemIdxs[1] = -1;
        nowIdx = 0;
        isReady = true;
        equipMgr.invenMgr.SortBtnActive(true);
    }
}