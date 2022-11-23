using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    Camera cam;

    public Button goUpgradeBtn;
    public Button startBtn;
    public Button invenBtn;
    public Button invenCancelBtn;

    public Transform mainPos;
    public Transform upgradePos;
    public Transform startPos;

    public GameObject upgradePanel;
    public GameObject startPanel;
    public GameObject invenPanel;
    public GameObject escPanel;

    public Button cancleBtn;

    Vector3 nowPos;
    Quaternion nowRot;

    Vector3 changeVec = Vector3.zero;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        GlobalData.playerCurGold = GlobalData.playerGold;

        Time.timeScale = 1;
        cam = Camera.main;
        SetCamPos(mainPos.position, mainPos.rotation);

        if (goUpgradeBtn != null)
            goUpgradeBtn.onClick.AddListener(() =>
            {
                SetCamPos(upgradePos.position, upgradePos.rotation);
                upgradePanel.gameObject.SetActive(true);
                goUpgradeBtn.gameObject.SetActive(false);
                startBtn.gameObject.SetActive(false);
                cancleBtn.gameObject.SetActive(true);
            });

        if (startBtn != null)
            startBtn.onClick.AddListener(() =>
            {
                SetCamPos(startPos.position, startPos.rotation);
                startPanel.gameObject.SetActive(true);
                goUpgradeBtn.gameObject.SetActive(false);
                startBtn.gameObject.SetActive(false);
                cancleBtn.gameObject.SetActive(true);
            });

        if (invenBtn != null)
            invenBtn.onClick.AddListener(() =>
            {
                invenPanel.gameObject.SetActive(true);
                invenBtn.gameObject.SetActive(false);
                goUpgradeBtn.gameObject.SetActive(false);
                startBtn.gameObject.SetActive(false);
            });

        if (invenCancelBtn != null)
            invenCancelBtn.onClick.AddListener(() =>
            {
                GlobalData.playerCurGold = GlobalData.playerGold;
                invenPanel.SetActive(false);
                invenBtn.gameObject.SetActive(true);
                goUpgradeBtn.gameObject.SetActive(true);
                startBtn.gameObject.SetActive(true);
            });

        if (cancleBtn != null)
            cancleBtn.onClick.AddListener(() =>
            {
                GlobalData.playerCurGold = GlobalData.playerGold;
                SetCamPos(mainPos.position, mainPos.rotation);
                upgradePanel.gameObject.SetActive(false);
                startPanel.gameObject.SetActive(false);
                goUpgradeBtn.gameObject.SetActive(true);
                startBtn.gameObject.SetActive(true);
                cancleBtn.gameObject.SetActive(false);
            });

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, nowPos, 0.05f);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, nowRot, 0.05f);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.gameObject.SetActive(true);
        }
    }

    void SetCamPos(Vector3 pos, Quaternion rot)
    {
        nowPos = pos;
        nowRot = rot;
    }
}