using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public int maxMag;
    public int maxUseMag;
    public int curUseMag;
    public Transform firePos;
    

    public float shotDelay = 1.0f;
    public float reloadDelay = 2.0f;
    float curReloadDelay;
    float curShotDelay;

    string bulletName;
    public int Price;

    PlayerBase playerBS;

    [Header("--- UI Text ---")]
    public Text magText;

    public void InitGun(string bulletName)
    {
        playerBS = FindObjectOfType<PlayerBase>();

        curReloadDelay = reloadDelay;
        curUseMag = maxUseMag;
        this.bulletName = bulletName;
    }

    public void OneShotUpdate()
    {
        if (curUseMag <= 0)
            return;
        if (curShotDelay > 0)
            return;

        if (Input.GetMouseButton(0))
        {
            curUseMag--;
            curShotDelay = shotDelay;

            GameObject bullet = MemoryPoolManager.instance.GetObject(bulletName);
            bullet.transform.position = firePos.position;
            if (bullet.TryGetComponent(out Bullet bull))
            {
                bull.SetBullet(this.transform.rotation);
            }
            magText.text = curUseMag + " / " + maxMag;


            //if (curUseMag <= 0 && maxMag > 0)
            //    playerBS.ReloadImageOnOff(true);
        }
    }
    
    public void ManyShotUpdate(int bullNum)
    {
        if (curUseMag <= 0)
            return;
        if (curShotDelay > 0)
            return;

        if (Input.GetMouseButton(0))
        {
            curUseMag--;
            curShotDelay = shotDelay;

            for(int i = 0; i < bullNum; i++)
            {
                GameObject bullet = MemoryPoolManager.instance.GetObject(bulletName);
                bullet.transform.position = firePos.position;
                Quaternion rot = this.transform.rotation;
                rot.z += Random.Range(-0.25f, 0.25f);

                if (bullet.TryGetComponent(out Bullet bull))
                {
                    bull.SetBullet(rot);
                }
            }
            magText.text = curUseMag + " / " + maxMag;

            //if (curUseMag <= 0 && maxMag > 0)
            //    playerBS.ReloadImageOnOff(true);
        }
    }

    public void ResetReload()
    {
        curReloadDelay = reloadDelay;
        //playerBS.ReloadImageOnOff(false);
    }

    public void ReloadUpdate()
    {
        if(curShotDelay >= 0.0f)
        {
            curShotDelay -= Time.deltaTime;
        }

        if(curUseMag <= 0 && maxMag > 0)
        {
            playerBS.ReloadImageOnOff(true, reloadDelay);

            curReloadDelay -= Time.deltaTime;
            if(curReloadDelay <= 0.0f)
            {
                if(maxMag < maxUseMag)
                {
                    curUseMag = maxMag;
                    maxMag = 0;

                }
                else
                {
                    curUseMag = maxUseMag;
                    maxMag -= maxUseMag;
                }
                magText.text = curUseMag + " / " + maxMag;
                curReloadDelay = reloadDelay;
                //playerBS.ReloadImageOnOff(false);
            }
        }
    }
}