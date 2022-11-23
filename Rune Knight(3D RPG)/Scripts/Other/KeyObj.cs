using UnityEngine;

public class KeyObj : MonoBehaviour
{
    public Stage1Manager stage1Mgr;
    public KeyDoor workObj;

    public GameObject[] AdditiveObjs;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        stage1Mgr = FindObjectOfType<Stage1Manager>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 200);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GlobalData.gameStop = true;

            //�߰������� ��ȣ�ۿ��� ������Ʈ���� ���ش� (����� ���� ������Ʈ ����)
            if(AdditiveObjs.Length > 0)
                for(int i = 0; i < AdditiveObjs.Length; i++)
                {
                    AdditiveObjs[i].SetActive(true);
                }

            stage1Mgr.OpenDoor(workObj);    //������ �ϴ� ���� �Ѱ���
            this.gameObject.SetActive(false);   //Ű ������Ʈ ��Ȱ��ȭ
        }
    }
}