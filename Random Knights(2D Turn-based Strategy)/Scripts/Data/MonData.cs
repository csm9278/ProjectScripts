using UnityEngine;

[CreateAssetMenu(fileName = "MyScriptable", menuName = "Monster", order = 3)]
public class MonData : ScriptableObject
{
    [SerializeField]
    private string MonName;
    public string monName { get { return MonName; } }
    [SerializeField]
    private int maxHp;
    public int MaxHp { get { return maxHp; } }
    [SerializeField]
    private int lv;
    public int Lv { get { return lv; } }
}
