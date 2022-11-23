using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemUpgradeManager : MonoBehaviour
{
    int idx = -1;
    bool isEquipItem = false;

    public Text UpgradeInfoText;
    public Text UpgradeResultText;

    public Button UpgradeBtn;
    public Button CancleBtn;
    public Button OkBtn;

    public ParticleSystem upgradingEff;
    public ParticleSystem upgradeFailEff;
    public ParticleSystem upgradeSuccessEff;

    EquipPanelManager equipPanelMgr;

    Item upgradeItem;
    public Image itemImage;

    int upgradePercent = 50;
    int upgradeCost = 0;

    private void OnEnable()
    {
        UpgradeResultText.text = "";
        UpgradeBtn.gameObject.SetActive(true);
        CancleBtn.gameObject.SetActive(true);
        OkBtn.gameObject.SetActive(false);
        itemImage.color = Color.white;
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (UpgradeBtn != null)
            UpgradeBtn.onClick.AddListener(() =>
            {
                if (GlobalData.playerGold < upgradeCost)
                    return;

                equipPanelMgr.StartCoroutine(equipPanelMgr.DecreaseGoldFunc(upgradeCost));

                UpgradeBtn.gameObject.SetActive(false);
                CancleBtn.gameObject.SetActive(false);
                StartCoroutine(UpgradeStartCo());
            });

        if (CancleBtn != null)
            CancleBtn.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });

        if (OkBtn != null)
            OkBtn.onClick.AddListener(() =>
            {
                equipPanelMgr.TotalStatusFunc();
                this.gameObject.SetActive(false);
            });

        equipPanelMgr = FindObjectOfType<EquipPanelManager>();
    }

    public void UpgradeInit(Item item, int itemIdx, bool isEquip)
    {
        idx = itemIdx;
        Debug.Log(idx);
        isEquipItem = isEquip;

        upgradeItem = item;

        itemImage.sprite = item.itemImage;

        upgradePercent = (100 - (upgradeItem.upgradeNum * 10));
        upgradeCost = (200 + (upgradeItem.upgradeNum * 100)) * (int)upgradeItem.rankType;


        UpgradeInfoText.text = item.itemName + "\n" +
                               "강화 확률 : " + upgradePercent + "%\n" +
                               "강화 비용 : " + upgradeCost + "Gold\n" +
                               "강화 하시겠습니까?";
    }

    IEnumerator UpgradeStartCo()
    {
        for(int i = 0; i < 3; i++)
        {
            upgradingEff.Play();
            yield return new WaitForSeconds(0.5f);
        }

        int rand = Random.Range(0, 100);
        if(rand <= upgradePercent)  //강화 성공
        {
            upgradeSuccessEff.Play();
            upgradeItem.upgradeNum++;   //강화 수치 증가

            if(upgradeItem.addedName == "")
                upgradeItem.addedName = upgradeItem.itemName;   //원본 백업

            upgradeItem.itemName = upgradeItem.addedName + " + " + upgradeItem.upgradeNum; //이름 변경

            UpgradeInfoText.text = " 강화 성공!!";
            UpgradeResultText.text = upgradeItem.itemName;
            UpgradeResultText.gameObject.SetActive(true);
            OkBtn.gameObject.SetActive(true);
            StartCoroutine(UpgradeItemStatus());    //능력치 업그레이드 연출
        }
        else
        {
            upgradeFailEff.Play();
            UpgradeResultText.text = "<color=red>강화 실패!!</color>";
            UpgradeInfoText.text = " 장비가 가루가 되었습니다...";
            itemImage.color = new Color32(118, 118, 118, 255);

            if(isEquipItem) //아이템 제거현장
            {
                equipPanelMgr.UnEquipItem(GlobalData.equipItems[idx].itemType);      //아이템 해제

                equipPanelMgr.infoMgr.ClearInfo();
            }
            else
            {
                GlobalData.items.RemoveAt(idx);
                equipPanelMgr.invenMgr.ClearBtn(idx);
            }

            UpgradeResultText.gameObject.SetActive(true);
            OkBtn.gameObject.SetActive(true);
        }

    }

    IEnumerator UpgradeItemStatus()
    {
        string beforetext = "";
        int[] beforeStauts = new int[upgradeItem.addStatus.Length]; // 연출용 변수값
        int[] upgradeStatus = new int[upgradeItem.addStatus.Length]; // 연출용 변수값
        int plusValue = 0;

        for(int i = 0; i < upgradeItem.addStatus.Length; i++)   //미리 정보는 업데이트
        {
            if (upgradeItem.addStatus[i] <= 0)
                continue;

            if(!isEquipItem)    //장비하고 있는 아이템이 아닐때
            {
                beforeStauts[i] = GlobalData.items[idx].addStatus[i];       //업그레이드 전 치수를 백업
                GlobalData.items[idx].addStatus[i] += (int)(upgradeItem.addStatus[i] * 0.2f);   //실제 치수 업그레이드

                upgradeStatus[i] = GlobalData.items[idx].addStatus[i];      //업그레이드 후 치수를 설정
                if (upgradeStatus[i] == GlobalData.items[idx].addStatus[i]) //만약 20%강화에도 능력치 변화가 없는 작은 능력치일시 +1수치 추가
                {
                    upgradeStatus[i]++;
                    GlobalData.items[idx].addStatus[i]++;
                }
            }
            else
            {
                beforeStauts[i] = GlobalData.equipItems[idx].addStatus[i];       //업그레이드 전 치수를 백업
                GlobalData.equipItems[idx].addStatus[i] += (int)(upgradeItem.addStatus[i] * 0.2f);   //실제 치수 업그레이드

                upgradeStatus[i] = GlobalData.equipItems[idx].addStatus[i];      //업그레이드 후 치수를 설정
                if (upgradeStatus[i] == GlobalData.equipItems[idx].addStatus[i]) //만약 20%강화에도 능력치 변화가 없는 작은 능력치일시 +1수치 추가
                {
                    upgradeStatus[i]++;
                    GlobalData.equipItems[idx].addStatus[i]++;
                }
            }
        }

        if (!isEquipItem)
            GlobalData.items[idx].sellPrice += GlobalData.items[idx].UpSellPrice;   //판매가격 증가
        else
            GlobalData.equipItems[idx].sellPrice += GlobalData.equipItems[idx].UpSellPrice;   //판매가격 증가


        equipPanelMgr.invenMgr.RefreshItemBtn(idx, isEquipItem); //버튼의 정보값을 바꿔준다

        for (int i = 0; i < upgradeItem.addStatus.Length; i++)
        {
            if (upgradeItem.addStatus[i] <= 0)
                continue;

            yield return new WaitForSeconds(0.5f);

            UpgradeInfoText.text += "\n" + GlobalData.itemInfoName[i];     // 강화성공 /n 체력
            beforetext = UpgradeInfoText.text;  //강화성공 /n 체력

            for(int j = beforeStauts[i]; j < upgradeStatus[i]; j++) //치수의 절반만큼 수치 업그레이드
            {
                plusValue++;
                beforeStauts[i]++;
                UpgradeInfoText.text = beforetext + " +" + beforeStauts[i].ToString();
                yield return new WaitForSeconds(0.05f);
            }

            UpgradeInfoText.text += "<color=green> (+ " + plusValue.ToString() + ")</color>";
            plusValue = 0;
        }
    }
}