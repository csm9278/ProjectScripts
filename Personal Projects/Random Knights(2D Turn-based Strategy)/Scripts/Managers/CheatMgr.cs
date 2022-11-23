using UnityEngine;
using UnityEngine.UI;

public class CheatMgr : MonoBehaviour
{
    public Button cheatOnBtn;
    public Button cheatOffBtn;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (cheatOnBtn != null)
            cheatOnBtn.onClick.AddListener(() =>
            {
                GlobalData.cheatMode = true;
                this.gameObject.SetActive(false);
            });

        if (cheatOffBtn != null)
            cheatOffBtn.onClick.AddListener(() =>
            {
                GlobalData.cheatMode = false;
                this.gameObject.SetActive(false);
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}