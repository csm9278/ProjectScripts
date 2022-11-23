using UnityEngine;
using UnityEngine.UI;

public class BossMan : Monster
{
    public float m_DelayTime = 0.0f;
    bool m_IsRight = false;
    Vector2 attackSize = new Vector2(4, 3);

    float changeWeaponTime = 3.0f;

    //공격
    bool attackEnd = false;

    //소환 몬스터
    [Header("--- SpawnMonster ---")]
    [SerializeField] GameObject humanManMon;
    [SerializeField] GameObject humanWomanMon;
    [SerializeField] Transform manSpawnPos;
    [SerializeField] Transform womanSpawnPos;
    [SerializeField] GameObject monsterPotal;
    [SerializeField] Transform potalPos;
    [SerializeField] Image HPBar = null;
    [SerializeField] Image HPBack = null;
    [SerializeField] Image Icon = null;
    bool firstSpawn = false, secondSpawn = false;

    //보스소환
    [Header("--- Boss ---")]
    public Transform bossSpawnPos;
    public GameObject bossPrefab;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(originPos, (attackPos.position - originPos));
        Gizmos.DrawCube(attackPos.position, new Vector2(4, 3));
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        originPos = new Vector3(this.transform.position.x, attackPos.position.y, 0.0f);

        CheckDistanceFromPlayer();
        AiUpdate();
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

            if (m_CalcVec.magnitude <= m_ChaseDistance) // 일정거리 안에 들어올 시 변신
            {
                m_Monstate = MonsterState.PATROL;
                SoundMgr.Instance.PlayGUISound("BossMan_Chng", 1.0f);
                m_Animator.SetBool("IsChange", true);
            }
        }
        else if(m_Monstate == MonsterState.PATROL)  //여기서는 Patrol이 변신 과정
        {
            if (changeWeaponTime >= 0.0f)
            {
                changeWeaponTime -= Time.deltaTime;
                if (changeWeaponTime <= 0.0f)
                {
                    m_Animator.SetBool("IsMove", true);
                    m_Monstate = MonsterState.CHASE;
                }
            }
            else
            {
                m_Monstate = MonsterState.CHASE;
                m_Animator.SetBool("IsMove", true);
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
                int randMotion = Random.Range(0, 2);
                if (randMotion == 0)
                {
                    m_Animator.SetBool("IsAttack", true);
                    SoundMgr.Instance.PlayEffSound("BossManAtt1", 1.0f);
                }
                else
                {
                    m_Animator.SetBool("IsAttack2", true);
                    SoundMgr.Instance.PlayEffSound("BossManAtt2", 1.0f);
                }
            }
        }
        else if (m_Monstate == MonsterState.SKILL)
        {

        }
        else if (m_Monstate == MonsterState.ATTACK)
        {
            if (m_DelayTime >= 0.0f)
            {
                m_DelayTime -= Time.deltaTime;
                if (m_DelayTime <= 0.0f)
                {
                    ChangeRotate2();
                    if (m_CalcVec.magnitude >= m_AttackDistance)
                    {
                        m_Monstate = MonsterState.CHASE;
                        m_Animator.SetBool("IsAttack2", false);
                        m_Animator.SetBool("IsAttack", false);
                    }
                }
            }
            else if (m_DelayTime <= 0.0f && m_Animator.GetBool("IsAttack") == false && m_Animator.GetBool("IsAttack2") == false)
                m_Monstate = MonsterState.CHASE;
        }
        else if (m_Monstate == MonsterState.DIE)
        {
            SoundMgr.Instance.PlayEffSound("BossManDie", 1.0f);
            m_Animator.SetTrigger("DieTrigger");
            m_Monstate = MonsterState.CORPSE;
            HPBack.gameObject.SetActive(false);
            HPBar.gameObject.SetActive(false);
            Icon.gameObject.SetActive(false);
            CoinDrop();
        }
        else if (m_Monstate == MonsterState.CORPSE)
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
                }
            }
        }
    }


    /// <summary>
    /// 몬스터 소환 스킬 애니메이터 이벤트 함수 호출
    /// </summary>
    void SpawnMonster()
    {
        GameObject potal = Instantiate(monsterPotal);
        potal.transform.position = potalPos.position;
        Destroy(potal, 2.0f);
        SoundMgr.Instance.PlayEffSound("BossManPortal", 1.0f);
        SpawnMob();
    }

    void SpawnMob()
    {
        GameObject man = Instantiate(humanManMon);
        man.transform.position = manSpawnPos.position;
        man.GetComponent<HumanManMonster>().SpawnFunc();

        GameObject woman = Instantiate(humanWomanMon);
        woman.transform.position = womanSpawnPos.position;
        woman.GetComponent<HumanWomanMonster>().SpawnFunc();
    }

    public void SpawnBoss()
    {
        GameObject boss = Instantiate(bossPrefab);
        boss.transform.position = bossSpawnPos.transform.position;
        if (this.transform.eulerAngles.y == 180)
            boss.transform.eulerAngles = new Vector3(0, 0, 0);
        else
            boss.transform.eulerAngles = new Vector3(0, 180.0f, 0);
        this.m_Animator.enabled = false;
        Destroy(this.gameObject);
    }

    public override void TakeDamage(int WeaponState)
    {
        base.TakeDamage(WeaponState);
        HPBar.fillAmount = m_CurHP / m_MaxHP;
        if (m_CurHP <= m_MaxHP * 0.7f && !firstSpawn)
        {
            m_Animator.SetTrigger("SpawnMonster");
            firstSpawn = true;
        }
        
        if(m_CurHP <= m_MaxHP * 0.4f && !secondSpawn)
        {
            m_Animator.SetTrigger("SpawnMonster");
            secondSpawn = true;
        }
    }

    void Attackend()
    {
        m_DelayTime = 1.0f;
    }

    public void BossManAttackEff()
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