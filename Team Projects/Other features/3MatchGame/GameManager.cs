using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int moveNumber = 10;
    public int clearMissionNumber = 3;

    public Text moveText;
    public Text missionText;
    public Image clearPanel;
    public Image failPanel;
    public Text failLeftText;

    public GameObject clearBeforeObj;   //스테이지 클리어 전 표시될 오브젝트
    public Text clearBeforeText;        //스테이지 클리어 전 출력될 텍스트
    public Text clearText;

    public Button RestartBtn;
    public Button QuitBtn;
    public bool isEnd = false;

    // 스코어
    public Text scoreText;
    public int score;
    public bool isUpScore = false;
    public Image starFillImg;
    public int perfectScore = 1000;
    public Image[] ClearStarImages;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        isEnd = false;
        moveText.text = "<size=30><color=#373737>Move</color></size><size=80><color=#388C00>\n" + moveNumber + "</color></size>";
        scoreText.text = "<color=#373737>Score : </color><color=#388C00>" + score.ToString() + "</color>";

        RestartBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene");
        });

        QuitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        scoreText.text = "<color=#373737>Score : </color><color=#388C00>" + score.ToString() + "</color>";
        starFillImg.fillAmount = (float)score / perfectScore;
    }    

    public void UseMove()
    {
        moveNumber--;
        moveText.text = "<size=30><color=#373737>Move</color></size><size=80><color=#388C00>\n" + moveNumber + "</color></size>";
    }

    public void MissionCount()
    {
        clearMissionNumber--;
        missionText.text = "x" + clearMissionNumber;
    }

    public void WinLoseCheck()
    {
        if(clearMissionNumber <= 0)
        {
            isEnd = true;
            StartCoroutine(MoveScoreCo());
        }
        else
        {
            if (moveNumber <= 0)
            {
                isEnd = true;
                failLeftText.text = clearMissionNumber + "<size=45> Left</size>";
                failPanel.gameObject.SetActive(true);
                RestartBtn.gameObject.SetActive(true);
                QuitBtn.gameObject.SetActive(true);
            }
        }
    }

    // 남은 무브에 따라 점수 추가 부여
    IEnumerator MoveScoreCo()
    {
        clearBeforeText.text = "Stage Clear!!";
        clearBeforeObj.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        clearBeforeText.text = "Move Bonus!!";

        while(moveNumber > 0)
        {
            UseMove();
            AddScore(1000);
            yield return new WaitForSeconds(0.25f);
        }

        clearBeforeObj.SetActive(false);

        clearText.text = "Clear~!";
        clearPanel.gameObject.SetActive(true);
        RestartBtn.gameObject.SetActive(true);
        QuitBtn.gameObject.SetActive(true);
        StartCoroutine(ClearFillStar());

        yield break;
    }

    IEnumerator ClearFillStar()
    {
        yield return new WaitForSeconds(0.5f);

        if(score >= perfectScore * 0.3f)    //1성
        {
            while(true)
            {
                ClearStarImages[0].fillAmount += Time.deltaTime;

                if (ClearStarImages[0].fillAmount >= 1)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }

        if (score >= perfectScore * 0.6f)    //2성
        {
            while (true)
            {
                ClearStarImages[1].fillAmount += Time.deltaTime * 2;

                if (ClearStarImages[1].fillAmount >= 1)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }
        else
            yield break;

        if (score >= perfectScore)    //1성
        {
            while (true)
            {
                ClearStarImages[2].fillAmount += Time.deltaTime;

                if (ClearStarImages[2].fillAmount >= 1)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }
        else
            yield break;

    }
}