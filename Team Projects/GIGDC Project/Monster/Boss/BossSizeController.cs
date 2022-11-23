using UnityEngine;

public class BossSizeController : MonoBehaviour
{

    public enum BossType
    {
        Stage_1_1,
        Stage_1_2,
        Stage_1_3,
        SpwanPotal
    }

    public BossType bossType;
    float maxScaleX = 0.0f, maxScaleY = 0.0f;
    Vector3 NowScale;

    bool SizeXok = false, sizeYok = false;


    private void Start() => StartFunc();

    private void StartFunc()
    {
        if(bossType == BossType.Stage_1_1)
        {
            maxScaleX = 1.6f;
            maxScaleY = 1.6f;
        }
        else if(bossType == BossType.Stage_1_2)
        {
            maxScaleX = 1.2f;
            maxScaleY = 1.2f;
        }
        else if(bossType == BossType.SpwanPotal)
        {
            maxScaleX = 0.5f;
            maxScaleY = 0.5f;
        }
        NowScale = this.transform.localScale;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(sizeYok == false && sizeYok == false)
            SizeUpdate();
    }

    void SizeUpdate()
    {
        if (NowScale.x <= maxScaleX)
        {
            NowScale.x += Time.deltaTime * 0.75f;
        }
        else
            SizeXok = true;

        if (NowScale.y <= maxScaleY)
        {
            NowScale.y += Time.deltaTime * 0.75f;
        }
        else
            sizeYok = true;

        if (SizeXok && sizeYok)
            Time.timeScale = 1.0f;

        this.transform.localScale = NowScale;
    }
}