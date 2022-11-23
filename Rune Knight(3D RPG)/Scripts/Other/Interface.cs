using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillEff
{
    void SkillEff();
}

public interface IDamageable
{
    void TakeDamage(int val);
    void PlayEff();
}

public interface GetHp
{
    int GetMaxHp();
    int GetCurHp();
}

public interface SetStauts
{
    void SetStat(int hp, int attackDmg);
}

public interface IAttackPlayer
{
    void SetAttack();
}
