using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    public enum objectType
    {
        Battle,
        Shop,
        Map,
        Count
    }

    [Header("--- Objects ---")]
    public GameObject[] objectsArr;
    int objectIdx;
    public GameObject player;
    EnemyGenerator enemyGenerator;
    
    [Header("--- FadeInOut ---")]
    public Image fadeOutImage;
    public Image fadeInImage;
    Color curColor;
    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    public bool changing = false;

    private void Awake()
    {
        inst = this;
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        player = GameObject.Find("Player");
        player.gameObject.SetActive(false);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
            ChangeObjects(objectType.Map);
    }

    public void ChangeObjects(objectType type)
    {
        if (changing)
            return;

        StartCoroutine(ChangeCo(type));

    }

    IEnumerator ChangeCo(objectType type)
    {
        GlobalData.playerMoveLock = true;
        changing = true;
        Debug.Log(changing);
        fadeInImage.gameObject.SetActive(true);
        while(true)
        {
            if (FadeIn())
                break;
            yield return endOfFrame;
        }
        fadeOutImage.gameObject.SetActive(true);
        objectsArr[objectIdx].gameObject.SetActive(false);
        objectIdx = (int)type;
        objectsArr[objectIdx].gameObject.SetActive(true);

        if (type == objectType.Map) //map에선 플레이어가 안보이게
        {
            player.gameObject.SetActive(false);
            Camera.main.transform.position = Vector3.zero;
        }
        else
        {
            player.gameObject.SetActive(true);
            objectsArr[(int)objectType.Map].gameObject.SetActive(false);
        }

        player.transform.position = Vector3.zero;
        fadeInImage.gameObject.SetActive(false);

        //타입별 추가적인 요소 설정
        switch (type)
        {
            case objectType.Battle:
                BattleObjectSet();
                break;
        }

        yield return new WaitForSeconds(2.0f);

        while (true)
        {
            if (FadeOut())
                break;
            yield return endOfFrame;
        }
        fadeOutImage.gameObject.SetActive(false);

        fadeInImage.color = new Color(0, 0, 0, 0);
        fadeOutImage.color = new Color(0, 0, 0, 1);
        changing = false;

        GlobalData.playerMoveLock = false;


    }

    void BattleObjectSet()
    {
        if (enemyGenerator == null)
            enemyGenerator = objectsArr[(int)objectType.Battle].GetComponentInChildren<EnemyGenerator>();

        if(enemyGenerator != null)
        enemyGenerator.BattleStart();
    }

    bool FadeIn()
    {
        curColor = fadeInImage.color;
        if (curColor.a <= 1)
        {
            curColor.a += Time.deltaTime;
            fadeInImage.color = curColor;
        }
        else
            return true;

        return false;
    }

    bool FadeOut()
    {
        curColor = fadeOutImage.color;

        if (curColor.a > 0)
        {
            curColor.a -= Time.deltaTime;
            fadeOutImage.color = curColor;
        }
        else
            return true;

        return false;    
    }
}