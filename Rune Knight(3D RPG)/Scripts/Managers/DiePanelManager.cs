using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiePanelManager : MonoBehaviour
{


    Image fadeImg;
    Color fadeColor;
    float fadeAlpha;

    public GameObject RestartObj;

    public Button restartBtn;
    public Button lobbyBtn;

    public bool changeStart = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        fadeImg = this.gameObject.GetComponent<Image>();

        if (restartBtn != null)
            restartBtn.onClick.AddListener(() =>
            {
                GlobalData.gameStop = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

        if (lobbyBtn != null)
            lobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        FadeInFunc();
    }


    void FadeInFunc()
    {
        fadeAlpha += Time.deltaTime;
        fadeColor = new Color(0, 0, 0, fadeAlpha);

        fadeImg.color = fadeColor;

        if (fadeAlpha >= 1)
        {
            RestartObj.gameObject.SetActive(true);
        }
    }
}