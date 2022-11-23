using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelManager : MonoBehaviour
{
    public Text userGoldText;
    public Text[] beforeTexts;
    public Text[] upgradesTexts;

    public Button[] upgradeBtns;
    public Button cancleBtn;

    private void OnEnable()
    {
        userGoldText.text = "Gold : " + GlobalData.playerGold;
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        RefreshTexts();

        if(upgradeBtns.Length > 0)
        {
            for(int i = 0; i < upgradeBtns.Length; i++)
            {
                int idx = i;
                upgradeBtns[idx].onClick.AddListener(() =>
                {
                    if (GlobalData.playerGold < GlobalData.UpgradePrices[idx])
                        return;

                    StartCoroutine(DecreaseGoldFunc(GlobalData.UpgradePrices[idx]));
                    GlobalData.beforeStatus[idx] += GlobalData.UpgradeStatus[idx];
                    GlobalData.UpgradePrices[idx] += 100;
                    RefreshTexts();
                });
            }
        }
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    void RefreshTexts()
    {
        //userGoldText.text = "º¸À¯°ñµå : " + GlobalData.playerGold;

        for (int i = 0; i < beforeTexts.Length; i++)
        {
            beforeTexts[i].text = GlobalData.beforeStatus[i].ToString();
            upgradesTexts[i].text = GlobalData.UpgradePrices[i].ToString();
        }
    }

    public IEnumerator DecreaseGoldFunc(int val)
    {
        GlobalData.playerGold -= val;
        for (int i = 0; i < val; i++)
        {
            GlobalData.playerCurGold--;
            userGoldText.text = "Gold  : " + GlobalData.playerCurGold.ToString();

            yield return new WaitForEndOfFrame();
        }
    }
}