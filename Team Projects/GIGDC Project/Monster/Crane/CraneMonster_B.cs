using UnityEngine;

public class CraneMonster_B : Monster
{

    //이동 관련 변수
    float MoveTime = 3.0f;
    bool MoveRight = false;

    public GameObject attackEff;
    public Transform effSpawnPos;

    Vector2 attackSize = new Vector2(3, 3);

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackPos.position, new Vector2(3, 3));
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
        m_Monstate = MonsterState.PATROL;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        CheckDistanceFromPlayer(true);
        MonUpdate();

    }

    public void MonUpdate()
    {
        m_CalcVec.y = this.transform.position.y;
        if (m_Monstate == MonsterState.PATROL)
        {
            PatrolUpdate();
        }
        else if(m_Monstate == MonsterState.CHASE)
        {
            ChaseUpdate();
        }
        else if(m_Monstate == MonsterState.ATTACK)
        {
            AttackUpdate();
        }
        else if (m_Monstate == MonsterState.DIE)
        {
            m_Animator.SetTrigger("DieTrigger");
            m_Monstate = MonsterState.CORPSE;
            CoinDrop();
        }
        else if (m_Monstate == MonsterState.CORPSE)
        {
            //-- 아무것도 안함
        }
    }

    public void PatrolUpdate()
    {

        if (MoveTime >= 0.0f)
        {
            MoveTime -= Time.deltaTime;
            if (MoveRight)
            {
                m_Rb.transform.position += Vector3.right * m_MoveSpeed * Time.deltaTime;
            }
            else
            {
                m_Rb.transform.position += Vector3.left * m_MoveSpeed * Time.deltaTime;
            }

            if (MoveTime <= 0.0f)
            {
                MoveTime = Random.Range(3.0f, 5.0f);
                MoveRight = !MoveRight;

                if (MoveRight)
                {
                    this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
                }
                else
                {
                    this.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
                }
            }
        }


        if (m_CalcVec.magnitude <= m_ChaseDistance)
        {
            m_Monstate = MonsterState.CHASE;

        }
    }

    void SpawnEff()
    {
        Vector3 spawnInterval = effSpawnPos.position;
        for(int i = 0; i < 3; i++)
        {
            GameObject eff = Instantiate(attackEff);

            spawnInterval = effSpawnPos.position;
            if(!MoveRight)
                spawnInterval.x += i * 0.75f;
            else
                spawnInterval.x -= i * 0.75f;

            Debug.Log(spawnInterval.x);
            eff.transform.position = spawnInterval;
            eff.transform.eulerAngles = effSpawnPos.eulerAngles;

            Destroy(eff, 0.2f);
        }

    }

    public void ChaseUpdate()
    {
        if (m_CalcVec.x >= 0.1f)
        {
            m_Rb.transform.position += Vector3.right * m_MoveSpeed * 2.0f * Time.deltaTime;
            this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            MoveRight = true;
        }
        else if (m_CalcVec.x <= -0.1f)
        {
            m_Rb.transform.position += Vector3.left * m_MoveSpeed * 2.0f * Time.deltaTime;
            this.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
            MoveRight = false;
        }

        if (m_CalcVec.magnitude <= m_AttackDistance)
        {
            m_Monstate = MonsterState.ATTACK;
            m_Animator.SetBool("CanAttack", true);
        }
    }

    public void AttackUpdate()
    {
        if (m_CalcVec.magnitude >= m_AttackDistance)
        {
            m_Animator.SetBool("CanAttack", false);
        }
    }

    public void ChangeRotate()
    {
        if (m_CalcVec.x >= 0.1f)
        {
            this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            MoveRight = true;
        }
        else if (m_CalcVec.x <= -0.1f)
        {
            this.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
            MoveRight = false;
        }
    }


    void Attack()
    {
        Vector3 attackdir = attackPos.position - originPos;

        Collider2D coll = Physics2D.OverlapBox(attackPos.position, attackSize * 0.5f, 0, playerMask);
        if (coll != null)
        {
            if (coll.gameObject.TryGetComponent(out playerTakeDmg))
            {
                Debug.Log("몬스터에 의한 데미지");
                playerTakeDmg.P_TakeDamage(m_Atk);
            }
        }
        else
            Debug.Log("검출안됨");
    }

    public override void TakeDamage(int WeaponState)
    {
        if (m_CurHP <= 0.0f)
            return;

        float a_DamVal = GlobalUserData.m_weaponDataList[WeaponState].m_WeaponDamage;
        float a_CritVal = GlobalUserData.m_weaponDataList[WeaponState].m_Critical;
        float a_CritDmg = a_DamVal + (a_DamVal * GlobalUserData.m_weaponDataList[WeaponState].m_CriticalDmg);

        Debug.Log(a_CritDmg);

        int crit = Random.Range(0, 100);

        if (crit < a_CritVal)
        {
            m_CurHP -= a_CritDmg;
            Debug.Log("crit!");
        }
        else
        {
            m_CurHP -= a_DamVal;
            Debug.Log("몬스터피격");
        }

        if (m_CurHP <= 0)
        {
            m_CurHP = 0.0f;
            m_Monstate = MonsterState.DIE;
            m_Rb.gravityScale = 1.5f;
        }
    }
}