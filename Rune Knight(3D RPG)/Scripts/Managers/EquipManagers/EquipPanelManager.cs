using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EquipPanelManager : MonoBehaviour
{
    public Button[] equipBtns;
    public Button addItemBtn;
    public Button itemCombineBtn;
    public Button AddGoldBtn;
    public GameObject itemCombinePanel;

    ParticleSystem[] equipParticles;

    public Text totalStatusText;
    public Text goldText;

    [HideInInspector] public itemInfoPanelManager infoMgr;
    [HideInInspector] public InventoryManager invenMgr;
    [HideInInspector] public CombinePanelManager combineMgr;

    WaitForEndOfFrame endFrame = new WaitForEndOfFrame();

    private void Awake()
    {
        invenMgr = FindObjectOfType<InventoryManager>();
        infoMgr = FindObjectOfType<itemInfoPanelManager>();
        combineMgr = FindObjectOfType<CombinePanelManager>();

    }

    private void OnEnable()
    {
        goldText.text = "Gold : " +  GlobalData.playerGold.ToString();
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (addItemBtn != null)
            addItemBtn.onClick.AddListener(() =>
            {
                if (GlobalData.playerGold < 500)
                    return;

                StartCoroutine(DecreaseGoldFunc(500));

                Item item = new Item();

                invenMgr.AddItenFunc(item);
            });

        if (itemCombineBtn != null)
            itemCombineBtn.onClick.AddListener(() =>
            {
                ActiveBtns(false);
                itemCombinePanel.gameObject.SetActive(true);
                
            });

        if (AddGoldBtn != null)
            AddGoldBtn.onClick.AddListener(() =>
            {
                StartCoroutine(AddGoldFunc(1000));
            });

        if (equipBtns != null)
            equipParticles = new ParticleSystem[equipBtns.Length];

        for(int i = 0; i < equipBtns.Length; i++)
        {
            int idx = i;
            equipParticles[idx] = equipBtns[idx].GetComponentInChildren<ParticleSystem>();

            equipBtns[idx].onClick.AddListener(() =>
            {
                if (GlobalData.equipItems[idx] == null)
                    return;

                infoMgr.SetInfo(GlobalData.equipItems[idx], idx, true);
            });
        }

        InitEquipItems();

    }

    //private void Update() => UpdateFunc();

    //private void UpdateFunc()
    //{
        
    //}

    public void ActiveBtns(bool isfalse)
    {
        addItemBtn.gameObject.SetActive(isfalse);
        itemCombineBtn.gameObject.SetActive(isfalse);
    }

    void InitEquipItems()
    {
        for(int i = 0; i < GlobalData.equipItems.Length;i++)
        {
            if (GlobalData.equipItems[i] == null)
                continue;

            equipBtns[i].GetComponentsInChildren<Image>()[1].sprite = GlobalData.equipItems[i].itemImage;

        }

        TotalStatusFunc();
    }

    public void EquipItems(Item.ItemType type)
    {
        equipBtns[(int)type].GetComponentsInChildren<Image>()[1].sprite = GlobalData.equipItems[(int)type].itemImage;
        equipParticles[(int)type].Play();

        TotalStatusFunc();
    }

    public void UnEquipItem(Item.ItemType type)
    {
        equipBtns[(int)type].GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("ItemImage/NoneItem");
        GlobalData.equipItems[(int)type] = null;    //장착해제

        TotalStatusFunc();
    }

    public void TotalStatusFunc()
    {
        for (int i = 0; i < GlobalData.itemStatus.Length; i++)   //일단 모든 장비 능력치 초기화
        {
            GlobalData.itemStatus[i] = 0;
        }

        for (int i = 0; i < equipBtns.Length; i++)
        {
            if (GlobalData.equipItems[i] == null)
            {
                continue;
            }

            for (int j = 0; j < GlobalData.itemStatus.Length; j++)
            {
                GlobalData.itemStatus[j] += GlobalData.equipItems[i].addStatus[j];  //총 아이템 능력치에 각아이템의 능력치에 접근해 더해줌
            }
        }

        totalStatusText.text = "";

        for(int i = 0; i < GlobalData.itemStatus.Length; i++)
        {
            if (GlobalData.itemStatus[i] <= 0)
                continue;

            totalStatusText.text += GlobalData.itemInfoName[i] + " +" + GlobalData.itemStatus[i].ToString() + "\n";
        }
    }

    public IEnumerator AddGoldFunc(int  val)
    {
        GlobalData.playerGold += val;

        for (int i  = 0; i < val; i++)
        {
            GlobalData.playerCurGold++;
            goldText.text = "Gold  : " + GlobalData.playerCurGold.ToString();

            yield return endFrame;
        }

    }


    public IEnumerator DecreaseGoldFunc(int val)
    {
        GlobalData.playerGold -= val;

        for (int i = 0; i < val; i++)
        {
            GlobalData.playerCurGold--;
            goldText.text = "Gold  : " + GlobalData.playerCurGold.ToString();

            yield return endFrame;
        }
    }
}