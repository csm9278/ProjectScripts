using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Health = 0,
        Item,
        Gun,
        Ammo,
        Count
    }

    public ItemType type;

    public GameObject infoCanvas;
    GameObject player;
    SpriteRenderer spRenderer;

    public Text itemInfoText;
    int pirce = 0;

    //Heal
    int itemValue = 0;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        spRenderer = GetComponentInChildren<SpriteRenderer>();

        InitItem(type);
        infoCanvas.gameObject.SetActive(false);

        player = GameObject.Find("Player");
    }

    public void ResetItem()
    {
        this.gameObject.SetActive(true);
        InitItem(type);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        //±¸¸Å?
        if(infoCanvas.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                ItemEff();
                this.gameObject.SetActive(false);
            }
        }
    }

    public void InitItem(ItemType type)
    {
        switch(type)
        {
            case ItemType.Health:
                itemValue = Random.Range(1, 4);
                spRenderer.sprite = GlobalData.healItem[itemValue - 1];
                itemInfoText.text = GlobalData.healInfo[itemValue - 1] + "\n" + (itemValue* 30) + "Gold";
                break;

            case ItemType.Gun:
                itemValue = Random.Range(0, GlobalData.weapons.Length);
                spRenderer.sprite = GlobalData.weapons[itemValue].GetComponentInChildren<SpriteRenderer>().sprite;
                itemInfoText.text = GlobalData.weapons[itemValue].name + "\n" + GlobalData.weapons[itemValue].GetComponent<Gun>().Price + "Gold";
                break;

            case ItemType.Item:

                break;

            case ItemType.Ammo:

                break;
        }
    }

    public void ItemEff()
    {
        switch(type)
        {
            case ItemType.Health:
                Heal();
                break;

            case ItemType.Gun:
                BuyGun();
                break;

            case ItemType.Item:

                break;

            case ItemType.Ammo:

                break;
        }
    }

    void Heal()
    {
        if(player.TryGetComponent(out PlayerBase pBase))
        {
            pBase.HealPlayer(itemValue);
        }
    }

    void BuyGun()
    {
        if (player.TryGetComponent(out PlayerBase pBase))
        {
            pBase.AddNewGun(GlobalData.weapons[itemValue]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            infoCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            infoCanvas.gameObject.SetActive(false);
        }
    }
}