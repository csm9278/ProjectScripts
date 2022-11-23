using UnityEngine;
using UnityEngine.UI;

public class StageBtnFunc : MonoBehaviour
{
    public MapKind kind = MapKind.Battle;
    public Text kindText = null;
    public Image kindImg = null;

    public StageManager _stageMgr;

    public Sprite[] sprites;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        ChangeBtnFunc();
    }

    //private void Update() => UpdateFunc();

    //private void UpdateFunc()
    //{

    //}

    public void BtnFunc()
    {
        Debug.Log(kind);
        if (kind == MapKind.Battle)
        {
            SoundMgr.Instance.PlayEffSound("Fight");
            GameObject changein = GameObject.FindWithTag("ChangerIn");
            if (changein != null)
                changein.GetComponent<SceneChanger>().ChangeStart("BattleScene");

            GlobalData.stage1.battleNum--;
            GlobalData.stage1.floor++;
        }
        else if (kind == MapKind.Chest)
        {
            SoundMgr.Instance.PlayEffSound("EnterEvent");
            GameObject changein = GameObject.FindWithTag("ChangerIn");
            if (changein != null)
                changein.GetComponent<SceneChanger>().ChangeStart("EventScene");

            GlobalData.stage1.eventNum--;
            GlobalData.stage1.floor++;
        }
        else if (kind == MapKind.Shop)
        {
            SoundMgr.Instance.PlayEffSound("EnterShop", 0.4f);
            GameObject changein = GameObject.FindWithTag("ChangerIn");
            if (changein != null)
                changein.GetComponent<SceneChanger>().ChangeStart("ShopScene");

            GlobalData.stage1.shopNum--;
            GlobalData.stage1.floor++;
        }
        else if (kind == MapKind.Heal)
        {
            SoundMgr.Instance.PlayEffSound("heal", 0.4f);
            GlobalData.hero.HealHero((int)(GlobalData.hero.maxHp * 0.3f));
            GlobalData.stage1.HealNum--;
            GlobalData.stage1.floor++;
            GlobalData.stage1.ResetNum();
            _stageMgr.isPick = false;

            _stageMgr.ChangeBtn();
        }
        else if (kind == MapKind.BossBattle)
        {
            SoundMgr.Instance.PlayEffSound("Fight");
            GameObject changein = GameObject.FindWithTag("ChangerIn");
            GlobalData.stageLevel = 5;
            GlobalData.isBossStage = true;
            if (changein != null)
                changein.GetComponent<SceneChanger>().ChangeStart("BattleScene");
        }
        GlobalData.stage1.ResetNum();

    }

    /// <summary>
    /// 각 Num이 제대로 있는지 확인
    /// </summary>
    MapKind CheckEmpty(MapKind map)
    {
        if (GlobalData.stage1.Maxfloor <= GlobalData.stage1.floor)
        {
            map = MapKind.BossBattle;
            return map;
        }

        int eventCount = GlobalData.stage1.curbattleNum + GlobalData.stage1.cureventNum;
        int shopCount = eventCount + GlobalData.stage1.curshopNum;
        int healCount = shopCount + GlobalData.stage1.curHealNum;


        int rand = Random.Range(0, GlobalData.stage1.Maxfloor - GlobalData.stage1.curfloor);

        if (rand < GlobalData.stage1.curbattleNum)
        {
            map = MapKind.Battle;
        }
        else if (GlobalData.stage1.curbattleNum <= rand && rand < eventCount)
            map = MapKind.Chest;
        else if (eventCount <= rand && rand < shopCount)
            map = MapKind.Shop;
        else if (shopCount <= rand && rand < healCount)
            map = MapKind.Heal;
        else
            map = MapKind.BossBattle;


        return map;
    }

    public void ChangeBtnFunc()
    {
        kind = CheckEmpty(kind);
        if (kind == MapKind.Battle)
        {
            kindText.text = "전투";
            kindImg.sprite = sprites[0];
            kindImg.transform.localPosition = new Vector3(-0.4f, -14.0f, 0);
            kindImg.rectTransform.sizeDelta = new Vector2(130, 100);
            kindImg.transform.rotation = Quaternion.Euler(0, 0, 0);
            GlobalData.stage1.curbattleNum--;
        }

        if (kind == MapKind.Chest)
        {
            kindText.text = "장비 상자";
            kindImg.sprite = sprites[1];
            kindImg.transform.localPosition = new Vector3(-0.4f, -14.0f, 0);
            kindImg.rectTransform.sizeDelta = new Vector2(130, 130);
            kindImg.transform.rotation = Quaternion.Euler(0, 0, 90);

            GlobalData.stage1.cureventNum--;
        }

        if (kind == MapKind.Shop)
        {
            kindText.text = "상점";
            kindImg.sprite = sprites[2];
            kindImg.rectTransform.sizeDelta = new Vector2(150.0f, 150.0f);
            kindImg.transform.rotation = Quaternion.Euler(0, 0, 0);
            GlobalData.stage1.curshopNum--;
        }

        if (kind == MapKind.Heal)
        {
            kindText.text = "회복";
            kindImg.sprite = sprites[3];
            kindImg.rectTransform.sizeDelta = new Vector2(130, 130);
            kindImg.transform.rotation = Quaternion.Euler(0, 0, 90);
            GlobalData.stage1.curHealNum--;
        }

        if (kind == MapKind.BossBattle)
        {
            kindText.text = "마지막 전투";
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);
            kindImg.sprite = sprites[4];
            kindImg.transform.localPosition = new Vector3(0, 0, 0);
            kindImg.rectTransform.sizeDelta = new Vector2(130, 130);
            kindImg.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        GlobalData.stage1.curfloor++;
    }

}