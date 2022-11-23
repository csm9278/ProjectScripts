using UnityEngine;

public class HumanWomanMonster : Monster
{
    //대기 관련 변수
    float m_IdleTime = 0.0f;

    public bool isAType = true;

    //이동 관련 변수
    float MoveTime = 3.0f;
    bool MoveRight = false;

    //공격 관련 변수
    public float m_AttackDelay = 1.5f;
    float m_CurattackDelay;
    GameObject WeaponPrefab;
    public Transform weaponPos;
    public float attackDealy_2 = 0.0f;

    private void Awake()
    {
        if(isAType)
            WeaponPrefab = (GameObject)Resources.Load("HumanWoman_Stone");
        else
            WeaponPrefab = (GameObject)Resources.Load("HumanWoman_Spit");
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
        m_Monstate = MonsterState.IDLE;

        m_CurattackDelay = m_AttackDelay;

        m_IdleTime = Random.Range(2.0f, 3.0f);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        CheckDistanceFromPlayer();
        MonUpdate();
    }

    public void MonUpdate()
    {
        if (m_Monstate == MonsterState.IDLE)
        {
            IdleUpdate();
        }
        else if (m_Monstate == MonsterState.PATROL)
        {
            PatrolUpdate();
        }
        else if (m_Monstate == MonsterState.CHASE)
        {
            ChaseUpdate();
        }
        else if (m_Monstate == MonsterState.ATTACK)
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
            Collider2D collider = this.GetComponent<BoxCollider2D>();
            if (collider != null)
                collider.enabled = false;
        }
        else if (m_Monstate == MonsterState.Hitted)
        {
            if (HittedTIme >= 0.0f)
            {
                HittedTIme -= Time.deltaTime;
                if (HittedTIme <= 0.0f)
                {
                    m_Monstate = MonsterState.IDLE;
                    m_Animator.SetBool("IsMove", true);
                }
            }
        }
    }

    public void IdleUpdate()
    {
        if (m_IdleTime >= 0.0f)
        {
            m_IdleTime -= Time.deltaTime;
            if (m_IdleTime <= 0.0f)
            {
                m_IdleTime = Random.Range(2.0f, 3.0f);
                m_Monstate = MonsterState.PATROL;
                m_Animator.SetBool("IsMove", true);

                if (MoveRight)
                    this.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
                else
                    this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            }
        }

        if (m_CalcVec.magnitude <= m_ChaseDistance)
        {
            m_Monstate = MonsterState.CHASE;
            m_Animator.SetBool("IsMove", true);

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

                m_Animator.SetBool("IsMove", false);
                m_Monstate = MonsterState.IDLE;
            }
        }

        if (m_CalcVec.magnitude <= m_ChaseDistance)
        {
            m_Monstate = MonsterState.CHASE;
            m_Animator.SetBool("IsMove", true);
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

        if (m_CalcVec.magnitude <= m_AttackDistance)
        {
            m_Monstate = MonsterState.ATTACK;
            SoundMgr.Instance.PlayGUISound("WZombie_Idle", 0.3f);
            m_Animator.SetBool("CanAttack", true);
            m_Animator.SetBool("IsMove", false);
        }

        if (m_ChaseDistance * 1.5f < m_CalcVec.magnitude)
        {
            m_Monstate = MonsterState.PATROL;
        }
    }

    /// <summary>
    /// 사또가 몬스터 소환시 호출할것
    /// </summary>
    public void SpawnFunc()
    {
        m_Monstate = MonsterState.CHASE;
    }

    public void AttackUpdate()
    {
        if (attackDealy_2 >= 0.0f)
        {
            attackDealy_2 -= Time.deltaTime;
        }
        else
        {
            ChangeRotate2();

            if (m_CalcVec.magnitude >= m_AttackDistance)
            {
                m_Animator.SetBool("CanAttack", false);
                m_Animator.SetBool("IsMove", true);
                m_Monstate = MonsterState.CHASE;
            }

            //공격기능
            if (m_CurattackDelay >= 0.0f)
            {
                m_CurattackDelay -= Time.deltaTime;
                if (m_CurattackDelay <= 0.0f)
                {
                    m_CurattackDelay = m_AttackDelay;
                    GameObject marble = Instantiate(WeaponPrefab);
                    SoundMgr.Instance.PlayGUISound("WZombie_Spit", 0.7f);
                    marble.transform.position = weaponPos.position;
                    marble.transform.eulerAngles = this.transform.eulerAngles;
                    marble.GetComponent<FoxMarble>().SetRightBool(MonRight);
                    m_Animator.SetTrigger("Attack");
                    attackDealy_2 = 1.0f;
                    Destroy(marble, 3.0f);
                }
            }
        }

    }
}