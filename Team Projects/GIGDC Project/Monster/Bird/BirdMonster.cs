using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BodyAttack
{
    ATTACK_BEFORE,
    ATTACK,
    ATTACK_AFTER
}

public class BirdMonster : Monster
{
    float m_DelayTime = 0.0f;
    float m_DistanceFromPlayer = 0.0f;
    bool m_IsRight = false;

    bool m_IsFind = false;  //�÷��̾� Ž��
    float m_IdleTimer = 5.0f;   // Idle ���·� ���ư��� ���� �ð�
    public float m_FindDist = 5.0f;
    bool IsRight = false;   //���� Ȯ�ο� ����'
    bool isAttack = false; // ����������?

    public LayerMask platformmask;
    //���� ���� ���
    Vector2 m_FirstVec = Vector2.zero;
    float m_CurHeight = 0.0f;
    float m_MaxHeight = 0.0f;
    public float m_FlyHeight = 2.5f;
    public float m_FlySpeed = 0.0f;
    public float m_IdleDistance = 0.0f;

    //Chase���� ���� ����
    public float m_ChaseDelay = 2.0f;
    public Vector3 m_ChaseVec = Vector3.zero;

    //Attack���� ���� ����
    float m_AttackDelay = 5.0f;
    [SerializeField]BodyAttack m_BodyAttackState = BodyAttack.ATTACK_BEFORE;
    Vector3 m_AttackVec = Vector3.zero;
    Vector3 m_AttackCurVec = Vector3.zero;
    public Sprite[] m_AttackImgs;
    SpriteRenderer m_SpRenderer;


    public FlyMonsterState m_FlyMonState;

    // Start is called before the first frame update
    void Start()
    {
        InitMonster();
        m_FlyMonState = FlyMonsterState.IDLE;
        m_FirstVec = this.transform.position;
        m_CurHeight = m_FirstVec.y;
        m_MaxHeight = m_CurHeight + m_FlyHeight;
        m_SpRenderer = GetComponent<SpriteRenderer>();
    }

// Update is called once per frame
void Update()
    {
        CheckDistanceFromPlayer();
        MonAiUpdate();
    }

    //�÷��̾���� �Ÿ� ���ϱ� �Լ�
    void CheckDistanceFromPlayer()
    {
        m_CalcVec = m_Player.transform.position - this.transform.position;
        m_DistanceFromPlayer = m_CalcVec.magnitude;
        m_Animator.SetFloat("DistanceFromPlayer", m_DistanceFromPlayer);
    }

    void MonAiUpdate()
    {
        if (m_Player == null)
        {
            Debug.Log("Player Null");
            return;
        }

        if (m_FlyMonState == FlyMonsterState.IDLE)
        {
            if(m_DistanceFromPlayer <= m_FindDist)    // �÷��̾ �����Ÿ� ������ ���ý� FLY���·� ��ȯ
            {
                SoundMgr.Instance.PlayGUISound("Bird_Find", 0.8f);
                m_FlyMonState = FlyMonsterState.FLY;
                m_IsFind = true;
                m_Animator.SetBool("IsFind", m_IsFind);
            }

            if(this.transform.position.y >= m_FirstVec.y)   //IDle ���� ������ ���� �������� �ڵ�
            {
                m_Rb.transform.position += Vector3.down * m_FlySpeed * Time.deltaTime;

                if (this.transform.position.y <= m_FirstVec.y)   //���� ������ ���� IDle Animation�� ����ϱ� ���� �ڵ�
                {
                    m_Animator.SetBool("IsFind", m_IsFind);
                }
            }


        }
        else if (m_FlyMonState == FlyMonsterState.FLY)
        {
            //���� ��ȯ �ڵ�
            ChangeRotate();

            if (this.transform.position.y <= m_MaxHeight)    //�ִ� ���� ���̺��� ���ٸ� ����
            {
                m_Rb.transform.position += Vector3.up * m_FlySpeed * Time.deltaTime;

                if (m_CalcVec.x >= 0.1f)
                {
                    m_Rb.transform.position += Vector3.left * Time.deltaTime * 2.0f;
                }
                else if (m_CalcVec.x <= -0.1f)
                {
                    m_Rb.transform.position += Vector3.right * Time.deltaTime * 2.0f;
                }
            }
            else // ü�̽� �׽�Ʈ
            {
                m_FlyMonState = FlyMonsterState.CHASE;
            }
        }
        else if (m_FlyMonState == FlyMonsterState.PATROL)
        {

        }
        else if (m_FlyMonState == FlyMonsterState.CHASE)
        {
            //������ȯ �ڵ�
            ChangeRotate();

            if (m_ChaseDelay >= 0.0f)    //�i�� ��ġ Ž��
            {
                m_ChaseDelay -= Time.deltaTime;
                if(m_ChaseDelay <= 0.0f)
                {
                    m_ChaseVec = m_Player.transform.position;

                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, Mathf.Infinity, platformmask);
                    if (hit)
                    {
                        Debug.Log("hit");
                        m_MaxHeight = hit.point.y + m_FlyHeight;
                    }
                    else
                        Debug.Log("nohit");

                    m_ChaseDelay = 2.0f;
                }
            }

            if(m_ChaseVec.x >= this.transform.position.x)
            {
                m_Rb.transform.position += Vector3.right * Time.deltaTime * 5.0f;
            }
            else
            {
                m_Rb.transform.position += Vector3.left * Time.deltaTime * 5.0f;
            }

            if(m_AttackDelay >= 0.0f)
            {
                m_AttackDelay -= Time.deltaTime;
                if(m_AttackDelay <= 0.0f)
                {
                    m_AttackDelay = 1.0f;   //���ݻ��� ���� �� Attack _BEFORE ������ ���� �ð�
                    m_Animator.enabled = false;
                    m_FlyMonState = FlyMonsterState.ATTACK;
                    m_BodyAttackState = BodyAttack.ATTACK_BEFORE;
                }
            }

        }
        else if (m_FlyMonState == FlyMonsterState.ATTACK)
        {
            if(m_BodyAttackState == BodyAttack.ATTACK_BEFORE)
            {
                m_SpRenderer.sprite = m_AttackImgs[0];
                isAttack = true;
                if(m_AttackDelay >= 0.0f)
                {
                    if(m_IsRight)
                        m_Rb.transform.position += (new Vector3(1, 1, 0)) * Time.deltaTime;
                    else
                        m_Rb.transform.position += (new Vector3(-1, 1, 0)) * Time.deltaTime;

                    m_AttackDelay -= Time.deltaTime;
                    if(m_AttackDelay <= 0.0f)
                    {
                        m_AttackVec = m_Player.transform.position;
                        m_AttackVec.y += 1.5f;
                        m_BodyAttackState = BodyAttack.ATTACK;
                        m_AttackDelay = 5.0f;
                        m_SpRenderer.sprite = m_AttackImgs[1];
                        ChangeRotate();
                    }
                }
            }
            else if(m_BodyAttackState == BodyAttack.ATTACK)
            {
                m_AttackCurVec = m_AttackVec - this.transform.position;
                m_Rb.transform.position += m_AttackCurVec.normalized * m_FlySpeed * 8.0f * Time.deltaTime;
                if(m_AttackCurVec.magnitude <= 1.0f)
                {
                    m_BodyAttackState = BodyAttack.ATTACK_AFTER;
                }
            }
            else if(m_BodyAttackState == BodyAttack.ATTACK_AFTER)
            {
                m_Animator.enabled = true;
                m_FlyMonState = FlyMonsterState.FLY;
                isAttack = false;
                m_AttackDelay = 5.0f;
            }
        }
        else if (m_FlyMonState == FlyMonsterState.DIE)
        {
            m_Animator.enabled = true;
            isAttack = false;
            m_Animator.SetTrigger("DieTrigger");
            SoundMgr.Instance.PlayGUISound("Bird_Die", 0.6f);
            m_FlyMonState = FlyMonsterState.CORPSE;

            CoinDrop();
        }
        else if (m_FlyMonState == FlyMonsterState.CORPSE)
        {
            //-- �ƹ��͵� ����
        }
    }

    void ChangeRotate()
    {
        if (m_CalcVec.x >= 0.1f)
        {
            m_IsRight = true;
            this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
        }
        else if (m_CalcVec.x <= -0.1f)
        {
            m_IsRight = false;
            this.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttack)
            return;

        if (collision.TryGetComponent(out playerTakeDmg))
            playerTakeDmg.P_TakeDamage();
    }

    public override void TakeDamage(int WeaponState)
    {
        if (m_CurHP <= 0.0f)
            return;

        float a_DamVal = GlobalUserData.m_weaponDataList[WeaponState].m_WeaponDamage + (GlobalUserData.m_ItemDataList[5].m_CurItemCount * 5);
        int a_CritVal = GlobalUserData.m_weaponDataList[WeaponState].m_Critical + (GlobalUserData.m_ItemDataList[7].m_CurItemCount * 10);
        float a_CritDmg = a_DamVal + (a_DamVal * (GlobalUserData.m_weaponDataList[WeaponState].m_CriticalDmg + ((float)GlobalUserData.m_ItemDataList[6].m_CurItemCount * 0.05f)));

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
            Debug.Log("�����ǰ�");
        }

        if (m_CurHP <= 0)
        {
            m_CurHP = 0.0f;
            m_FlyMonState = FlyMonsterState.DIE;
            this.GetComponent<CircleCollider2D>().enabled = false;
            m_Rb.gravityScale = 1.5f;
        }
    }

}