using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public GameObject[] SKillObj_check;
    public GameObject[] SKillObj;
    GameObject nowSkill;

    public Transform[] skillPos;
    int skillIdx = -1;
    public bool casting = false;
    Camera cam;
    LayerMask groundMask;
    RaycastHit groundHit;

    float skillDealy = -0.1f;
    float castingOffDelay = -0.1f;

    //skill1
    float[] skillCollTimes = { 30.0f, 5.0f, 10.0f, 15.0f };
    float[] curskillCollTimes = new float[4];

    public Image[] skillCoolImgs;
    public Text[] skillCoolTexts;
    public GameObject[] skillCanvasObjs;


    // Start is called before the first frame update
    void Start()
    { 
        cam = Camera.main;

        groundMask = 1 << 7;

        StartCoroutine(SkillCoolCo());
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalData.gameStop)
            return;

        if (Input.GetKeyDown(KeyCode.Q)) //힐
        {
            if (skillCanvasObjs[0].activeSelf == false)
                return;

            if (curskillCollTimes[0] > 0)
                return;

            skillIdx = 0;
            SKillObj[skillIdx].SetActive(true);
            SKillObj[skillIdx].GetComponent<ParticleSystem>().Play();
            if(SKillObj[skillIdx].TryGetComponent(out ISkillEff skill))
            {
                skill.SkillEff();
            }

            curskillCollTimes[skillIdx] = skillCollTimes[skillIdx];
            skillCoolTexts[skillIdx].gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.W)) //파이어볼
        {
            if (skillCanvasObjs[1].activeSelf == false)
                return;

            if (curskillCollTimes[1] > 0)
                return;

            if (!casting)
            {
                casting = true;
                skillIdx = 1;
                SKillObj_check[skillIdx].SetActive(true);
                nowSkill = SKillObj_check[skillIdx];
                skillDealy = 0.1f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E)) // 물줄기
        {
            if (skillCanvasObjs[2].activeSelf == false)
                return;

            if (curskillCollTimes[2] > 0)
                return;

            if (!casting)
            {
                casting = true;
                skillIdx = 2;
                SKillObj_check[skillIdx].SetActive(true);
                nowSkill = SKillObj_check[skillIdx];
                skillDealy = 0.1f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))   //끌어당기기
        {
            if (skillCanvasObjs[3].activeSelf == false)
                return;

            if (curskillCollTimes[3] > 0)
                return;

            if (!casting)
            {
                casting = true;
                skillIdx = 3;
                SKillObj_check[skillIdx].SetActive(true);
                nowSkill = SKillObj_check[skillIdx];
                skillDealy = 0.1f;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            casting = false;
            if (nowSkill == null)
                return;

            if(nowSkill.activeSelf)
                nowSkill.SetActive(false);
            skillIdx = -1;
        }

        if (casting)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam.farClipPlane;

            Vector3 dir = cam.ScreenToWorldPoint(mousePos);

            Ray ray = new Ray(cam.transform.position, dir);
            if (Physics.Raycast(ray, out groundHit, mousePos.z, groundMask))
            {
                if(SKillObj_check[skillIdx].GetComponent<SkillType>().skilltype == SkillType.type.Area)
                    SKillObj_check[skillIdx].transform.position = groundHit.point;

                if(casting && Input.GetMouseButtonDown(0) && skillDealy <= 0.0f)
                {
                    GameObject sk = Instantiate(SKillObj[skillIdx]);
                    sk.gameObject.SetActive(true);
                    sk.transform.position = skillPos[skillIdx].position;
                    Quaternion rot = Quaternion.LookRotation(sk.transform.position - this.transform.position);

                    rot.Normalize();
                    rot.x = 0;
                    rot.z = 0;

                    sk.transform.rotation = rot;

                    if (sk.TryGetComponent(out ISkillEff skill))
                    {
                        skill.SkillEff();
                    }

                    castingOffDelay = 0.1f;
                    curskillCollTimes[skillIdx] = skillCollTimes[skillIdx];
                    skillCoolTexts[skillIdx].gameObject.SetActive(true);
                    skillCoolTexts[skillIdx].text = curskillCollTimes[skillIdx].ToString("F1");
                    SKillObj_check[skillIdx].SetActive(false);
                }
            }
        }

        if(skillDealy >= 0.0f)
        {
            skillDealy -= Time.deltaTime;
        }

        if(castingOffDelay >= 0.0f)
        {
            castingOffDelay -= Time.deltaTime;
            if(castingOffDelay <= 0.0f)
            {
                casting = false;
            }
        }

    }

    IEnumerator SkillCoolCo()
    {
        while(true)
        {
            for(int i = 0; i < curskillCollTimes.Length; i++)
            {
                if(curskillCollTimes[i] >= 0.0f)
                {
                    curskillCollTimes[i] -= 0.05f;
                    skillCoolImgs[i].fillAmount = curskillCollTimes[i] / skillCollTimes[i];
                    if (skillCoolImgs[i].fillAmount > 0)
                        skillCoolTexts[i].text = curskillCollTimes[i].ToString("F1");
                    else
                        skillCoolTexts[i].gameObject.SetActive(false);
                }
            }

            yield return new WaitForSeconds(0.05f);
        }
    }



}
