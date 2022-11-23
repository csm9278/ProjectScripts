using UnityEngine;

public class FirstSceneMgr : MonoBehaviour
{

    private void Start() => StartFunc();

    private void StartFunc()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}