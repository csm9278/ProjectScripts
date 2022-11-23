using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscPanelManager : MonoBehaviour
{

    public Button ContinueBtn;
    public Button GoLobbyBtn;
    public Button GoTitleBtn;
    public Button QuitBtn;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (ContinueBtn != null)
            ContinueBtn.onClick.AddListener(() =>
            {
                GlobalData.gameStop = false;
                Time.timeScale = 1.0f;
                this.gameObject.SetActive(false);
            });

        if (GoLobbyBtn != null)
            GoLobbyBtn.onClick.AddListener(() =>
            {
                GlobalData.gameStop = false;
                Time.timeScale = 1.0f;
                SceneManager.LoadScene("LobbyScene");
            });

        if (GoTitleBtn != null)
            GoTitleBtn.onClick.AddListener(() =>
            {
                GlobalData.gameStop = false;
                Time.timeScale = 1.0f;
                SceneManager.LoadScene("TitleScene");
            });

        if (QuitBtn != null)
            QuitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

    }
}