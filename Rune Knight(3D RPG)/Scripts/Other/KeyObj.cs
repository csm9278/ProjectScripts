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

            //추가적으로 상호작용할 오브젝트들을 켜준다 (현재는 적군 오브젝트 무리)
            if(AdditiveObjs.Length > 0)
                for(int i = 0; i < AdditiveObjs.Length; i++)
                {
                    AdditiveObjs[i].SetActive(true);
                }

            stage1Mgr.OpenDoor(workObj);    //열려야 하는 문을 넘겨줌
            this.gameObject.SetActive(false);   //키 오브젝트 비활성화
        }
    }
}