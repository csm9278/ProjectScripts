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
                            target.battlemanager.AddStateText("<color=gray>(????) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " ???????? ??????.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "???????? ??????.</color>");
                        }



                        target.TakeDamage(damage);
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);
                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(????) " + (damage * debuffCount).ToString() + " -> " + ((int)((damage * debuffCount) * 0.75f)).ToString() + " ???????? ??????.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + (damage * debuffCount).ToString() + "???????? ??????.</color>");
                        }

                        target.TakeDamage(damage * debuffCount);
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);

                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(????) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " ???????? ??????.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "???????? ??????.</color>");
                        }

                        target.TakeDamage(damage * debuffCount);

                        target.AddDebuff(debuffName, debuffCount);
                        target.battlemanager.AddStateText("<color=brown>?????? " + debuffCount.ToString() + "???? ??????.</color>");
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);
                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(????) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " ???????? ??????.</color>");
                            target.battlemanager.AddStateText("<color=gray>(????) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " ???????? ??????.</color>");

                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "???????? ??????.</color>");
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "???????? ??????.</color>");
                        }


                        target.TakeDamage(damage);
                        target.TakeDamage(damage);
                    }
                    break;


                case "??":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);
                        Debug.Log(damage);
                        if (isWeak)
                        {
                            target.battlemanager.AddStateText("<color=gray>(????) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " ???????? ??????.</color>");
                            damage = (int)(damage * 0.75f);
                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "???????? ??????.</color>");
                        }

                        target.TakeDamage(damage);

                        target.AddDebuff(debuffName, debuffCount);
                        target.battlemanager.AddStateText("<color=#D92E38>????" + debuffCount.ToString() + "???? ??????.</color>");

                    }
                    break;

                case "????":
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
                        Debug.Log("???????? : " + attackTime);


                        CoroutineHandler.Start_Coroutine(ManyAttack(target, attackTime, isWeak));
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int heal = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1);

                        target.battlemanager.AddStateText("<color=lime>" + heal.ToString() + "?????? ????????.</color>");

                        GlobalData.hero.HealHero(heal);
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int heal = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);

                        target.battlemanager.AddStateText("<color=orange>" + heal.ToString() + "???????? ??????.</color>");
                        target.battlemanager.AddStateText("<color=lime>" + heal.ToString() + "?????? ????????.</color>");

                        target.TakeDamage(heal);

                        GlobalData.hero.HealHero(heal);
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int damage = Random.Range(minDmg + GlobalData.hero.AddAtk, maxDmg + 1 + GlobalData.hero.AddAtk);

                        target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + " + " + debuffCount.ToString() + "???????? ??????.</color>");

                        target.TakeDamage(damage + debuffCount);
                    }
                    break;

                case "????":
                    if (target != null)
                    {
                        int percent = Random.Range(0, 101);

                        if(percent < debuffCount)
                        {
                            target.battlemanager.AddStateText("<color=orange>" + minDmg.ToString() + "???????? ??????.</color>");
                            target.TakeDamage(minDmg + GlobalData.hero.AddAtk);

                        }
                        else
                        {
                            target.battlemanager.AddStateText("<color=orange>???????? ?????? ??????...</color>");
                        }


                    }
                    break;

                case "????":
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
                        target.battlemanager.AddStateText("<color=gray>(????) " + damage.ToString() + " -> " + ((int)(damage * 0.75f)).ToString() + " ???????? ??????.</color>");
                        damage = (int)(damage * 0.75f);
                    }
                    else
                    {
                        target.battlemanager.AddStateText("<color=orange>" + damage.ToString() + "???????? ??????.</color>");
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
        Skill skill = new Skill("???? ????", "3~6??????", 3, 6, "", 0, 2);
        warriorSkill.Add(skill);
        skill = new Skill("?? ??????", "1???????? ????", 1, 1, "", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("?????? ????", "3~6?????? + 2??????", 3, 6, "????", 2, 2);
        warriorSkill.Add(skill);
        skill = new Skill("??????", "3~6???????? 3?? ????", 3, 6, "????", 3, 4);
        warriorSkill.Add(skill);
        skill = new Skill("????", "3 ~ 5 ???????? 2???? ????", 3, 5, "????", 2, 3);
        warriorSkill.Add(skill);
        skill = new Skill("????", "3???????? ????", 3, 3, "", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("????????", "1~3 ???????? ???? ????", 1, 3, "????", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("???? ??????", "1???????? 50%?????? ???? ??????", 1, 1, "????", 50, 1);
        warriorSkill.Add(skill);
        skill = new Skill("???? ????", "1~5???????? 2???? ???????? ????. ", 1, 5, "????", 2, 3);
        warriorSkill.Add(skill);
        skill = new Skill("?? ??????", "1???????? ???? 1 ?????? ????", 1, 1, "??", 1, 1);
        warriorSkill.Add(skill);
        skill = new Skill("???? ????", "2???????? ???? 2 ???????? ????", 2, 2, "????", 2, 3);
        warriorSkill.Add(skill);
        skill = new Skill("???? ????", "1~3 ?????? ????????.", 1, 3, "????", 0, 1);
        warriorSkill.Add(skill);
        skill = new Skill("???? ????", "1~6 ???????? ???? ?????? ?????? ????????.", 1, 6, "????", 0, 2);
        warriorSkill.Add(skill);

        //???? ????
        skill = new Skill("???? ????", "1 ~ 3 ??????", 1, 3, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("?? ??????", "1???????? ????", 1, 1, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("?????? ????????", "1 ???????? ???? 2 ???????? ????", 1, 1, "??", 2, 1);
        thiefSkill.Add(skill);

        skill = new Skill("???? ???? ????", "2~3???????? 3?? ????", 2, 3, "????", 4, 2);
        thiefSkill.Add(skill);

        skill = new Skill("?? ??????", "1 ???????? ???? 2???????? ????", 1, 1, "????", 2, 1);
        thiefSkill.Add(skill);

        skill = new Skill("???? ??????", "2 ~ 4 ???????? ????", 1, 1, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("???? ??????", "2 ~ 4 ???????? ????", 1, 1, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("???? ????", "1~3 ???????? ???? 4 ?????? ????", 1, 3, "??", 4, 2);
        thiefSkill.Add(skill);

        skill = new Skill("???? ????", "1 ~ 3 ???????? ???? ?????? ?????? ????????", 1, 3, "????", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("????", "3???????? ????", 3, 3, "", 0, 1);
        thiefSkill.Add(skill);

        skill = new Skill("?????? ????", "3???????? 4?? ????", 3, 3, "????", 4, 4);
        thiefSkill.Add(skill);

        skill = new Skill("???? ??????", "10???????? 60% ?????? ????", 10, 10, "????", 60, 1);
        thiefSkill.Add(skill);

        //skill = new Skill("???????? ????", "999???????? ????", 999, 999, "", 0, 1);
        //warriorSkill.Add(skill);
    }
}