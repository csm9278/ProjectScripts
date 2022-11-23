using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatueMgr : MonoBehaviour
{
    public BattleManager _battlemgr;

    //히어로 스테이터스 관리
    public Image hpBarImg;
    public Text hpText;
    public Text heroNameText;
    public Text goldText;
    public Text LvText;

    public Dictionary<string, int> debuffDic = new Dictionary<string, int>();

    private void Start() => StartFunc();

    private void StartFunc()
    {

        HeroStatusInit();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }


    public void HeroStatusInit()
    {
        heroNameText.text = GlobalData.heroName;
        hpText.text = GlobalData.hero.curHp.ToString() + " / " + GlobalData.hero.maxHp.ToString();
        hpBarImg.fillAmount = (float)GlobalData.hero.curHp / GlobalData.hero.maxHp;

        goldText.text = "x " + GlobalData.hero.gold.ToString();
        LvText.text = "Lv. " + GlobalData.hero.lv.ToString();
    }

    public void RefreshStatus()
    {

    }

    public void SetHp(int dam)
    {
        if (dam <= 2)
            SoundMgr.Instance.PlayEffSound("LowDamage");
        else if (3 <= dam && dam <= 5)
            SoundMgr.Instance.PlayEffSound("MediumDamage");
        else if (6 <= dam)
            SoundMgr.Instance.PlayEffSound("HugeDamage");

        GlobalData.hero.curHp -= dam;

        if (GlobalData.hero.curHp <= 0)
            GlobalData.hero.curHp = 0;

        hpText.text = GlobalData.hero.curHp.ToString() + " / " + GlobalData.hero.maxHp.ToString();
        hpBarImg.fillAmount = (float)GlobalData.hero.curHp / GlobalData.hero.maxHp;
    }

    public void AddDebuff(string debuffName, int debuffCount, bool isHero = false)
    {
        if (debuffDic.ContainsKey(debuffName) && debuffDic[debuffName] > 0)
        {
            debuffDic[debuffName] += debuffCount;

            if (debuffName == "독")
                _battlemgr.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Poison, debuffCount, isHero);
            else if (debuffName == "약화")
                _battlemgr.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Weak, debuffCount, isHero);
            else if (debuffName == "화염")
                _battlemgr.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Fire, debuffCount, isHero);
        }
        else
        {
            debuffDic.Add(debuffName, debuffCount);
            if (debuffName == "독")
                _battlemgr.debuffNodeFunc(DebuffNodeMgr.DebuffType.Poison, debuffCount, isHero);
            else if (debuffName == "약화")
                _battlemgr.debuffNodeFunc(DebuffNodeMgr.DebuffType.Weak, debuffCount, isHero);
            else if (debuffName == "화염")
                _battlemgr.debuffNodeFunc(DebuffNodeMgr.DebuffType.Fire, debuffCount, isHero);
        }

        if (debuffName == "독")
            SoundMgr.Instance.PlayEffSound("applypoison", 0.4f);
        else if (debuffName == "약화")
            SoundMgr.Instance.PlayEffSound("applyweaken", 0.4f);
        else if (debuffName == "화염")
            SoundMgr.Instance.PlayEffSound("fire", 0.5f);

        Debug.Log(debuffName + ": " + debuffDic[debuffName]);
    }

    public void DebuffEff()
    {
        if (debuffDic.ContainsKey("독"))
        {
            PoisonEff(debuffDic["독"]);
            //Debug.Log(debuffDic["독"] + " 만큼 독 데미지");
            _battlemgr.AddStateText("<color=darkblue>" + debuffDic["독"].ToString() + "독 데미지를 입었다.</color>");
            debuffDic["독"]--;
            //Debug.Log(debuffDic["독"] + " 현재 독 카운트");
            SoundMgr.Instance.PlayEffSound("poison", 0.4f);

            if (debuffDic["독"] <= 0)
                debuffDic.Remove("독");
        }
    }

    public void DecreaseDebuff(string debuffName)
    {
        if(debuffDic.ContainsKey(debuffName))
        {
            debuffDic[debuffName]--;

            if (debuffDic[debuffName] <= 0)
                debuffDic.Remove(debuffName);
        }
    }

    public void PoisonEff(int PoisonCount)
    {
        Debug.Log("독 뎀 계산 전 : " + GlobalData.hero.curHp.ToString() + " - " + PoisonCount);

        GlobalData.hero.curHp -= PoisonCount;
        Debug.Log("히어로 독뎀 후 : " + GlobalData.hero.curHp.ToString());

        if (GlobalData.hero.curHp <= 0)
            GlobalData.hero.curHp = 0;

        hpText.text = GlobalData.hero.curHp.ToString() + " / " + GlobalData.hero.maxHp.ToString();
        hpBarImg.fillAmount = (float)GlobalData.hero.curHp / GlobalData.hero.maxHp;
    }
}