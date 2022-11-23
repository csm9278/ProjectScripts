using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage1Manager : MonoBehaviour
{
    Camera cam;
    public KeyDoor keydoor;
    float delay = -.1f;

    float firstDelay = 2.0f;
    public ChangeAlpha firstTitleAlpha;

    public GameObject clearPanel;
    public GameObject bossObj;

    public Button goLobbyBtn;
    public GameObject escPanel;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        GlobalData.gameStop = true;
        cam = Camera.main;

        if (goLobbyBtn != null)
            goLobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(firstDelay >= 0.0f)
        {
            firstDelay -= Time.deltaTime;
            if(firstDelay <= 0.0f)
            {
                firstTitleAlpha.enabled = true;
            }
        }

        // ������ ������ �������� ����
        if(delay >= 0.0f)
        {
            delay -= Time.deltaTime;
            if(delay <= 0.0f)
            {
                keydoor.enabled = true;
                keydoor.isWork = true;
            }
        }

        if (!bossObj.activeSelf)
        {
            clearPanel.SetActive(true);
            GlobalData.gameStop = true;

            GlobalData.StageLevelUp(2);
            Time.timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escPanel.activeSelf)
            {
                Time.timeScale = 1.0f;
                GlobalData.gameStop = false;
                escPanel.gameObject.SetActive(false);
            }
            else
            {
                Time.timeScale = 0.0f;
                GlobalData.gameStop = true;
                escPanel.gameObject.SetActive(true);
            }
        }
    }

    public void OpenDoor(KeyDoor door)
    {
        keydoor = door; //���� �Ѱܹ��� ���� ���� ���� �޾ƿ�
        cam.GetComponent<CameraController>().camType = CameraController.CamType.Target;
        GlobalData.gameStop = true;
        delay = 1.0f;   //1�� �� ���� ������ ����(ī�޶� �̵� ������)
    }
}