using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public enum ChangeType
    {
        In,
        Out
    }

    public ChangeType changeType = ChangeType.In;

    Vector3 StartPos = Vector3.zero;
    Vector3 EndPos = Vector3.zero;

    float DelayTime = -.5f;
    string changeSceneName = "";
    bool isChangeStart = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if(changeType == ChangeType.In)
        {
            StartPos = new Vector3(-1400.0f, 0, 0);
            EndPos = Vector3.zero;
        }
        else if(changeType == ChangeType.Out)
        {
            StartPos = Vector3.zero;
            EndPos = new Vector3(1400.0f, 0, 0);
            DelayTime = 1.0f;
        }

        this.transform.localPosition = StartPos;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        SceneChange();
    }

    public void SceneChange()
    {
        if(DelayTime >= 0.0f)
        {
            DelayTime -= Time.deltaTime;
            return;
        }

        if (isChangeStart && this.transform.localPosition.x <= EndPos.x)
        {
            this.transform.Translate(Vector3.right * 20);
            if (changeSceneName != "" &&
                this.transform.localPosition.x >= EndPos.x)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(changeSceneName);
            }
        }
    }

    public void ChangeStart(string sceneName = "")
    {
        this.gameObject.SetActive(true);
        isChangeStart = true;

        if (sceneName != "")
            changeSceneName = sceneName;
    }
}