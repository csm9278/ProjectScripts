using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    WaitForSeconds checkEndDelay = new WaitForSeconds(0.3f);

    public Button[] SkillBtns;
    public Sprite[] SKillCostImg;

    public GameObject sceneChangerIn;
    public GameObject sceneChangerOut;

    float soundDelay = -0.1f;

    float battleWinTimer = -.5f;
    float battleLoseTimer = -.5f;
    bool battleWin = false;
    bool battleLose = false;
    public Text ExpText;
    public GameObject ExpGage;

    public GameObject pausePanel;
    public Button pauseBtn;

    MonsterCtrl monsterCtrl;
    HeroStatueMgr heroStatmgr;

    public GameObject BattleEndPanel;
    public GameObject PlayerDiePanel;

    //스킬 코스트 관련 변수
    int SkillCost;
    public Text SkillCostText;
    public Button NextTurnBtn;

    //UI및 캐릭터 이미지 이동
    public GameObject heroStatusUI;
    public GameObject monsterStatusUI;

    //적 행동 관련
    float EnemyActionDelay = -0.5f;
    public GameObject EnemyAttackInfoObj;
    public Text EnemyAttackInfoText;
    string EnemyAttackInfoString;
    float heroActionDelay = -0.5f;

    bool heroTurn = true;
    bool enemyDelay = true; //디버프 맞고 행동하기위해서

    [Header("--- Debuff Node---")]
    public GameObject heroDebuffGrid;
    public GameObject EnemydebuffGrid;
    public GameObject debuffNode;
    public List<GameObject> HerodebuffNodeList;
    public List<GameObject> EnemydebuffNodeList;

    [Header("--- StateInfo ---")]
    public Text StateText;

    [Header("--- ImageChanger ---")]
    public Sprite[] backSpriets;
    public Image backImg;
    public Image heroImg;
    public Sprite[] heroSpirtes;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        soundDelay = 1.0f;
        sceneChangerOut.GetComponent<SceneChanger>().ChangeStart();

        if (GlobalData.heroType == GlobalData.HeroType.Warrior)
            heroImg.sprite = heroSpirtes[0];
        else if (GlobalData.heroType == GlobalData.HeroType.Assasin)
            heroImg.sprite = heroSpirtes[1];
        else if (GlobalData.heroType == GlobalData.HeroType.Mage)
            heroImg.sprite = heroSpirtes[2];

        int backrand = Random.Range(0, backSpriets.Length - 1);
        backImg.sprite = backSpriets[backrand];

        heroImg.sprite = heroSpirtes[(int)GlobalData.heroType];

        SpawnMonster();

        SkillBtnInit();

        StateText.text = "<color=yellow>플레이어 턴!!</color>";

        SkillCost = GlobalData.hero.ActivePoint;
        monsterCtrl = GameObject.FindObjectOfType<MonsterCtrl>();
        heroStatmgr = GameObject.FindObjectOfType<HeroStatueMgr>();
        heroStatmgr._battlemgr = this;
        


        ExpText.text = "Exp x" + monsterCtrl.mondata.Lv;


        if (pauseBtn != null)
            pauseBtn.onClick.AddListener(() =>
            {
                pausePanel.SetActive(true);
            });

        if (NextTurnBtn != null)
            NextTurnBtn.onClick.AddListener(() =>
            {
                NextTurnBtn.interactable = false;

                TurnEnd();
                AddStateText("<color=red>적의 턴!!</color>");
            });
 
        HerodebuffNodeList = new List<GameObject>();
        EnemydebuffNodeList = new List<GameObject>();
        SkillCostText.text = GlobalData.hero.ActivePoint.ToString();

        Debug.Log(CheckEnd());
        StartCoroutine(CheckEnd());
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        SoundDelayFunc();

        if(battleLoseTimer >= 0.0f)
        {
            battleLoseTimer -= Time.deltaTime;
            if (battleLoseTimer <= 0.0f)
                battleLose = true;
        }

        if(battleWinTimer >= 0.0f)
        {
            battleWinTimer -= Time.deltaTime;
            if (battleWinTimer <= 0.0f)
                battleWin = true;

        }

        if(Input.GetMouseButtonUp(0))
        {
            if (battleLose)
            {
                SoundMgr.Instance.PlayEffSound("NextScene");
                GlobalData.Reset();
                sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("TitleScene");
                battleLose = false;
            }

            if (battleWin)
            {
                SoundMgr.Instance.PlayEffSound("NextScene");
                if(GlobalData.isLevelUp)
                {
                    sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("EventScene");
                    GlobalData.isLevelUp = false;
                }
                else
                    sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("StageScene");
                battleWin = false;

            }
        }

        //적 행동 관련
        if(EnemyActionDelay >= 0.0f && !heroTurn)
        {
            EnemyActionDelay -= Time.deltaTime;
            if (EnemyActionDelay <= 0.0f)
            {
                if(enemyDelay)
                {
                    monsterCtrl.DebuffEff();
                    DecreaseOneDebuffNode(false, DebuffNodeMgr.DebuffType.Poison);

                    EnemyActionDelay = 1.5f;
                    enemyDelay = false;
                    return;
                }

                if (BattleEndCheck())
                    return;

                monsterCtrl.MonSkillEff(heroStatmgr, ref EnemyAttackInfoString, monsterCtrl.debuffDic.ContainsKey("약화"));
                EnemyAttackInfoText.text = EnemyAttackInfoString;
                EnemyAttackInfoObj.SetActive(true);
                heroActionDelay = 2.0f;
                enemyDelay = true;

                monsterCtrl.LateDebuffEff();
                DecreaseOneDebuffNode(false, DebuffNodeMgr.DebuffType.Weak);
            }
        }

        if (heroActionDelay >= 0.0f)
        {
            heroActionDelay -= Time.deltaTime;
            if (heroActionDelay <= 0.0f)
            {
                TurnEnd();
                EnemyAttackInfoObj.SetActive(false);

                AddStateText("<color=yellow>플레이어 턴!!</color>");

                heroStatmgr.DebuffEff();
                DecreaseOneDebuffNode(true, DebuffNodeMgr.DebuffType.Poison);

                if (BattleEndCheck())
                    return;

                //DecreaseDebuffNodeFunc(true);
            }
        }
    }

    /// <summary>
    /// 스킬 보유 개수만큼 버튼 Init하는 단계
    /// </summary>
    void SkillBtnInit()
    {
        for(int i =0; i < GlobalData.hero.Skills.Length; i++)
        {
            if (GlobalData.hero.Skills[i] == null)
                continue;

            int idx = i;
            SkillBtns[i].gameObject.SetActive(true);
            SkillBtns[i].GetComponentInChildren<Text>().text = GlobalData.hero.SkillInfo[i];
            SkillBtns[i].GetComponentsInChildren<Image>()[1].sprite = SKillCostImg[GlobalData.hero.SkillCost[idx] - 1];
            SkillBtns[idx].onClick.AddListener(()=>
            {
                if (GlobalData.hero.SkillCost[idx] > SkillCost)
                    return;

                if(heroStatmgr.debuffDic.ContainsKey("화염"))
                {
                    AddStateText("<color=red>화염 상태로 인해 1 데미지를 입었다.</color>");
                    heroStatmgr.SetHp(1);
                    heroStatmgr.HeroStatusInit();
                    heroStatmgr.DecreaseDebuff("화염");
                    DecreaseOneDebuffNode(true, DebuffNodeMgr.DebuffType.Fire);
                    SoundMgr.Instance.PlayEffSound("fire", 0.4f);
                }

                GlobalData.hero.Skills[idx](monsterCtrl, heroStatmgr.debuffDic.ContainsKey("약화"));
                SkillCost -= GlobalData.hero.SkillCost[idx];
                SkillCostText.text = SkillCost.ToString();
                BattleEndCheck();
                heroStatmgr.HeroStatusInit();
            });
        }
    }

    void SoundDelayFunc()
    {
        if (soundDelay >= 0.0f)
        {
            soundDelay -= Time.deltaTime;
            if (soundDelay <= 0.0f)
                SoundMgr.Instance.PlayBGM("BattleSound");
        }
    }

    /// <summary>
    ///  몬스터 스폰 함수
    /// </summary>
    void SpawnMonster()
    { 
        if (GlobalData.stageLevel == 1)
        {
            int rand = Random.Range(0, GlobalData.lv1Count);
            GameObject obj = Instantiate(GlobalData.monsterListLv1[rand]);
            obj.transform.localPosition = new Vector2(6.3f, 2.8f);
            GlobalData.monsterListLv1.RemoveAt(rand);
            GlobalData.lv1Count--;
            Debug.Log(GlobalData.lv1Count);
            if (GlobalData.lv1Count < 3)
            {
                GlobalData.stageLevel++;
            }
        }
        else if(GlobalData.stageLevel == 2)
        {
            int rand = Random.Range(0, GlobalData.lv2Count);
            GameObject obj = Instantiate(GlobalData.monsterListLv2[rand]);
            obj.transform.localPosition = new Vector2(6.3f, 2.8f);
            GlobalData.monsterListLv2.RemoveAt(rand);
            GlobalData.lv2Count--;
            if (GlobalData.lv2Count < 3)
                GlobalData.stageLevel++;
        }
        else if (GlobalData.stageLevel == 3)
        {
            int rand = Random.Range(0, GlobalData.lv3Count);
            GameObject obj = Instantiate(GlobalData.monsterListLv3[rand]);
            obj.transform.localPosition = new Vector2(6.3f, 2.8f);
            GlobalData.monsterListLv3.RemoveAt(rand);
            GlobalData.lv3Count--;
            if (GlobalData.lv3Count < 2)
                GlobalData.stageLevel++;
        }
        else if (GlobalData.stageLevel == 4)
        {
            int rand = Random.Range(0, GlobalData.lv4Count);
            GameObject obj = Instantiate(GlobalData.monsterListLv4[rand]);
            obj.transform.localPosition = new Vector2(6.3f, 2.8f);
            GlobalData.monsterListLv4.RemoveAt(rand);
            GlobalData.lv4Count--;
            if (GlobalData.lv4Count < 1)
                GlobalData.stageLevel++;
        }
        else if(GlobalData.stageLevel == 5)
        {
            backImg.sprite = backSpriets[backSpriets.Length - 1];
            int rand = Random.Range(0, GlobalData.lv5Count);
            GameObject obj = Instantiate(GlobalData.monsterListLv5[rand]);
            obj.transform.localPosition = new Vector2(6.3f, 2.8f);
            GlobalData.monsterListLv5.RemoveAt(rand);
            GlobalData.lv5Count--;
            if (GlobalData.lv5Count <= 0)
                GlobalData.stageLevel++;
        }
    }

    /// <summary>
    /// 지속적으로 배틀 종료상태인지 감시하는 옵저버 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckEnd()
    {
        while(true)
        {
            yield return checkEndDelay;


            if (GlobalData.hero.curHp <= 0.0f)
            {
                Debug.Log("플레이어 사망");
                battleLoseTimer = 1.0f;
                PlayerDiePanel.gameObject.SetActive(true);
                PlayerDiePanel.GetComponentInChildren<Text>().text = GlobalData.heroName + " 사망...";

                yield break;
            }

            if (monsterCtrl.stmgr.curHp <= 0)
            {
                if (GlobalData.isBossStage)
                {
                    SoundMgr.Instance.PlayBGM("");
                    SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                    sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("ClearScene");
                    yield break;
                }

                SoundMgr.Instance.PlayBGM("WinSoundBgm");
                BattleEndPanel.SetActive(true);
                StartCoroutine(ExpGage.GetComponent<ExpBarManager>().GetExpCo(monsterCtrl.mondata.Lv));
                battleWinTimer = 2.0f;

                yield break;
            }
        }

    }

    /// <summary>
    /// 턴 종료 함수
    /// </summary>
    void TurnEnd()
    {
        heroTurn = !heroTurn;

        StateText.text = "";

        if(heroTurn)
        {
            for(int i = 0; i < SkillBtns.Length; i++)
            {
                if (SkillBtns[i].gameObject.activeSelf == false)
                    continue;

                if (GlobalData.hero.curHp <= 0)
                    return;

                SkillBtns[i].GetComponent<Animator>().SetTrigger("InTrigger");
            }
            SkillCost = GlobalData.hero.ActivePoint;    //액티브 포인트 충전
            SkillCostText.text = SkillCost.ToString();

            NextTurnBtn.interactable = true;

        }
        else
        {
            for (int i = 0; i < SkillBtns.Length; i++)
            {
                if (SkillBtns[i].gameObject.activeSelf == false)
                    continue;

                SkillBtns[i].GetComponent<Animator>().SetTrigger("QuitTrigger");
            }

            heroStatmgr.DecreaseDebuff("약화");
            DecreaseOneDebuffNode(true, DebuffNodeMgr.DebuffType.Weak);

            //디버프가 없을시 바로 공격
            if (monsterCtrl.debuffDic.ContainsKey("독") == false)
                enemyDelay = false;

            EnemyActionDelay = 1.5f;
        }
        

    }

    /// <summary>
    /// 배틀이 끝났는지 체크하는 함수
    /// </summary>
    /// <returns></returns>
    bool BattleEndCheck()
    {
        if(GlobalData.hero.curHp <= 0.0f)
        {
            //Debug.Log("플레이어 사망");
            battleLoseTimer = 1.0f;
            PlayerDiePanel.gameObject.SetActive(true);
            PlayerDiePanel.GetComponentInChildren<Text>().text = GlobalData.heroName + " 패배...";

            return true;
        }

        if(monsterCtrl.stmgr.curHp <= 0)
        {
            if (GlobalData.isBossStage)
            {
                SoundMgr.Instance.PlayBGM("");
                SoundMgr.Instance.PlayEffSound("NextScene", 0.2f);
                sceneChangerIn.GetComponent<SceneChanger>().ChangeStart("ClearScene");
                return false;
            }

            SoundMgr.Instance.PlayBGM("WinSoundBgm");
            BattleEndPanel.SetActive(true);
            battleWinTimer = 3.0f;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 디버프 노드를 추가해주는 함수
    /// </summary>
    public void debuffNodeFunc(DebuffNodeMgr.DebuffType type, int count, bool isHero = false)
    {
        DebuffNodeMgr mgr;
        GameObject node;
        if (isHero)
        {
            node = Instantiate(debuffNode, heroDebuffGrid.transform);
        }
        else
        {
            node = Instantiate(debuffNode, EnemydebuffGrid.transform);
        }
        mgr = node.GetComponent<DebuffNodeMgr>();
        mgr.SetDebuff(type, count);

        if(isHero)
        {
            HerodebuffNodeList.Add(node);
            mgr.ListIndex = HerodebuffNodeList.Count - 1;
        }
        else
        {
            EnemydebuffNodeList.Add(node);
            mgr.ListIndex = EnemydebuffNodeList.Count - 1;
        }
    }

    public void AddDebuffNodeCountFunc(DebuffNodeMgr.DebuffType type, int count, bool isHero = false)
    {
        if(isHero)
        {
            for (int i = 0; i < HerodebuffNodeList.Count; i++)
            {
                if (HerodebuffNodeList[i].GetComponent<DebuffNodeMgr>().AddCount(type, count))
                    break;
            }
        }
        else
        {
            for (int i = 0; i < EnemydebuffNodeList.Count; i++)
            {
                if (EnemydebuffNodeList[i].GetComponent<DebuffNodeMgr>().AddCount(type, count))
                    break;
            }
        }
    }

    public void DecreaseDebuffNodeFunc(bool isHero = false)
    {
        if(isHero)
        {
            for (int i = 0; i < HerodebuffNodeList.Count; i++)
            {
                HerodebuffNodeList[i].GetComponent<DebuffNodeMgr>().DecreaseCount(isHero);
            }
        }
        else
        {
            for (int i = 0; i < EnemydebuffNodeList.Count; i++)
            {
                EnemydebuffNodeList[i].GetComponent<DebuffNodeMgr>().DecreaseCount(isHero);
            }
            Debug.Log("노드리스트 수 :" + EnemydebuffNodeList.Count);
        }
    }

    public void DecreaseOneDebuffNode(bool isHero = false, DebuffNodeMgr.DebuffType type = DebuffNodeMgr.DebuffType.Count)
    {
        if (isHero)
        {
            for (int i = 0; i < HerodebuffNodeList.Count; i++)
            {
                if(HerodebuffNodeList[i].GetComponent<DebuffNodeMgr>().debuffType == type)
                {
                    HerodebuffNodeList[i].GetComponent<DebuffNodeMgr>().DecreaseCount(isHero);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < EnemydebuffNodeList.Count; i++)
            {
                if (EnemydebuffNodeList[i].GetComponent<DebuffNodeMgr>().debuffType == type)
                {
                    EnemydebuffNodeList[i].GetComponent<DebuffNodeMgr>().DecreaseCount(isHero);
                    break;
                }
            }
        }
    }

    public void AddStateText(string inputText)
    {
        StateText.text += "\n" + inputText;
    }
}