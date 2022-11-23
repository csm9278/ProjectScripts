using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillData
{
    public delegate void MonsterSKill(HeroStatueMgr mgr, ref string InfoText, bool isWeak = false);

    public static Dictionary<string, MonsterSKill> MonSkillList = new Dictionary<string, MonsterSKill>();

    public static int BoxKnightDamage = 9;
    static int RabbitTurn = 1;
    static int MoonNightTurn = 1;
    static int AliceTurn = 1;

    public static void InitMonSKillData()
    {
        MonSkillList.Add("불량배", BullySkill);
        MonSkillList.Add("오징어", SquidSkill);
        MonSkillList.Add("개구리 전사", FrogSkill);
        MonSkillList.Add("불덩이", FireSkill);
        MonSkillList.Add("상자기사", BoxKnightSkill);
        MonSkillList.Add("들개", DogSkill);
        MonSkillList.Add("불의 정령", FireElementalSkill);
        MonSkillList.Add("숲의 정령", ForestElementalSkill);
        MonSkillList.Add("도굴꾼", RabbitSkill);
        MonSkillList.Add("정비공", EngineerSkill);
        MonSkillList.Add("석영", GemStoneSkill);
        MonSkillList.Add("유령", GhostSkill);
        MonSkillList.Add("흉내쟁이", MirrorManSkill);
        MonSkillList.Add("독항아리", PoisonBoxSkill);
        MonSkillList.Add("검은영혼", DarkSoulSkill);
        MonSkillList.Add("열쇠공", KeyManSkill);
        MonSkillList.Add("문나이트", MoonNightSkill);
        MonSkillList.Add("앨리스", AliceSkill);
        MonSkillList.Add("드래곤", DragonSkill);
        MonSkillList.Add("듀라한", DurahanSkill);
    }

    public static void SquidSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        InfoText = "1~3데미지 + 2데미지를 준다";

        int rand = Random.Range(1, 4);
        rand += 2;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + rand + " -> " + (int)(rand * 0.75));

            rand = (int)(rand * 0.75f);
        }

        mgr._battlemgr.AddStateText("<color=orange>" + rand.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(rand);
    }
    public static void FrogSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {

        int rand = Random.Range(1, 4);
        rand += 2;
        InfoText = "1~3데미지 + 2데미지를 준다";

        if (isWeak)
        {
            Debug.Log("약화 적용 " + rand + " -> " + (int)(rand * 0.75));

            rand = (int)(rand * 0.75f);
        }


        mgr._battlemgr.AddStateText("<color=orange>" + rand.ToString() + "데미지를 입었다</color>");


        mgr.SetHp(rand);
    }
    public static void BullySkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        InfoText = "1~2 데미지 * 2배 데미지를 준다";


        int rand = Random.Range(1, 3);
        rand *= 2;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + rand + " -> " + (int)(rand * 0.75));

            rand = (int)(rand * 0.75f);
        }


        mgr._battlemgr.AddStateText("<color=orange>" + rand.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(rand);
    }
    public static void FireSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        InfoText = "1~2 데미지 * 2배 데미지를 준다";

        int rand = Random.Range(1, 3);
        rand *= 2;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + rand + " -> " + (int)(rand * 0.75));

            rand = (int)(rand * 0.75f);
        }


        mgr._battlemgr.AddStateText("<color=orange>" + rand.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(rand);
    }
    public static void BoxKnightSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("상자기사 스킬");
        Debug.Log("9 데미지를 준다 때릴때마다 2씩 감소 최소 1");

        InfoText = "9 데미지를 준다 때릴때마다 2씩 감소 최소 1";

        int currentDmg = BoxKnightDamage;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + BoxKnightDamage + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(BoxKnightDamage * 0.75f);
        }

        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(currentDmg);
        BoxKnightDamage -= 2;

        if (BoxKnightDamage <= 1)
            BoxKnightDamage = 1;
    }
    public static void DogSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("들개 스킬");
        Debug.Log("1~3 피해를 준다 2독 상태 부여");

        InfoText = "1~3 피해를 준다 2독 상태 부여";


        int currentDmg = Random.Range(1, 4);
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }

        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");


        mgr.SetHp(currentDmg);
        mgr.AddDebuff("독", 2, true);
    }
    public static void FireElementalSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("화염정령 스킬");
        Debug.Log("3~6 피해를 주고 2화염 상태부여");
        InfoText = "3~6 피해를 주고 2화염 상태 부여";

        int rand = Random.Range(3, 7);
        int currentDmg = rand;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }



        mgr.SetHp(currentDmg);
        mgr.AddDebuff("화염", 2, true);
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

    }
    public static void ForestElementalSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("숲의 정령 스킬");
        Debug.Log("1~6 피해를 준다 2독 상태 부여");
        InfoText = "1~6 피해를 준다 2독 상태 부여";

        int rand = Random.Range(1, 7);
        int currentDmg = rand;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.AddDebuff("독", 2, true);

        if (rand % 2 == 0)
        {
            mgr.SetHp(currentDmg * 2);
        }
        else
            mgr.SetHp(currentDmg);
    }
    public static void RabbitSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("도굴꾼 스킬");
        Debug.Log("3 * 턴수 피해를 준다.");
        InfoText = "3 * 턴수 피해를 준고 약화상태 부여.";

        int currentDmg = 3;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }

        mgr.AddDebuff("약화", 1,  true);
        mgr.SetHp(currentDmg * RabbitTurn);
        mgr._battlemgr.AddStateText("<color=orange>" + (currentDmg * RabbitTurn).ToString() + "데미지를 입었다</color>");
        RabbitTurn++;

    }
    public static void EngineerSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("엔지니어 스킬");
        Debug.Log("1 ~ 6 주사위를 2개 굴린후 합한만큼 데미지를 준다");
        InfoText = "1 ~ 6 주사위를 2개 굴린후 합한만큼 데미지를 준다";

        int rand = Random.Range(1, 7);

        int currentDmg = rand;

        rand = Random.Range(1, 7);
        currentDmg += rand;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(currentDmg);
    }
    public static void GemStoneSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("석영 스킬");
        Debug.Log("1 ~ 10사이의 피해를 준다");
        InfoText = "1 ~ 10사이의 피해를 준다";

        int rand = Random.Range(1, 11);

        int currentDmg = rand;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));
            mgr._battlemgr.AddStateText("<color=orange>" + ((int)(currentDmg * 0.75f)).ToString() + "데미지를 입었다</color>");

            currentDmg = (int)(currentDmg * 0.75f);
        }
        else
        {
            mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        }

        mgr.SetHp(currentDmg);
    }
    public static void GhostSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("유령 스킬");
        Debug.Log("1 ~ 2사이의 피해를 모아서 준다. 70%확률로 데미지를 추가로 축척한다.");
        InfoText = "1 ~ 2사이의 피해를 모아서 준다. 70%확률로 데미지를 추가로 축척한다.";
        int rand;
        int currentDmg = 0;

        for (int i = 0; i < 100; i++)
        {
            rand = Random.Range(1, 3);
            currentDmg += rand;

            rand = Random.Range(1, 11);
            if (rand <= 2)
                break;
        }


        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }

        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(currentDmg);
        Debug.Log(currentDmg);

    }
    public static void MirrorManSkill(HeroStatueMgr mgr, ref string InfoText, bool isWeak = false)
    {
        Debug.Log("흉내쟁이 스킬");
        int rand = Random.Range(0, 4);

        switch(rand)
        {
            case 0:
                MonSkillList["정비공"](mgr, ref InfoText, isWeak);
                break;
            case 1:
                MonSkillList["석영"](mgr, ref InfoText, isWeak);
                break;
            case 2:
                MonSkillList["유령"](mgr, ref InfoText, isWeak);
                break;
            case 3:
                MonSkillList["독항아리"](mgr, ref InfoText, isWeak);
                break;
        }

    }
    public static void PoisonBoxSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("독항아리 스킬");
        Debug.Log("1 ~ 2 사이 데미지를 주고 3독 상태 부여");
        InfoText = "1 ~ 2 사이 데미지를 주고 3독 상태 부여";

        int rand = Random.Range(1, 2);

        int currentDmg = rand;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }

        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.AddDebuff("독", 3, true);
        mgr.SetHp(currentDmg);

    }
    public static void DarkSoulSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("검은 영혼 스킬");
        Debug.Log("5 ~ 15 피해를 준다");
        InfoText = "5 ~ 15 사이의 피해를 준다.";

        int rand = Random.Range(5, 16);

        int currentDmg = rand;

        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(currentDmg);

    }
    public static void KeyManSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("열쇠공 스킬");
        Debug.Log("3의 피해를 준다 연속타격 70퍼센트");
        InfoText = "3의 피해를 준다 연속타격 70퍼센트";

        int rand;
        int currentDmg = 0;

        for (int i = 0; i < 100; i++)
        {
            currentDmg += 3;

            rand = Random.Range(0, 10);
            if (rand < 7)
                break;
        }

        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(currentDmg);

    }
    public static void MoonNightSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("문나이트 스킬");
        Debug.Log("8 * 턴수 피해를 준다.");
        InfoText = "8 * 턴수 피해를 준고 약화상태 부여. 현재 턴 : " + MoonNightTurn.ToString();

        int currentDmg = 8;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }
        mgr._battlemgr.AddStateText("<color=orange>" + (currentDmg * MoonNightTurn).ToString() + "데미지를 입었다</color>");

        mgr.AddDebuff("약화", 1, true);
        mgr.SetHp(currentDmg * MoonNightTurn);
        MoonNightTurn++;
    }
    public static void AliceSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("앨리스 스킬");
        Debug.Log("5피해를 주고 모든 상태이상을 2만큼 부여한다.");
        InfoText = "5피해를 주고 모든 상태이상을 2만큼 부여한다.";

        int currentDmg = 5;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.AddDebuff("독", 2, true);
        mgr.AddDebuff("약화", 2, true);
        mgr.AddDebuff("화염", 2, true);
        mgr.SetHp(currentDmg);
    }
    public static void DragonSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("드래곤 스킬");
        Debug.Log("3~6 피해의 3배를 준다 데미지가 8일시 5배 피해");
        InfoText = "3~6 피해의 3배를 준다 데미지가 8일시 5배 피해";

        int rand = Random.Range(3, 7);
        int currentDmg = rand;
        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
        }

        if (rand == 6)
        {
            mgr.SetHp(currentDmg * 5);
            mgr._battlemgr.AddStateText("<color=orange>" + (currentDmg * 5).ToString() + "데미지를 입었다</color>");

        }
        else
        {
            mgr._battlemgr.AddStateText("<color=orange>" + (currentDmg * 3).ToString() + "데미지를 입었다</color>");

            mgr.SetHp(currentDmg * 3);
        }
    }
    public static void DurahanSkill(HeroStatueMgr mgr,ref string InfoText, bool isWeak = false)
    {
        Debug.Log("듀라한 스킬");
        Debug.Log("4~5사이의 피해를 모아서 준다. 70%확률로 데미지를 추가로 축척한다.");
        InfoText = "4~5사이의 피해를 모아서 준다. 70%확률로 데미지를 추가로 축척한다.";

        int rand;
        int currentDmg = 0;

        for (int i = 0; i < 100; i++)
        {
            rand = Random.Range(4, 6);
            currentDmg += rand;

            rand = Random.Range(0, 10);
            if (rand < 3)
                break;
        }


        if (isWeak)
        {
            Debug.Log("약화 적용 " + currentDmg + " -> " + (int)(currentDmg * 0.75));

            currentDmg = (int)(currentDmg * 0.75f);
            mgr._battlemgr.AddStateText("<color=orange>" + ((int)(currentDmg * 0.75)).ToString() + "데미지를 입었다</color>");

        }
        else
        mgr._battlemgr.AddStateText("<color=orange>" + currentDmg.ToString() + "데미지를 입었다</color>");

        mgr.SetHp(currentDmg);
        Debug.Log(currentDmg);

    }
}