using UnityEngine;
using UnityEngine.UI;

public class ShopItemRootMgr : MonoBehaviour
{
    enum ItemType
    {
        Heal,
        AtkUp,
        HpUp,
        MaxCount
    }

    ItemType itemType = ItemType.MaxCount;
    public Sprite[] iconSprites;

    public int itemCost;
    public Text itemText;
    public Image ItemIconImg;
    public Text costText;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        //itemType = (ItemType)Random.Range(0, (int)ItemType.MaxCount);


    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public void InitItem(int i)
    {
        itemType = (ItemType)i;
        switch (itemType)
        {
            case ItemType.Heal:
                itemCost = 2;
                itemText.text = "체력 30%회복";
                costText.text = "x 2";
                ItemIconImg.sprite = iconSprites[0];
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(103, 110);
                break;

            case ItemType.AtkUp:
                itemCost = 4;
                itemText.text = "추가 고정공격력 + 1";
                costText.text = "x 4";
                ItemIconImg.sprite = iconSprites[1];
                break;

            case ItemType.HpUp:
                itemCost = 4;
                itemText.text = "최대체력 4 증가";
                costText.text = "x 4";
                ItemIconImg.sprite = iconSprites[1];
                break;

                //case ItemType.RandomItem:
                //    itemCost = 2;
                //    itemText.text = "랜덤 스킬 획득";
                //    costText.text = "x 2";
                //    ItemIconImg.sprite = iconSprites[2];
                //    break;
        }
    }

    public void ItemEff()
    {
        switch (itemType)
        {
            case ItemType.Heal:
                GlobalData.hero.HealHero((int)(GlobalData.hero.maxHp * 0.3f));
                GlobalData.hero.gold -= 2;
                break;

            case ItemType.AtkUp:
                GlobalData.hero.AddAtk++;
                GlobalData.hero.gold -= 4;
                SoundMgr.Instance.PlayEffSound("gainxp", 0.4f);
                break;

            case ItemType.HpUp:
                GlobalData.hero.maxHp += 4;
                GlobalData.hero.HealHero(4);
                GlobalData.hero.gold -= 4;
                break;
        }
    }
}