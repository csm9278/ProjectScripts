using UnityEngine;
using UnityEngine.UI;

public class itemInfoPanelManager : MonoBehaviour
{
    public Text itemNameText;
    public Text infoText;
    public Button equipBtn;
    public Button unEquipBtn;
    public Button sellBtn;
    public Text sellCostText;
    public Image ItemImage;
    public int listIdx = -1;
    bool isEquipItem = false;
    InventoryManager invenMgr;
    EquipPanelManager equipMgr;

    //강화하기 변수
    [Header("---Upgrade Item ---")]
    public Button UpgradeBtn;
    public GameObject upgradePanel;
    public ItemUpgradeManager itemUpgradeMgr;



    private void Start() => StartFunc();

    private void StartFunc()
    {
        invenMgr = FindObjectOfType<InventoryManager>();
        equipMgr = FindObjectOfType<EquipPanelManager>();
        itemUpgradeMgr = upgradePanel.GetComponent<ItemUpgradeManager>();

        if (equipBtn != null)
            equipBtn.onClick.AddListener(equipItemFunc);

        if (unEquipBtn != null)
            unEquipBtn.onClick.AddListener(UnEquipItemFunc);

        if (sellBtn != null)
            sellBtn.onClick.AddListener(SellItemFunc);

        if (UpgradeBtn != null)
            UpgradeBtn.onClick.AddListener(UpgradeItemFunc);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    /// <summary>
    /// 장비 장착 버튼 함수
    /// </summary>
    public void equipItemFunc()
    {
        if (listIdx < 0)
            return;

        if (GlobalData.equipItems[(int)GlobalData.items[listIdx].itemType] != null) //넘어온 아이템 타입의 아이템이 이미 장비중이라면..
            invenMgr.AddItenFunc(GlobalData.equipItems[(int)GlobalData.items[listIdx].itemType]);

        GlobalData.equipItems[(int)GlobalData.items[listIdx].itemType] = GlobalData.items[listIdx]; //아이템을 장착
        equipMgr.EquipItems(GlobalData.items[listIdx].itemType);

        GlobalData.items.RemoveAt(listIdx);
        invenMgr.ClearBtn(listIdx);
        invenMgr.RefreshItems();

        ClearInfo();
    }

    /// <summary>
    /// 장비해제 버튼 함수
    /// </summary>
    public void UnEquipItemFunc()
    {
        if (listIdx < 0)
            return;

        invenMgr.AddItenFunc(GlobalData.equipItems[listIdx]);
        equipMgr.UnEquipItem(GlobalData.equipItems[listIdx].itemType);

        ClearInfo();
    }
    
    /// <summary>
    /// 아이템 판매 버튼 함수
    /// </summary>
    public void SellItemFunc()
    {
        if(isEquipItem)
        {
            StartCoroutine(equipMgr.AddGoldFunc(GlobalData.equipItems[listIdx].sellPrice));   //가격만큼 돈 추가
            equipMgr.UnEquipItem(GlobalData.equipItems[listIdx].itemType);      //아이템 해제

            ClearInfo();
        }
        else
        {
            //GlobalData.playerGold += GlobalData.items[listIdx].sellPrice;
            StartCoroutine(equipMgr.AddGoldFunc(GlobalData.items[listIdx].sellPrice));
            GlobalData.items.RemoveAt(listIdx);
            invenMgr.ClearBtn(listIdx);
            invenMgr.RefreshItems();
            //equipMgr.goldText.text = "Gold : " + GlobalData.playerGold.ToString();

            ClearInfo();
        }
    }

    /// <summary>
    /// 아이템 업그레이드 버튼 함수
    /// </summary>
    void UpgradeItemFunc()
    {
        upgradePanel.gameObject.SetActive(true);

        if (isEquipItem)
            itemUpgradeMgr.UpgradeInit(GlobalData.equipItems[listIdx], listIdx, isEquipItem);
        else
            itemUpgradeMgr.UpgradeInit(GlobalData.items[listIdx], listIdx, isEquipItem);

        ClearInfo();
    }

    /// <summary>
    /// 인포매니저의 정보값 초기화
    /// </summary>
    public void ClearInfo()
    {
        itemNameText.text = "";
        infoText.text = "";
        ItemImage.sprite = Resources.Load<Sprite>("ItemImage/NoneItem");
        listIdx = -1;

        sellCostText.text = "";
        equipBtn.gameObject.SetActive(false);
        unEquipBtn.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        UpgradeBtn.gameObject.SetActive(false);
    }

    public void SetInfo(Item item, int idx, bool isEquipItem = false)
    {
        itemNameText.text = item.itemName;
        infoText.text = item.infoString;
        ItemImage.sprite = item.itemImage;
        sellCostText.text = "판매가격 : " + item.sellPrice;
        listIdx = idx;
        this.isEquipItem = isEquipItem;

        changeEquipBtns(isEquipItem);
    }

    /// <summary>
    /// 장착하기 또는 해제하기 띄우기
    /// </summary>
    /// <param name="isEquipItem"></param>
    void changeEquipBtns(bool isEquipItem)
    {
        if (isEquipItem)
        {
            equipBtn.gameObject.SetActive(false);
            unEquipBtn.gameObject.SetActive(true);
            UpgradeBtn.gameObject.SetActive(true);
        }
        else
        {
            equipBtn.gameObject.SetActive(true);
            unEquipBtn.gameObject.SetActive(false);
            UpgradeBtn.gameObject.SetActive(true);
        }

        sellBtn.gameObject.SetActive(true);
    }
}