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
            [Tooltip("��ȭ ���� �� ����ġ�� On/Off���� ����. �̹� On�̳� Off�� ��� ��ȭ ��Ȱ��ȭ")]
            public bool toOn;
            public string switchName;
        }

        [Header("Dialogue Data")]
        [Tooltip("���� ���Ϳ� �������� ���� �Է�.")]
        [SerializeField] private int sectorNum = 0;
        [SerializeField] private int stageNum = 0;
        [Tooltip("�ش��ϴ� Json Ű���� �����´�.")]
        [SerializeField] private string jsonKey = "";
        [Header("Switch")]
        [Tooltip("����ġ�� ����ϴ� ��ȭ�� ���. �⺻ true")]
        [SerializeField] private bool isCheckSwitch = true;
        [SerializeField] private List<Switch> switches = new List<Switch>();
        [Header("Play Flags")]
        [Tooltip("������ ������ �ݺ� �����ؾ� �ϴ� �޽����� ��� üũ. �ռ� ����ġ ���ǵ��� ���� ������.")]
        [SerializeField] private bool isReplay = false;

        private new BoxCollider2D collider;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (!collider) collider = GetComponent<BoxCollider2D>();
            collider.isTrigger = true;

            //���� ��ȣ, �������� ��ȣ, JSONŰ�� ���ٸ� ��Ȱ��ȭ
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

                //�ϳ��� ���� for���� ������ �ʰ�....
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

                //2�� �̻��̸� for���� ������.
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

            //�÷��̾ �������� ��� DialogueManager�� �μ��� ref �����Ͽ� ����ϰ� ��

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