using System.Collections.Generic;
using UnityEngine;

public class SkillList : MonoBehaviour
{
    public static Dictionary<string, GameObject> checkSkillDic;
    public GameObject[] checkSkillObjects;

    public static Dictionary<string, GameObject> skillDic;
    public GameObject[] skillObjects;

    private void Start() => StartFunc();

    private void Awake()
    {
        checkSkillDic = new Dictionary<string, GameObject>();
        skillDic = new Dictionary<string, GameObject>();

        //��ų Ȯ�ο�
        if (checkSkillObjects.Length > 0)
        {
            for (int i = 0; i < checkSkillObjects.Length; i++)
            {
                checkSkillDic.Add(checkSkillObjects[i].gameObject.name, checkSkillObjects[i]);
            }
        }

        //��ų ������
        if (skillObjects.Length > 0)
        {
            for (int i = 0; i < skillObjects.Length; i++)
            {
                skillDic.Add(skillObjects[i].gameObject.name, skillObjects[i]);
            }
        }
    }

    private void StartFunc()
    {

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}