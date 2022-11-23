using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Manager : MonoBehaviour
{
    enum MonsterType
    {
        Skeleton,
        Ghoul,
        Count
    }

    public Transform[] SpawnPoses;
    public GameObject escPanel;

    [Header("--- Stage Level ---")]
    public Text stageLevelText;
    int stageLevel = 0;
    public float levelDisplayTime;
    float curDisplayTime;
    Color stageLevelTextColor;
    ChangeAlpha levelTextAlpha;

    [Header("--- Monster Spawn ---")]
    public GameObject[] monsters;
    int enemyRand;
    int posRand;
    List<GameObject> monList = new List<GameObject>();

    private void Start() => StartFunc();

    private void StartFunc()
    {
        GlobalData.gameStop = false;

        if (stageLevelText != null)
        {
            stageLevelTextColor = stageLevelText.color;
            levelTextAlpha = stageLevelText.GetComponent<ChangeAlpha>();
        }

        StageLevelUp();

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {

        if (curDisplayTime >= 0.0f)
        {
            curDisplayTime -= Time.deltaTime;
            if(curDisplayTime <= 0.0f)
            {
                levelTextAlpha.enabled = true;
                StartCoroutine(EnemySpawnCo());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escPanel.activeSelf)
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

    public void StageLevelUp()
    {
        stageLevel++;

        stageLevelText.text = "Level " + stageLevel.ToString();
        stageLevelText.gameObject.SetActive(true);
        curDisplayTime = levelDisplayTime;  //���� �ð� �� �ؽ�Ʈ�� ����� �� �ֵ���
        levelTextAlpha.enabled = false;     //���� �ð� �� �ؽ�Ʈ�� ����� �� �ֵ���
        stageLevelText.color = stageLevelTextColor; //������ Color���� �ʱ�ȭ
    }

    IEnumerator EnemySpawnCo()
    {
        for (int i = 0; i < stageLevel * 3; i++)
        {
            if (i % 9 == 0 && i != 0) //9����°���� ���� ��ȯ
                enemyRand = (int)MonsterType.Ghoul;
            else
                enemyRand = (int)MonsterType.Skeleton;

            posRand = Random.Range(0, SpawnPoses.Length);   //���� ������

            GameObject mon = Instantiate(monsters[enemyRand]);
            mon.transform.position = SpawnPoses[posRand].position;
            monList.Add(mon);

            yield return new WaitForSeconds(0.5f);

            if (mon.TryGetComponent(out IAttackPlayer attack)) //���̷���
            {
                attack.SetAttack();
            }

            if(mon.TryGetComponent(out SetStauts stat))
            {
                stat.SetStat(stageLevel * 10, stageLevel);
            }

        }

        StartCoroutine(checkMonsters());    //���� ��� üũ �ڷ�ƾ ����
    }

    int count = 0;
    IEnumerator checkMonsters()
    {
        while(true)
        {
            count = 0;
            for(int i = 0;  i < monList.Count; i++)
            {
                if(!monList[i].gameObject.activeSelf)
                {
                    count++;
                }
            }

            if(count == monList.Count)
            {
                monList.Clear();
                StageLevelUp();
                yield break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}