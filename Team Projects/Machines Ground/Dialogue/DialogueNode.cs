using UnityEngine;
using System.Collections.Generic;

namespace RootMain
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueNode : MonoBehaviour
    {
        [System.Serializable]
        public struct Switch
        {
            [Tooltip("대화 실행 후 스위치를 On/Off할지 여부. 이미 On이나 Off인 경우 대화 비활성화")]
            public bool toOn;
            public string switchName;
        }

        [Header("Dialogue Data")]
        [Tooltip("현재 섹터와 스테이지 값을 입력.")]
        [SerializeField] private int sectorNum = 0;
        [SerializeField] private int stageNum = 0;
        [Tooltip("해당하는 Json 키값을 가져온다.")]
        [SerializeField] private string jsonKey = "";
        [Header("Switch")]
        [Tooltip("스위치를 사용하는 대화인 경우. 기본 true")]
        [SerializeField] private bool isCheckSwitch = true;
        [SerializeField] private List<Switch> switches = new List<Switch>();
        [Header("Play Flags")]
        [Tooltip("접근할 때마다 반복 실행해야 하는 메시지의 경우 체크. 앞선 스위치 조건들을 전부 무시함.")]
        [SerializeField] private bool isReplay = false;

        private new BoxCollider2D collider;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (!collider) collider = GetComponent<BoxCollider2D>();
            collider.isTrigger = true;

            //섹터 번호, 스테이지 번호, JSON키도 없다면 비활성화
            if (sectorNum <= 0 || stageNum <= 0 ||
                string.IsNullOrEmpty(jsonKey))
            {
                gameObject.SetActive(false);
                return;
            }

            if (isReplay) return; 

            if (isCheckSwitch)
            {
                if (switches.Count == 0) gameObject.SetActive(false);

                //하나면 굳이 for문을 돌리지 않게....
                else if (switches.Count == 1)
                {
                    if (string.IsNullOrEmpty(switches[0].switchName) ||
                            !SwitchManager.CurrentSwitch(switches[0].switchName))
                    {
                        gameObject.SetActive(false);
                        return;
                    }

                    if ((switches[0].toOn && SwitchManager.GetSwitch(switches[0].switchName)) ||
                        (!switches[0].toOn && !SwitchManager.GetSwitch(switches[0].switchName)))
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                }

                //2개 이상이면 for문을 돌린다.
                else
                    for (int i = 0; i < switches.Count; i++)
                    {
                        if (string.IsNullOrEmpty(switches[i].switchName) ||
                            !SwitchManager.CurrentSwitch(switches[i].switchName))
                        {
                            gameObject.SetActive(false);
                            return;
                        }

                        if ((switches[i].toOn && SwitchManager.GetSwitch(switches[i].switchName)) ||
                            (!switches[i].toOn && !SwitchManager.GetSwitch(switches[i].switchName)))
                        {
                            gameObject.SetActive(false);
                            return;
                        }
                    }
            }
        }

        //private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null) return;
            if (!collision.TryGetComponent(out Aspect aspect) && aspect.characterAspect != Aspect.CharacterAspect.Player) return;

            //플레이어가 접촉했을 경우 DialogueManager에 인수를 ref 전달하여 출력하게 함

            if (isReplay) return;

            if (isCheckSwitch)
            {
                if (switches.Count == 1)
                    SwitchManager.SetSwitch(switches[0].switchName, switches[0].toOn);
                else
                    for (int i = 0; i < switches.Count; i++)
                        SwitchManager.SetSwitch(switches[i].switchName, switches[i].toOn);

            }
            
            gameObject.SetActive(false);
        }
    }
}