using UnityEngine;
using System.Collections;

public class HealSkill : MonoBehaviour, ISkillEff
{
    Player player;
    WaitForSeconds healDelay = new WaitForSeconds(0.1f);

    private void Start() => StartFunc();

    private void StartFunc()
    {
    }

    public void SkillEff()
    {
        if(player == null)
        player = FindObjectOfType<Player>();
        StartCoroutine(HealSkillCo());
    }

    IEnumerator HealSkillCo()
    {
        for(int i = 0; i < GlobalData.beforeStatus[(int)GlobalData.statues.Heal] + GlobalData.itemStatus[(int)GlobalData.statues.Heal]; i++)
        {
            player.Heal(1);
            yield return healDelay;
        }
    }

}