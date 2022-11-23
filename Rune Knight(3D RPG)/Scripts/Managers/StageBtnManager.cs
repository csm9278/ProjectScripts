using UnityEngine;
using UnityEngine.UI;

public class StageBtnManager : MonoBehaviour
{
    public int StageNum;

    public Text stageName;
    public Button startBtn;

    public GameObject fadeInObj;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (GlobalData.StageLevel < StageNum)
            this.gameObject.SetActive(false);


        if (startBtn != null)
            startBtn.onClick.AddListener(() =>
            {
                if(fadeInObj.TryGetComponent(out SceneChangeManager mgr))
                {
                    mgr.ChangeScene(stageName.text);
                }
            });
    }

}