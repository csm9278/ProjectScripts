using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    //HpText
    [Header("--- Hp ---")]
    public int hp;
    int curHp;
    public Text hpText;
    public Image hpBar;

    //color
    SpriteRenderer sp;
    Color curColor;

    [Header("--- Canvas ---")]
    public Image reloadImage;
    public Image reloadStick;
    float reloadTime = 20.0f;
    float curTime;
    Vector2 stickFirst;
    Vector2 stickEnd;

    WeaponRoot weaponRoot;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        sp = GetComponentInChildren<SpriteRenderer>();
        curHp = hp;
        hpText.text = curHp + " / " + hp;

        stickFirst = new Vector2(-0.625f, 0.91f);
        stickEnd = new Vector2(0.75f, 0.91f);

        weaponRoot = GetComponentInChildren<WeaponRoot>();


        reloadImage.gameObject.SetActive(false);
        reloadStick.gameObject.SetActive(false);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (sp.color != Color.white)
        {
            curColor = sp.color;
            curColor.b += Time.deltaTime;
            curColor.g += Time.deltaTime;
            sp.color = curColor;
        }
    }


    public void OnDamage(int val)
    {
        curHp -= val;
        sp.color = Color.red;
        hpText.text = curHp + " / " + hp;
        hpBar.fillAmount = (float)curHp / hp;


        if (curHp <= 0)
            this.gameObject.SetActive(false);
    }

    public void ReloadImageOnOff(bool active, float reloadTime = 2)
    {
        if (reloadImage.gameObject.activeSelf == active)
            return;

        reloadImage.gameObject.SetActive(active);
        reloadStick.gameObject.SetActive(active);
        this.reloadTime = reloadTime;
        if(active)
        StartCoroutine(reloadCo());
    }

    public void AddNewGun(GameObject gun)
    {
        weaponRoot.AddGun(gun);
    }

    public void HealPlayer(int val)
    {
        if (val == 3)
            curHp += hp;
        else
        {
            curHp += (int)((hp * 0.3f) * val) ;
        }

        if(curHp >= hp)
        {
            curHp = hp;
        }

        hpText.text = curHp + " / " + hp;
        hpBar.fillAmount = (float)curHp / hp;
    }

    IEnumerator reloadCo()
    {
        curTime = 0;

        while (curTime < 1)
        {
            curTime += Time.deltaTime / reloadTime;
            reloadStick.transform.localPosition = Vector2.Lerp(stickFirst, stickEnd, curTime);
            yield return new WaitForEndOfFrame();
        }
        curTime = 0;
        reloadTime = 0;

        reloadImage.gameObject.SetActive(false);
        reloadStick.gameObject.SetActive(false);
        yield break;
    }
}