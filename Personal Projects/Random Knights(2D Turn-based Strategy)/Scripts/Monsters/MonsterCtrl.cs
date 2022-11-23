using UnityEngine;
using System;
using System.Collections.Generic;

public class MonsterCtrl : MonoBehaviour
{

    public MonsterSkillData.MonsterSKill MonSkillEff;

    public MonData mondata;
    GameObject status;
    public MonStatusMgr stmgr;
    public BattleManager battlemanager;

    public Dictionary<string, int> debuffDic = new Dictionary<string, int>();

    private void Start() => StartFunc();

    private void StartFunc()
    {
        Debug.Log(mondata.monName + " " + mondata.MaxHp + " : " + mondata.Lv);
        status = GameObject.Find("MonsterStatus");

        if(MonsterSkillData.MonSkillList.ContainsKey(mondata.monName))
        {
            Debug.Log("키있음");
            MonSkillEff = MonsterSkillData.MonSkillList[mondata.monName];
        }
        else
        {
            Debug.Log("키없음");
        }

        if (status != null)
            stmgr = status.GetComponent<MonStatusMgr>();

        if (stmgr != null)
            stmgr.SetStatus(mondata.monName, mondata.MaxHp, mondata.Lv);

        if (battlemanager == null)
            battlemanager = GameObject.FindObjectOfType<BattleManager>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public void TakeDamage(int dam)
    {
        stmgr.SetHp(dam);
    }

    public void AddDebuff(string debuffName, int debuffCount)
    {
        if (debuffDic.ContainsKey(debuffName) && debuffDic[debuffName] > 0)
        {
            debuffDic[debuffName] += debuffCount;

            if (debuffName == "독")
                battlemanager.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Poison, debuffCount);
            else if (debuffName == "약화")
                battlemanager.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Weak, debuffCount);
            else if (debuffName == "화염")
                battlemanager.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Fire, debuffCount);
        }
        else
        {
            debuffDic.Add(debuffName, debuffCount);
            if (debuffName == "독")
                battlemanager.debuffNodeFunc(DebuffNodeMgr.DebuffType.Poison, debuffCount);
            else if (debuffName == "약화")
                battlemanager.debuffNodeFunc(DebuffNodeMgr.DebuffType.Weak, debuffCount);
            else if (debuffName == "화염")
                battlemanager.debuffNodeFunc(DebuffNodeMgr.DebuffType.Fire, debuffCount);

        }

        if (debuffName == "독")
            SoundMgr.Instance.PlayEffSound("applypoison", 0.4f);
        else if (debuffName == "약화")
            SoundMgr.Instance.PlayEffSound("applyweaken", 0.4f);
        else if (debuffName == "화염")
            SoundMgr.Instance.PlayEffSound("fire", 0.4f);

        //Debug.Log(debuffName + ": " + debuffDic[debuffName]);
    }

    public void DebuffEff()
    {
        if(debuffDic.ContainsKey("독"))
        {
            stmgr.PoisonEff(debuffDic["독"]);
            battlemanager.AddStateText("<color=darkblue>적이 " + debuffDic["독"].ToString() + "독 데미지를 입었다.</color>");
            //Debug.Log(debuffDic["독"] + " 만큼 독 데미지");
            debuffDic["독"]--;
            //Debug.Log(debuffDic["독"] + " 현재 독 카운트");
            SoundMgr.Instance.PlayEffSound("poison", 0.4f);

            if (debuffDic["독"] <= 0)
                debuffDic.Remove("독");
        }

    }

    public void LateDebuffEff()
    {
        if (debuffDic.ContainsKey("약화"))
        {
            debuffDic["약화"]--;

            if (debuffDic["약화"] <= 0)
                debuffDic.Remove("약화");
        }
    }
}