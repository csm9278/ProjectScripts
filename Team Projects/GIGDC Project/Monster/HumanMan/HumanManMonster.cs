using UnityEngine;

public class HumanManMonster : Monster
{
    public float m_DelayTime = 0.0f;
    bool m_IsRight = false;
    Vector3 m_Cacy = Vector3.zero;
    Vector2 attackSize = new Vector2(3, 3);

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(originPos, (attackPos.position - originPos));
        Gizmos.DrawCube(attackPos.position, new Vector2(3, 3));
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        originPos = new Vector3(this.transform.position.x, attackPos.position.y, 0.0f);

        CheckDistanceFromPlayer();
        AiUpdate();


        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    m_Monstate = MonsterState.DIE;
        //}
    }

    void AiUpdate()
    {
        if (m_Player == null)
        {
            Debug.Log("Player Null");
            return;
        }

        if (m_Monstate == MonsterState.IDLE)
        {
            if (m_DelayTime >= 0.0f)
            {
                m_DelayTime -= Time.deltaTime;
                if (m_DelayTime <= 0.0f)
                {
                    m_Monstate = MonsterState.PATROL;
                    m_DelayTime = Random.Range(2.0f, 3.0f);
                    m_Animator.SetBool("IsMove", true);
                }
            }

            if (m_CalcVec.magnitude <= m_ChaseDistance) // 체이스 거리 안에 들어올 시
            {
                m_Monstate = MonsterState.CHASE;
                m_Animator.SetBool("IsMove", true);
            }
        }
        else if (m_Monstate == MonsterState.PATROL)
        {
            if (m_CalcVec.magnitude <= m_ChaseDistance) // 체이스 거리 안에 들어올 시
            {
                m_Monstate = MonsterState.CHASE;
            }

            if (m_DelayTime >= 0.0f)
            {
                m_DelayTime -= Time.deltaTime;

                if (m_IsRight)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                    m_Rb.transform.position += Vector3.right * Time.deltaTime * (m_MoveSpeed / 3);
                }
                else
                {
                    this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
                    m_Rb.transform.position += Vector3.left * Time.deltaTime * (m_MoveSpeed / 3);
                }

                if (m_DelayTime <= 0.0f)
                {
                    m_Monstate = MonsterState.IDLE;
                    m_DelayTime = Random.Range(1.5f, 3.0f);
                    m_Animator.SetBool("IsMove", false);
                    m_IsRight = !m_IsRight;
                }
            }
        }
        else if (m_Monstate == MonsterState.CHASE)
        {
            if (m_CalcVec.x >= 0.1f)
            {
                m_Rb.transform.position += Vector3.right * m_MoveSpeed * Time.deltaTime;
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (m_CalcVec.x <= -0.1f)
            {
                m_Rb.transform.position += Vector3.left * m_MoveSpeed * Time.deltaTime;
                this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            }

            if (m_CalcVec.magnitude <= m_AttackDistance)
            {
                m_Monstate = MonsterState.ATTACK;
                SoundMgr.Instance.PlayGUISound("MZombie_att", 0.3f);
                m_Animator.SetBool("IsAttack", true);
            }
        }
        else if (m_Monstate == MonsterState.SKILL)
        {

        }
        else if (m_Monstate == MonsterState.ATTACK)
        {
            if (m_CalcVec.magnitude >= .5f)
            {
                m_Animator.SetBool("IsAttack", false);
            }

            if (m_Animator.GetBool("IsAttack") == false && m_CalcVec.magnitude <= m_AttackDistance)
            {
                m_Animator.SetBool("IsAttack", true);
                //SoundMgr.Instance.PlayEffSound("MZombie_Knife", 1.0f);
            }
        }
        else if(m_Monstate == MonsterState.DIE)
        {
            m_Animator.SetTrigger("DieTrigger");
            m_Monstate = MonsterState.CORPSE;
            SoundMgr.Instance.PlayEffSound("WZombie_Die", 1.0f);
            CoinDrop();
        }
        else if(m_Monstate == MonsterState.CORPSE)
        {
            //아무것도 안할거

        }
        else if (m_Monstate == MonsterState.Hitted)
        {
            m_Animator.SetBool("IsAttack", false);

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

    /// <summary>
    /// 사또가 몬스터 소환시 호출할것
    /// </summary>
    public void SpawnFunc()
    {
        m_Monstate = MonsterState.CHASE;
        if (m_Animator != null)
            m_Animator.SetBool("IsMove", true);
        else
            Debug.Log("NullAnimator");
    }

    public void HumanManAttackEff()
    {

        Vector3 attackdir = attackPos.position - originPos;

        //attackhit = Physics2D.Raycast(originPos, attackdir, attackdir.magnitude, playerMask);
        //if (attackhit)
        //{
        //    if (attackhit.collider.gameObject.TryGetComponent(out playerTakeDmg))
        //    {
        //        Debug.Log("몬스터에 의한 데미지");
        //        playerTakeDmg.P_TakeDamage(m_Atk);
        //    }
        //}

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