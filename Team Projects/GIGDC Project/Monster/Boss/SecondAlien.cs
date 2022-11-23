using UnityEngine;
using UnityEngine.UI;

public class SecondAlien : Monster
{
    public enum BossStage
    {
        Stage1_2,
        Stage1_3
    }

    public BossStage bossStage = BossStage.Stage1_2;

    public float m_DelayTime = 0.0f;
    bool m_IsRight = false;
    Vector2 attackSize = new Vector2(7, 4);

    float changeWeaponTime = 3.0f;

    //변환 및 사망
    bool isChange = false;
    bool changing = false;
    bool isDisappear = false;
    float dieAlpha = 1.0f;
    Color dieColor = Color.white;
    SpriteRenderer _spRenderer;
    public GameObject Protal;

    //HpBar
    [SerializeField] Image HPBar = null;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
        
        m_Monstate = MonsterState.CHASE;
        m_Animator.SetBool("IsMove", true);

        _spRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(originPos, (attackPos.position - originPos));
        Gizmos.DrawCube(attackPos.position, new Vector2(7, 4));
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

        if (m_Monstate == MonsterState.CHASE)
        {
            if (m_CalcVec.x >= 0.1f)
            {
                m_Rb.transform.position += Vector3.right * m_MoveSpeed * Time.deltaTime;
                this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            }
            else if (m_CalcVec.x <= -0.1f)
            {
                m_Rb.transform.position += Vector3.left * m_MoveSpeed * Time.deltaTime;
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (m_CalcVec.magnitude <= m_AttackDistance)
            {
                m_Monstate = MonsterState.ATTACK;
                int randMotion = Random.Range(0, 3);
                if (randMotion == 0)
                {
                    m_Animator.SetBool("IsAttack", true);
                    SoundMgr.Instance.PlayEffSound("Alien_Att1", 0.8f);
                }
                else if (randMotion == 1)
                {
                    m_Animator.SetBool("IsAttack2", true);
                    SoundMgr.Instance.PlayEffSound("Alien_Att2", 0.8f);
                }
                else
                {
                    m_Animator.SetBool("IsAttack3", true);
                    SoundMgr.Instance.PlayEffSound("Alien_Att3", 0.8f);
                }
            }
        }
        else if (m_Monstate == MonsterState.ATTACK)
        {
            if (m_DelayTime >= 0.0f)
            {
                m_DelayTime -= Time.deltaTime;
                if (m_DelayTime <= 0.0f)
                {
                    ChangeRotate1();
                    if (m_CalcVec.magnitude >= m_AttackDistance)
                    {
                        m_Monstate = MonsterState.CHASE;
                        m_Animator.SetBool("IsAttack2", false);
                        m_Animator.SetBool("IsAttack3", false);
                        m_Animator.SetBool("IsAttack", false);
                    }
                }
            }
            else if (m_DelayTime <= 0.0f && m_Animator.GetBool("IsAttack") == false && m_Animator.GetBool("IsAttack2") == false)
                m_Monstate = MonsterState.CHASE;
        }
        else if (m_Monstate == MonsterState.DIE)
        {
            if(bossStage == BossStage.Stage1_3)
            {
                SoundMgr.Instance.PlayEffSound("Alien_Die", 1.0f);
                m_Animator.SetTrigger("DieTrigger");
                m_Monstate = MonsterState.CORPSE;

                //CoinDrop();
                //if (Protal != null)
                //{
                //    Protal.gameObject.tag = "Credit";
                //    if (StageMgr.Inst.InfoText != null)
                //    {
                //        StageMgr.Inst.InfoText.gameObject.SetActive(true);
                //        StageMgr.Inst.InfoTimer = 3.0f;
                //        StageMgr.Inst.InfoText.text = "포탈이 열렸습니다.";
                //    }
                //    Protal.gameObject.SetActive(true);
                //    Vector3 ProtalPos = new Vector3(361.5f, 3.93f, 0.0f);
                //    Protal.transform.position = ProtalPos;
                //    Instantiate(Protal);
                //}
            }
            else if(bossStage == BossStage.Stage1_2)
            {
                SoundMgr.Instance.PlayEffSound("Alien_Flee", 1.0f);
                m_Animator.SetTrigger("EscapeTrigger");
                m_Animator.SetBool("IsAttack2", false);
                m_Animator.SetBool("IsAttack3", false);
                m_Animator.SetBool("IsAttack", false);
                //if (Protal != null)
                //{
                //    if (StageMgr.Inst.InfoText != null)
                //    {
                //        StageMgr.Inst.InfoText.gameObject.SetActive(true);
                //        StageMgr.Inst.InfoTimer = 3.0f;
                //        StageMgr.Inst.InfoText.text = "포탈이 열렸습니다.";
                //    }
                //    Protal.gameObject.SetActive(true);
                //    Vector3 ProtalPos = new Vector3(483.45f, 9.4f, 0.0f);
                //    Protal.transform.position = ProtalPos;
                //    Instantiate(Protal);
                //}
                m_Monstate = MonsterState.CORPSE;
                //SoundMgr.Instance.PlayEffSound("Coin", 0.2f);
                //UIMgr.Inst.AddGold(10000);
                //BossDrop();
                //Destroy(this.gameObject, 5.0f);
            }

        }
        else if (m_Monstate == MonsterState.CORPSE)
        {
            if(bossStage == BossStage.Stage1_2)
            {
                if(isDisappear)
                {
                    m_Rb.transform.position += Vector3.right * m_MoveSpeed * 5 * Time.deltaTime;
                    this.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
                }

            }
            else if(bossStage == BossStage.Stage1_3)
            {
                if (isDisappear)
                {
                    dieAlpha -= Time.deltaTime;
                    dieColor = new Color(1, 1, 1, dieAlpha);
                    _spRenderer.color = dieColor;

                    if (dieAlpha <= 0.05f)
                        Destroy(this.gameObject);
                }
            }
        }
        else if (m_Monstate == MonsterState.Hitted)
        {
            m_Monstate = MonsterState.CHASE;

            //m_Animator.SetBool("IsAttack", false);

            //if (HittedTIme >= 0.0f)
            //{
            //    HittedTIme -= Time.deltaTime;
            //    if (HittedTIme <= 0.0f)
            //    {
            //        m_Monstate = MonsterState.CHASE;
            //    }
            //}
        }
    }

    public override void TakeDamage(int WeaponState)
    {
        if (changing)
            return;

        base.TakeDamage(WeaponState);

        HPBar.fillAmount = m_CurHP / m_MaxHP;

        if(bossStage == BossStage.Stage1_3)
        {
            if (isChange == false && m_CurHP <= m_MaxHP * 0.5f)
            {
                isChange = true;
                m_Animator.SetTrigger("Change");
                SoundMgr.Instance.PlayGUISound("Alien_CHNG", 1.0f);
            }
        }

    }

    void Attackend()
    {
        m_DelayTime = 1.0f;
    }

    public void SecondAlienAttackEff()
    {
        Vector3 attackdir = attackPos.position - originPos;

        Collider2D coll = Physics2D.OverlapBox(attackPos.position, attackSize * 0.5f, 0, playerMask);
        if (coll != null)
        {
            if (coll.gameObject.TryGetComponent(out playerTakeDmg))
            {
                playerTakeDmg.P_TakeDamage(m_Atk);
            }
        }
        else
            Debug.Log("검출안됨");
    }

    /// <summary>
    /// 애니메이션 이벤트로 호출 이게 호출되면 데미지 리턴
    /// </summary>
    void ChangeStart()
    {
        m_Animator.SetBool("IsAttack2", false);
        m_Animator.SetBool("IsAttack3", false);
        m_Animator.SetBool("IsAttack", false);
        changing = true;
        m_Monstate = MonsterState.IDLE;
    }

    void ChangeEnd()
    {
        changing = false;
        m_Monstate = MonsterState.CHASE;

    }

    /// <summary>
    /// 애니메이션 이벤트로 호출 이게 호출되면 시체가 사라진다..
    /// </summary>
    void CorpseEff()
    {
        isDisappear = true;
    }

}