using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public enum statues
    {
        Hp,
        NormalAtk,
        Heal,
        FireBall,
        WaterFall,
        GainExplosion
    }
    
    public static int StageLevel = 0;

    //������ ����
    public static List<Item> items = new List<Item>();
    public static Item[] equipItems = new Item[4];

    public static bool gameStop = false;

    public static int[] beforeStatus = { 200, 20, 100, 30, 20, 50 };
    public static int[] UpgradeStatus = { 30, 10, 20, 10, 5, 20 };
    public static int[] UpgradePrices = { 100, 100, 100, 100, 100, 100 };


    public static int playerGold = 0;
    public static int playerCurGold = 0;    //������ ���� ���� ��尪

    //������ ����
    public static string[] itemInfoName = { "ü��", "�⺻���� ������", "ȸ����ų ȸ����", "���̾ ������", "���� ������", "���� ������" };
    public static int[] itemStatus = { 0, 0, 0, 0, 0, 0 };

    public static Sprite nonImage = Resources.Load<Sprite>("ItemImage/NoneItem");

    public static void StageLevelUp(int level)
    {
        if (StageLevel < level)
            StageLevel = level;
        else
            return;
    }

    //public static int heroMaxHp = 200;
    //public static int heroNormalAttDmg = 20;
    //public static int heroHeal = 100;
    //public static int fireBallDmg = 30;
    //public static int waterfallDmg = 20;
    //public static int explosionDmg = 50;

    //public static int heroMaxHpUp = 30;
    //public static int heroNormalAttDmgUp = 10;
    //public static int heroHealUp = 20;
    //public static int fireBallDmgUp = 10;
    //public static int waterfallDmgUp = 5;
    //public static int explosionDmgUp = 20;
}