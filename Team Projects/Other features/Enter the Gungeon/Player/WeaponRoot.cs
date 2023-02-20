using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRoot : MonoBehaviour
{
    List<GameObject> weaponList;
    int idx = 0;
    public Image gunImage;
    public Text gunAmmoText;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        weaponList = new List<GameObject>();
        Gun[] guns = transform.GetComponentsInChildren<Gun>();

        for(int i = 0; i < guns.Length; i++)
        {
            Debug.Log(guns[i].gameObject.name);
            weaponList.Add(guns[i].gameObject);
            if (guns[i].TryGetComponent(out Gun gun))
                gun.magText = gunAmmoText;
        }

        Debug.Log(weaponList.Count);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        weaponChangeFunc();
    }

    public void AddGun(GameObject gun)
    {
        GameObject newGun = Instantiate(gun, this.transform);

        if(newGun.TryGetComponent(out Gun g))
        {
            g.magText = gunAmmoText;
            gunAmmoText.text = g.maxUseMag + " / " + g.maxMag;
        }
        weaponList.Add(newGun);
        weaponList[idx].GetComponent<Gun>().ResetReload();
        weaponList[idx].gameObject.SetActive(false);
        idx = weaponList.Count - 1;
        gunImage.sprite = weaponList[idx].GetComponentInChildren<SpriteRenderer>().sprite;
        gunImage.SetNativeSize();
    }

    public void weaponChangeFunc()
    {
        if (weaponList.Count <= 1)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0)  //다음무기
            {
                //이전 무기 active false
                weaponList[idx].GetComponent<Gun>().ResetReload();
                weaponList[idx].gameObject.SetActive(false);

                idx++;
                if (idx >= weaponList.Count)
                    idx = 0;
                weaponList[idx].gameObject.SetActive(true);

                gunImage.sprite = weaponList[idx].GetComponentInChildren<SpriteRenderer>().sprite;
                gunImage.SetNativeSize();

                if(weaponList[idx].TryGetComponent(out Gun gun))
                {
                    gunAmmoText.text = gun.curUseMag + " / " + gun.maxMag;
                }

            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)   //이전 무기
            {
                weaponList[idx].GetComponent<Gun>().ResetReload();
                weaponList[idx].gameObject.SetActive(false);
                idx--;
                if (idx < 0)
                    idx = weaponList.Count - 1;

                weaponList[idx].gameObject.SetActive(true);

                gunImage.sprite = weaponList[idx].GetComponentInChildren<SpriteRenderer>().sprite;
                gunImage.SetNativeSize();

                if (weaponList[idx].TryGetComponent(out Gun gun))
                {
                    gunAmmoText.text = gun.curUseMag + " / " + gun.maxMag;
                }
            }

            Time.timeScale = 0.3f;
        }
        else
            Time.timeScale = 1.0f;
    }
}