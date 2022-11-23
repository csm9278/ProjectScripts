using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public delegate void SkillPointer(MonsterCtrl monctrl, bool isWeak = false);
    public static int[] maxExp = { 2, 4, 8, 14, 20 };

    public static bool isBossStage = false;
    public static bool isLevelUp = false;

    public static bool StagetutorialOn = true;
    public static bool BattletutorialOn = true;
    public static bool ShoptutorialOn = true;
    public static bool EquiptutorialOn = true;

    public static bool isWarriorClear = false;
    public static bool isThiefClear = false;
    public static bool isMageClear = false;

    public static bool cheatMode = false;

    public class Hero
    {
        public int maxHp;
        public int curHp;
        public int gold;
        public int lv;
        public int Exp;
        public int maxExp;
        public int ActivePoint;
        public int AddAtk;

        public SkillPointer[] Skills;
        public string[] SkillInfo;
        public int[] SkillCost;

        public Hero(int hp)
        {
            maxHp = hp;
            curHp = maxHp;
            gold = 0;
            Exp = 0;
            lv = 1;
            ActivePoint = 3;
            AddAtk = 0;
            maxExp = 2;
            Skills = new SkillPointer[4];
            SkillInfo = new string[4];
            SkillCost = new int[4];
        }

        public void ChangeSkill(SkillPointer changeskill, string skillInfo, int skillCost, int changeIdx)
        {
            Skills[changeIdx] = changeskill;
            SkillInfo[changeIdx] = skillInfo;
            SkillCost[changeIdx] = skillCost;
        }

        public void GetExp(int value)
        {
            Exp += value;
            if (Exp >= maxExp)
            {
                lv++;
                maxExp = GlobalData.maxExp[lv - 1];
                maxHp += 2;
                if(lv % 2 == 0)
                    ActivePoint++;
                curHp = maxHp;
                Exp = 0;
                Debug.Log("레벨업");
            }
        }

        public void HealHero(int value)
        {
            SoundMgr.Instance.PlayEffSound("heal", 0.4f);

            curHp += value;
            if (curHp >= maxHp)
                curHp = maxHp;
        }
    }

    public static Hero hero = new Hero(20);

    public enum HeroType
    {
        Warrior,
        Assasin,
        Mage
    }

    public static string heroName = "전사";

    public class CStage
    {
        public int battleNum;
        public int curbattleNum;
        public int eventNum;
        public int cureventNum;
        public int shopNum;
        public int curshopNum;
        public int HealNum;
        public int curHealNum;

        public int floor;
        public int curfloor;
        public int Maxfloor;

        public CStage(int bNum, int eNum, int sNum, int hNum)
        {
            battleNum = bNum;
            eventNum = eNum;
            shopNum = sNum;
            HealNum = hNum;

            ResetNum();

            floor = 0;
            Maxfloor = battleNum + eventNum + shopNum + HealNum;
        }

        public void ResetNum()
        {
            curbattleNum = battleNum;
            cureventNum = eventNum;
            curshopNum = shopNum;
            curHealNum = HealNum;
            curfloor = floor;
        }
    }

    public static void Reset()
    {
        if(cheatMode)
        {
            hero = new Hero(900);
            hero.gold += 100;
        }
        else
            hero = new Hero(20);

        stage1 = new CStage(11, 2, 4, 5);

        stageLevel = 1;

        isBossStage = false;
        lv1Count = monsterLv1.Length;
        lv2Count = monsterLv2.Length;
        lv3Count = monsterLv3.Length;
        lv4Count = monsterLv4.Length;

        monsterListLv1.Clear();
        monsterListLv2.Clear();
        monsterListLv3.Clear();
        monsterListLv4.Clear();
        monsterListLv5.Clear();

        MonsterListInit();
    }

    //이미지팩
    public static Sprite[] ImgPack1 = Resources.LoadAll<Sprite>("pack1");

    public static HeroType heroType = HeroType.Warrior;
    public static CStage stage1 = new CStage(11, 2, 4, 5);

    //몬스터 리스트
    public static int stageLevel = 1;

    public static GameObject[] monsterLv1 = Resources.LoadAll<GameObject>("Monsters/Lv1");
    public static GameObject[] monsterLv2 = Resources.LoadAll<GameObject>("Monsters/Lv2");
    public static GameObject[] monsterLv3 = Resources.LoadAll<GameObject>("Monsters/Lv3");
    public static GameObject[] monsterLv4 = Resources.LoadAll<GameObject>("Monsters/Lv4");
    public static GameObject[] monsterLv5 = Resources.LoadAll<GameObject>("Monsters/Lv5");

    public static int lv1Count = monsterLv1.Length;
    public static int lv2Count = monsterLv2.Length;
    public static int lv3Count = monsterLv3.Length;
    public static int lv4Count = monsterLv4.Length;
    public static int lv5Count = monsterLv5.Length;

    public static List<GameObject> monsterListLv1 = new List<GameObject>();
    public static List<GameObject> monsterListLv2 = new List<GameObject>();
    public static List<GameObject> monsterListLv3 = new List<GameObject>();
    public static List<GameObject> monsterListLv4 = new List<GameObject>();
    public static List<GameObject> monsterListLv5 = new List<GameObject>();

    public static void MonsterListInit()
    {
        for (int i = 0; i < lv1Count; i++)
        {
            monsterListLv1.Add(monsterLv1[i]);
        }

        for(int i = 0; i < lv2Count; i++)
        {
            monsterListLv2.Add(monsterLv2[i]);
        }

        for (int i = 0; i < lv3Count; i++)
        {
            monsterListLv3.Add(monsterLv3[i]);
        }

        for (int i = 0; i < lv4Count; i++)
        {
            monsterListLv4.Add(monsterLv4[i]);
        }

        for (int i = 0; i < lv4Count; i++)
        {
            monsterListLv5.Add(monsterLv5[i]);
        }

    }
}