using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillData
{
    static WaitForSeconds manyAttackDelay = new WaitForSeconds(0.3f);

    public class CoroutineHandler : MonoBehaviour
    {
        IEnumerator enumerator = null;
        private void Coroutine(IEnumerator coro)
        {
            enumerator = coro;
            StartCoroutine(coro);
        }

        void Update()
        {
            if (enumerator != null)
            {
                if (enumerator.Current == null)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Stop()
        {
            StopCoroutine(enumerator.ToString());
            Destroy(gameObject);
        }

        public static CoroutineHandler Start_Coroutine(IEnumerator coro)
        {
            GameObject obj = new GameObject("CoroutineHandler");
            CoroutineHandler handler = obj.AddComponent<CoroutineHandler>();
            if (handler)
            {
                handler.Coroutine(coro);
            }
            return handler;
        }
    }

    public class Skill
    {
        public string skillName;
        public string skillOption;
        public int minDmg = 0;
        public int maxDmg = 0;
        public string debuffName;
        public int debuffCount;
        public int skillCost;

        public string skillInfo;

        public Skill(string name, string option, int minD, int maxD, string deBName, int deBCount, int Cost)
        {
            skillName = name;
            skillOption = option;
            minDmg = minD;
            maxDmg = maxD;
            debuffName = deBName;
            debuffCount = deBCount;
            skillCost = Cost;

            skillInfo = skillName + "\n\n" + skillOption;
        }

        public void SkillEff(MonsterCtrl target = null, bool isWeak = false)
        {
            switch(debuffName)
            {
                case "":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);

                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(약화) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " 데미지를 입혔다.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "데미지를 입혔다.</color>");
                        }



                        target.TakeDamage(damage);
                    }
                    break;

                case "배수":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);
                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(약화) " + (damage * debuffCount).ToString() + " -> " + ((int)((damage * debuffCount) * 0.75f)).ToString() + " 데미지를 입혔다.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + (damage * debuffCount).ToString() + "데미지를 입혔다.</color>");
                        }

                        target.TakeDamage(damage * debuffCount);
                    }
                    break;

                case "약화":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);

                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(약화) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " 데미지를 입혔다.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "데미지를 입혔다.</color>");
                        }

                        target.TakeDamage(damage * debuffCount);

                        target.AddDebuff(debuffName, debuffCount);
                        target.battlemanager.AddStateText("<color=brown>약화를 " + debuffCount.ToString() + "만큼 입혔다.</color>");
                    }
                    break;

                case "더블":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);
                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(약화) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " 데미지를 입혔다.</color>");
                            target.battlemanager.AddStateText("<color=gray>(약화) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " 데미지를 입혔다.</color>");

                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "데미지를 입혔다.</color>");
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "데미지를 입혔다.</color>");
                        }


                        target.TakeDamage(damage);
                        target.TakeDamage(damage);
                    }
                    break;


                case "독":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);
                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(약화) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " 데미지를 입혔다.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "데미지를 입혔다.</color>");
                        }

                        target.TakeDamage(damage);

                        target.AddDebuff(debuffName, debuffCount);
                        target.battlemanager.AddStateText("<color=#D92E38>독을" + debuffCount.ToString() + "만큼 입혔다.</color>");

                    }
                    break;

                case "연타":
                    if (target != null)
                    {
                        int attackTime = 1;
                        for(int i = 0; i < 100; i++)
                        {
                            int rand = Random.Range(0, 101);
                            if (rand < debuffCount)
                                attackTime++;
                            else
                                break;
                        }
                        Debug.Log("공격횟수 : " + attackTime);


                        CoroutineHandler.Start_Coroutine(ManyAttack(target, attackTime, isWeak));
                    }
                    break;

                case "회복":
                    if (target != null)
                    {
                        int heal = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1);

                        target.battlemanager.AddStateText("<color=lime>" + heal.ToString() + "체력을 회복했다.</color>");

                        GlobalData.hero.HealHero(heal);
                    }
                    break;

                case "흡혈":
                    if (target != null)
                    {
                        int heal = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);

                        target.battlemanager.AddStateText("<color=orange>" + heal.ToString() + "데미지를 입혔다.</color>");
                        target.battlemanager.AddStateText("<color=lime>" + heal.ToString() + "체력을 회복했다.</color>");

                        target.TakeDamage(heal);

                        GlobalData.hero.HealHero(heal);
                    }
                    break;

                case "추가":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);

                        target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + " + " + debuffCount.ToString() + "데미지를 입혔다.</color>");

                        target.TakeDamage(damage + debuffCount);
                    }
                    break;

                case "확률":
                    if (target != null)
                    {
                        int percent = Random.Range(0, 101);

                        if(percent < debuffCount)
                        {
                            target.battlemanager.AddStateText("<color=orange>" + minDmg.ToString() + "데미지를 입혔다.</color>");
                            target.TakeDamage(minDmg + GlobalData.hero.AddAtk);

                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>데미지를 입히지 못했다...</color>");
                        }


                    }
                    break;

                case "연격":
                    if (target != null)
                    {
                        CoroutineHandler.Start_Coroutine(ManyAttack(target, debuffCount, isWeak));
                    }
                    break;
            }
        }

        IEnumerator ManyAttack(MonsterCtrl target, int attackTime, bool isWeak = false)
        {
            Debug.Log(attackTime);
            for(int i = 0; i < attackTime; i++)
            {
                if (target != null)
                {
                    int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                    Debug.Log(damage);
                    if (isWeak)
                    {
                        target.battlemanager.AddStateText("<color=gray>(약화) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " 데미지를 입혔다.</color>");
                        damage = (int)(damage * 0.75f);
                    }
                    else
                    {
                        target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "데미지를 입혔다.</color>");
                    }
                    target.TakeDamage(damage);
                }


                yield return manyAttackDelay;
            }

            yield break;
        }
    }
    
    public static List<Skill> warriorSkill = new List<Skill>();
    public static List<Skill> thiefSkill = new List<Skill>();

    public static void SkillInit()
    {
        Skill skill = new Skill("일반 공격", "3~6데미지", 3, 6, "", 0, 2);
        warriorSkill.Add(skill);
        skill = new Skill("돌 던지기", "1데미지를 준다", 1, 1, "", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("브로드 소드", "3~6데미지 + 2데미지", 3, 6, "추가", 2, 2);
        warriorSkill.Add(skill);
        skill = new Skill("삼연격", "3~6데미지를 3번 준다", 3, 6, "연격", 3, 4);
        warriorSkill.Add(skill);
        skill = new Skill("강타", "3 ~ 5 데미지를 2배로 준다", 3, 5, "배수", 2, 3);
        warriorSkill.Add(skill);
        skill = new Skill("가해", "3데미지를 준다", 3, 3, "", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("연속공격", "1~3 데미지를 두번 때림", 1, 3, "더블", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("연속 찌르기", "1데미지를 50%확률로 계속 때린다", 1, 1, "연타", 50, 1);
        warriorSkill.Add(skill);
        skill = new Skill("로또 일격", "1~5데미지를 2배의 데미지로 준다. ", 1, 5, "배수", 2, 3);
        warriorSkill.Add(skill);
        skill = new Skill("독 찌르기", "1데미지를 주고 1 독상태 부여", 1, 1, "독", 1, 1);
        warriorSkill.Add(skill);
        skill = new Skill("다리 걸기", "2데미지를 주고 2 약화상태 부여", 2, 2, "약화", 2, 3);
        warriorSkill.Add(skill);
        skill = new Skill("붕대 감기", "1~3 체력을 회복한다.", 1, 3, "회복", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("흡혈 일격", "1~6 데미지를 주고 그만큼 체력을 회복한다.", 1, 6, "흡혈", 0, 2);
        warriorSkill.Add(skill);

        //도적 스킬
        skill = new Skill("단검 공격", "1 ~ 3 데미지", 1, 3, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("돌 던지기", "1데미지를 준다", 1, 1, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("능숙한 독찌르기", "1 데미지를 주고 2 중독상태 부여", 1, 1, "독", 2, 1);
        thiefSkill.Add(skill);

        skill = new Skill("연속 단검 투척", "2~3데미지를 3번 준다", 2, 3, "연격", 4, 2);
        thiefSkill.Add(skill);

        skill = new Skill("눈 찌르기", "1 데미지를 주고 2약화상태 부여", 1, 1, "약화", 2, 1);
        thiefSkill.Add(skill);

        skill = new Skill("급소 찌르기", "2 ~ 4 데미지를 준다", 1, 1, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("급소 찌르기", "2 ~ 4 데미지를 준다", 1, 1, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("독의 대가", "1~3 데미지를 주고 4 독상태 부여", 1, 3, "독", 4, 2);
        thiefSkill.Add(skill);

        skill = new Skill("흡혈 단검", "1 ~ 3 데미지를 주고 그만큼 체력을 회복한다", 1, 3, "흡혈", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("가해", "3데미지를 준다", 3, 3, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("단검의 대가", "3데미지를 4번 준다", 3, 3, "연격", 4, 4);
        thiefSkill.Add(skill);

        skill = new Skill("제발 맞아라", "10데미지를 60% 확률로 준다", 10, 10, "확률", 60, 1);
        thiefSkill.Add(skill);

        //skill = new Skill("관리자의 권한", "999데미지를 준다", 999, 999, "", 0, 1);
        //warriorSkill.Add(skill);
    }
}