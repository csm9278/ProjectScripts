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
            Debug.Log("Ű����");
            MonSkillEff = MonsterSkillData.MonSkillList[mondata.monName];
        }
        else
        {
            Debug.Log("Ű����");
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

            if (debuffName == "��")
                battlemanager.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Poison, debuffCount);
            else if (debuffName == "��ȭ")
                battlemanager.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Weak, debuffCount);
            else if (debuffName == "ȭ��")
                battlemanager.AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType.Fire, debuffCount);
        }
        else
        {
            debuffDic.Add(debuffName, debuffCount);
            if (debuffName == "��")
                battlemanager.debuffNodeFunc(DebuffNodeMgr.DebuffType.Poison, debuffCount);
            else if (debuffName == "��ȭ")
                battlemanager.debuffNodeFunc(DebuffNodeMgr.DebuffType.Weak, debuffCount);
            else if (debuffName == "ȭ��")
                battlemanager.debuffNodeFunc(DebuffNodeMgr.DebuffType.Fire, debuffCount);

        }

        if (debuffName == "��")
            SoundMgr.Instance.PlayEffSound("applypoison", 0.4f);
        else if (debuffName == "��ȭ")
            SoundMgr.Instance.PlayEffSound("applyweaken", 0.4f);
        else if (debuffName == "ȭ��")
            SoundMgr.Instance.PlayEffSound("fire", 0.4f);

        //Debug.Log(debuffName + ": " + debuffDic[debuffName]);
    }

    public void DebuffEff()
    {
        if(debuffDic.ContainsKey("��"))
        {
            stmgr.PoisonEff(debuffDic["��"]);
            battlemanager.AddStateText("<color=darkblue>���� " + debuffDic["��"].ToString() + "�� �������� �Ծ���.</color>");
            //Debug.Log(debuffDic["��"] + " ��ŭ �� ������");
            debuffDic["��"]--;
            //Debug.Log(debuffDic["��"] + " ���� �� ī��Ʈ");
            SoundMgr.Instance.PlayEffSound("poison", 0.4f);

            if (debuffDic["��"] <= 0)
                debuffDic.Remove("��");
        }

    }

    public void LateDebuffEff()
    {
        if (debuffDic.ContainsKey("��ȭ"))
        {
            debuffDic["��ȭ"]--;

            if (debuffDic["��ȭ"] <= 0)
                debuffDic.Remove("��ȭ");
        }
    }
}