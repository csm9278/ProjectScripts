using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarManager : MonoBehaviour
{
    public Image expGage;
    public Text infoText;
    public Text levelupText;

    public string[] InfoText = { "최대체력이 2 올랐습니다.\n행동력이 1 올랐습니다.", "최대체력이 2 올랐습니다." };
    float getOneExp;

    WaitForSeconds wait0_3f = new WaitForSeconds(0.3f);
    private void Start() => StartFunc();

    private void StartFunc()
    {
        levelupText.text = "";
        infoText.text = "";
        expGage.fillAmount = (float)GlobalData.hero.Exp / GlobalData.maxExp[GlobalData.hero.lv - 1];

        getOneExp = 1.0f / GlobalData.maxExp[GlobalData.hero.lv - 1];
        Debug.Log(getOneExp);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public IEnumerator GetExpCo(int getExp)
    {
        GlobalData.hero.gold++;

        yield return new WaitForSeconds(1.0f);


        for (int i = 0; i < getExp; i++)
        {
            expGage.fillAmount += getOneExp;
            SoundMgr.Instance.PlayEffSound("gainxp", 0.3f);

            if (expGage.fillAmount >= 0.95f)
            {
                infoText.text = InfoText[(GlobalData.hero.lv - 1) % 2];

                expGage.fillAmount = 0.0f;
                if(GlobalData.hero.lv - 1 >= 4)
                {
                    levelupText.text = "최대 레벨 달성 !!";
                }
                else
                {
                    levelupText.text = "레벨 업 !!";
                    getOneExp = 1.0f / GlobalData.maxExp[GlobalData.hero.lv - 1];
                }

                SoundMgr.Instance.PlayEffSound("levelup");
                GlobalData.hero.GetExp(getExp);
                GlobalData.isLevelUp = true;

                yield return new WaitForSeconds(1.0f);

                yield break;
            }

            yield return wait0_3f;

        }
        GlobalData.hero.GetExp(getExp);
    }
}