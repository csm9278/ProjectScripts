using UnityEngine;

public class CraneMonster_A : Monster
{

    //이동 관련 변수
    float MoveTime = 3.0f;
    bool MoveRight = false;

    //공격 관련 변수
    public float m_AttackDelay = 1.5f;
    Vector2 attackSize = new Vector2(3, 3);
    float effTimer = 0.3f;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
        m_Monstate = MonsterState.PATROL;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackPos.position, new Vector2(3, 3)); 
        Gizmos.DrawRay(originPos, (attackPos.position - originPos));
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        originPos = new Vector3(this.transform.position.x, attackPos.position.y, 0.0f);

        CheckDistanceFromPlayer();
        MonUpdate();
    }

    public void MonUpdate()
    {
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
            GameObject m_Gold = null;
            m_Gold = (GameObject)Instantiate(Resources.Load("Gold"));
            m_Gold.transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
        }
        else if (m_Monstate == MonsterState.CORPSE)
        {

        }
        else if (m_Monstate == MonsterState.Hitted)
        {
            if (HittedTIme >= 0.0f)
            {
                HittedTIme -= Time.deltaTime;
                if (HittedTIme <= 0.0f)
                {
                    m_Monstate = MonsterState.CHASE;
                    m_Animator.SetBool("IsMove", true);
                }
            }
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
                    this.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
                }
                else
                {
                    this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
                }
            }
        }

        if(m_CalcVec.magnitude <= m_ChaseDistance)
        {
            m_Monstate = MonsterState.CHASE;

        }
    }

    public void ChaseUpdate()
    {

        if (m_CalcVec.x >= 0.1f)
        {
            m_Rb.transform.position += Vector3.right * m_MoveSpeed * 2.0f * Time.deltaTime;
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            MoveRight = true;
        }
        else if (m_CalcVec.x <= -0.1f)
        {
            m_Rb.transform.position += Vector3.left * m_MoveSpeed * 2.0f * Time.deltaTime;
            this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            MoveRight = false;
        }

        if(m_CalcVec.magnitude <= m_AttackDistance)
        {
            m_Monstate = MonsterState.ATTACK;
            m_Animator.SetBool("CanAttack", true);
        }

        if (m_ChaseDistance * 1.5f  < m_CalcVec.magnitude)
        {
            m_Monstate = MonsterState.PATROL;
        }
    }

    public void AttackUpdate()
    {
        if (m_CalcVec.magnitude >= 0.5f)
        {
            m_Animator.SetBool("CanAttack", false);
        }
    }

    public void CraneAttEff()
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

}