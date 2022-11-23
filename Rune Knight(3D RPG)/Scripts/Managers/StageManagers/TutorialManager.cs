using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialState
    {
        Title,
        FirstMove,
        FirstAttack,
        LearnHeal,
        LearnFireBall,
        LearnWaterSkill,
        LearnGainSkill,
        Free,
        LastWave
    }
    
    GameObject player;
    PlayerMove playerMove;

    public GameObject[] playerSkill;

    public TutorialState tutorialState = TutorialState.Title;

    //all
    public GameObject infoObj;
    public Text infoText;
    float infoDelay = 3.0f;
    float curInfoDelay = 3.0f;

    //title
    public GameObject titleObj;
    public GameObject escPanel;
    float titleDelay = 2.0f;

    [Header("--- FirstMonsters---")]
    public GameObject[] firstMonsters;
    public GameObject learnSkillPanel;
    public GameObject Arrow;
    bool firstMonstersEnd = true;

    [Header("--- LastMonsters ---")]
    public Canvas uiCanvas;
    public GameObject[] lastMonsters;
    public GameObject clearPanel;
    bool lastMonstersEnd = true;
    public Button GoTitleBtn;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        GlobalData.gameStop = true;

        player = GameObject.Find("Player");
        playerMove = player.GetComponent<PlayerMove>();

        if (playerMove != null)
            playerMove.canMove = false;

        if(playerSkill.Length > 0)
        {
            for(int i = 0; i < playerSkill.Length; i++)
            {
                playerSkill[i].gameObject.SetActive(false);
            }
        }

        if (GoTitleBtn != null)
            GoTitleBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        switch(tutorialState)
        {
            case TutorialState.Title:
                TitleFunc();
                break;

            case TutorialState.Free:
                FreeFunc();
                break;

            case TutorialState.FirstAttack:
                FirstAttackFunc();
                break;

            case TutorialState.LearnHeal:
                LearnHealFunc();
                break;

            case TutorialState.LearnFireBall:
                LearnFireBallFunc();
                break;

            case TutorialState.LearnWaterSkill:
                LearnWaterSkillFunc();
                break;

            case TutorialState.LearnGainSkill:
                LearnGainSkillFunc();
                break;

            case TutorialState.LastWave:
                LastWaveFunc();
                break;
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(escPanel.activeSelf)
            {
                Time.timeScale = 1.0f;
                GlobalData.gameStop = false;
                escPanel.gameObject.SetActive(false);
            }
            else
            {
                Time.timeScale = 0.0f;
                GlobalData.gameStop = true;
                escPanel.gameObject.SetActive(true);
            }
        }
    }


    void TitleFunc()
    {
        if(titleDelay >= 0.0f)
        {
            titleDelay -= Time.deltaTime;
            if(titleDelay <= 0.0f)
            {
                titleObj.GetComponent<ChangeAlpha>().enabled = true;
            }
        }

        if(!titleObj.activeSelf)
        {
            playerMove.canMove = true;
            tutorialState = TutorialState.FirstMove;
            infoObj.SetActive(true);
            infoText.text = "���콺 ���� Ŭ������ �̵��غ�����.";
        }
    }

    void FreeFunc()
    {
        if(curInfoDelay >= 0.0f)
        {
            curInfoDelay -= Time.deltaTime;
            if(curInfoDelay <= 0.0f)
            {
                infoObj.SetActive(false);
            }
        }
    }

    void FirstAttackFunc()
    {
        StartCoroutine(CheckFirstMons());
        infoText.text = "���� ���콺 ���� Ŭ������ ������ ������.";

        if(!firstMonstersEnd)
        {
            tutorialState = TutorialState.LearnHeal;
            GlobalData.gameStop = true;
            playerSkill[0].gameObject.SetActive(true);
            infoText.text = "QŰ�� ���� ȸ�� ��ų�� ����� ������";
            Time.timeScale = 0;
        }
    }

    IEnumerator CheckFirstMons()
    {
        int idx;
        while (firstMonstersEnd)
        {
            idx = firstMonsters.Length;

            for(int i = 0; i < firstMonsters.Length; i++)
            {
                if (!firstMonsters[i].activeSelf)
                    idx--;
            }

            if (idx <= 0)
                firstMonstersEnd = false;

            yield return new WaitForSeconds(1.0f);
        }
    }

    void LearnHealFunc()
    {
        learnSkillPanel.SetActive(true);
        uiCanvas.sortingOrder = -1;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            GlobalData.gameStop = false;

            infoText.text = "��ų�� ��Ÿ���� ���� �Ŀ� �� ����� �� �ֽ��ϴ�.";
            curInfoDelay = infoDelay;

            Time.timeScale = 1;
            tutorialState = TutorialState.Free;
            learnSkillPanel.SetActive(false);
        }
    }

    void LearnFireBallFunc()
    {
        learnSkillPanel.SetActive(true);
        infoObj.SetActive(true);

        if (Input.GetKeyDown(KeyCode.W))
        {
            GlobalData.gameStop = false;

            infoText.text = "���콺 ���� Ŭ������ �߻��ϰ� ����Ŭ������ ����� �� �ֽ��ϴ�.";
            curInfoDelay = infoDelay;
            Time.timeScale = 1;
            tutorialState = TutorialState.Free;
            learnSkillPanel.SetActive(false);
        }
    }

    void LearnWaterSkillFunc()
    {
        learnSkillPanel.SetActive(true);
        infoObj.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            GlobalData.gameStop = false;

            infoText.text = "���콺 ���� Ŭ������ �߻��ϰ� ����Ŭ������ ����� �� �ֽ��ϴ�.";
            curInfoDelay = infoDelay;
            Time.timeScale = 1;
            tutorialState = TutorialState.Free;
            learnSkillPanel.SetActive(false);
        }
    }

    void LearnGainSkillFunc()
    {
        learnSkillPanel.SetActive(true);
        infoObj.SetActive(true);

        if (Input.GetKeyDown(KeyCode.R))
        {
            GlobalData.gameStop = false;

            infoText.text = "�������� ���� ������� �����մϴ�.\n" +
                            "���� �� ���� ������ ǥ�õ˴ϴ�";
            curInfoDelay = infoDelay;
            Time.timeScale = 1;
            tutorialState = TutorialState.LastWave;
            StartCoroutine(CheckLastMons());
            learnSkillPanel.SetActive(false);
        }
    }

    IEnumerator CheckLastMons()
    {
        int idx;
        while (lastMonstersEnd)
        {
            idx = lastMonsters.Length;

            for (int i = 0; i < lastMonsters.Length; i++)
            {
                if (!lastMonsters[i].activeSelf)
                    idx--;
            }

            if (idx <= 0)
                lastMonstersEnd = false;

            yield return new WaitForSeconds(1.0f);
        }
    }

    void LastWaveFunc()
    {
        if(!lastMonstersEnd)
        {
            uiCanvas.sortingOrder = 1;
            Time.timeScale = 0;
            GlobalData.StageLevelUp(1);
            clearPanel.SetActive(true);
            GlobalData.gameStop = true;
        }
    }
}