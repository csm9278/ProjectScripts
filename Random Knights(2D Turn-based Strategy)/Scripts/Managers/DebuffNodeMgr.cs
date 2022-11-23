using UnityEngine;
using UnityEngine.UI;

public class DebuffNodeMgr : MonoBehaviour
{
    public enum DebuffType
    {
        Poison,
        Weak,
        Fire,
        Count
    }

    public DebuffType debuffType = DebuffType.Poison;
    public Sprite[] debuffImgs;
    public int Count = 0;

    public Image debuffImg;
    public Text debuffCountText;
    BattleManager battleManager;
    public int ListIndex;
    

    private void Start() => StartFunc();

    private void StartFunc()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public void SetDebuff(DebuffType type, int count)
    {
        debuffType = type;
        debuffImg.sprite = debuffImgs[(int)type];
        Count = count;
        debuffCountText.text = Count.ToString(); 
    }

    public bool AddCount(DebuffType type, int count)
    {
        if(debuffType != type)
        {
            return false;
        }

        Count += count;
        //Debug.Log("Count : " + Count);
        debuffCountText.text = Count.ToString();
        return true;
    }

    public void DecreaseCount(bool isHero = false)
    {
        Count--;
        if (Count <= 0)
        {
            Destroy(this.gameObject);
            if(isHero)
                battleManager.HerodebuffNodeList.RemoveAt(ListIndex);
            else
                battleManager.EnemydebuffNodeList.RemoveAt(ListIndex);
        }
        else
            debuffCountText.text = Count.ToString();
    }
}