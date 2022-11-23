using UnityEngine;
using UnityEngine.UI;

public class MonStatusMgr : MonoBehaviour
{
    public Image hpBarImg;
    public Text hpText;
    public Text MonNameText;
    public Text LvText;

    string monName;
    int maxHp;
    public int curHp;
    int monlv;

    private void Start() => StartFunc();

    private void StartFunc()
    {

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {

    }

    public void SetStatus(string name, int hp, int lv)
    {
        monName = name;
        maxHp = hp;
        curHp = maxHp;
        monlv = lv;

        hpText.text = curHp.ToString() + " / " + maxHp.ToString();
        LvText.text = "Lv. " + monlv.ToString();
        MonNameText.text = monName;
    }

    public void SetHp(int dam)
    {
        if (dam <= 2)
            SoundMgr.Instance.PlayEffSound("LowDamage");
        else if(3 <= dam && dam <= 5)
            SoundMgr.Instance.PlayEffSound("MediumDamage");
        else if(6 <= dam)
            SoundMgr.Instance.PlayEffSound("HugeDamage");

        curHp -= dam;

        if (curHp <= 0)
            curHp = 0;

        hpText.text = curHp.ToString() + " / " + maxHp.ToString();
        hpBarImg.fillAmount = (float)curHp / maxHp;
    }

    public void PoisonEff(int PoisonCount)
    {
        curHp -= PoisonCount;
        

        if (curHp <= 0)
            curHp = 0;

        hpText.text = curHp.ToString() + " / " + maxHp.ToString();
        hpBarImg.fillAmount = (float)curHp / maxHp;
    }
}