using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

namespace RootMain
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] public DialogueManager d_dialog;
        [SerializeField] private KeyNames _keyNames;

        //[Header("Stage key")]
        //string stagekey = "";

        [Header("Dialogue Idx")]
        [SerializeField] internal int idx;
        
        [Header("Use Switch")]
        [SerializeField] private bool isCheckSwitch;
        [SerializeField] private string switchName;
        [SerializeField] private bool workBySwitch;

        [Header("Event Caller")]
        [SerializeField] private GameObject beforeEvent;
        [SerializeField] private GameObject afterEvent;
        [SerializeField] private GameObject player;

        string chakey = "";
        string stagekey = "";
        //char[] stageCharArray;
        string dlgkey = "";



        // Start is called before the first frame update
        void Start()
        {
            d_dialog = GameObject.FindObjectOfType<DialogueManager>();
        }

        // Update is called once per frame
        //void Update()
        //{

        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isCheckSwitch && !string.IsNullOrEmpty(switchName))
                if (SwitchManager.GetSwitch(switchName) != workBySwitch)
                    return;

            if (beforeEvent != null)
                if (player != null)
                {

                    Vector2 playerVec = beforeEvent.transform.position - player.transform.position;
                    float angle = Mathf.Atan2(playerVec.y, playerVec.x) * Mathf.Rad2Deg;
                    Quaternion angleAxis = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
                    player.transform.rotation = angleAxis;


                }    
            if (collision.gameObject.TryGetComponent(out Aspect aspect))
            {
                if (aspect.characterAspect == Aspect.CharacterAspect.Player)
                {

                    //if (this.gameObject.name.Contains("I_Dialog")) //���� Ŭ������ �������ִ� ������Ʈ�� ��Ʈ�� ���̾�α׶��
                    //{
                    //    chakey = "Prologue";
                    //    stagekey = "intro";
                    //}
                    //else if (this.gameObject.name.Contains("P_Dialog")) //���� Ŭ������ �������ִ� ������Ʈ�� ���ѷα� ���̾�α׶��
                    //{
                    //    chakey = "Prologue";
                    //    stagekey = "Prologue";
                    //}
                    //else if (this.gameObject.name.Contains("S1_Dialog") && GlobalData.curAssistant == AssistantType.Blitz) //���� Ŭ������ �������ִ� ������Ʈ�� 1�������� ���̾�α׶��
                    //{
                    //    chakey = "Blitz";
                    //    stagekey = "Stage1";
                    //}

                    if (SceneManager.GetActiveScene().name.Contains("Intro_Scene"))
                    {
                        chakey = "Prologue";
                        stagekey = "intro";
                    }

                    else if (SceneManager.GetActiveScene().name.Contains("Prolog_Scene"))
                    {
                        chakey = "Prologue";
                        stagekey = "Prologue";
                    }

                    //else if (SceneManager.GetActiveScene().name.Contains("S1S1_Scene"))
                    //{
                    //    if (GlobalData.curAssistant == AssistantType.Blitz)
                    //    {
                    //        chakey = "Blitz";
                    //        stagekey = "Stage1";
                    //    }

                    //}

                    else if (SceneManager.GetActiveScene().name.Contains("TestScene"))
                    {
                        chakey = "Prologue";
                        stagekey = "Prologue";
                    }

                    //����ó���� Scene �̸��� Ű�� ��������
                    else
                    {
                        string name = SceneManager.GetActiveScene().name;
                        //stageCharArray = name.ToCharArray();
                        stagekey = "S" + name[1] + "S" + name[3];
                    }

                    //Debug.Log(chakey);
                    //Debug.Log(stagekey);

                    //�̸��� ���� GlobaData ���
                    if(!SceneManager.GetActiveScene().name.Contains("Intro_Scene") 
                        && !SceneManager.GetActiveScene().name.Contains("Prolog_Scene") && !SceneManager.GetActiveScene().name.Contains("TestScene"))
                        chakey = GlobalData.curAssistant.ToString();


                    dlgkey = "Dialog" + (idx + 1).ToString();
                    d_dialog.ShowDialogue(ref chakey, ref stagekey, ref dlgkey, idx, _keyNames,afterEvent);

                    if (!_keyNames.isPlayOnce)
                        _keyNames.isPlayOnce = true;

                    if (_keyNames.isPlayOnce == true)
                        this.gameObject.GetComponent<Collider2D>().enabled = false;
                }
            }
        }

    }
}
