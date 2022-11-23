using UnityEngine;
using UnityEngine.UI;

public class DataClearMgr : MonoBehaviour
{
    public Button clearDataBtn;
    public Button cancleBtn;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (clearDataBtn != null)
            clearDataBtn.onClick.AddListener(() =>
            {
                GlobalData.StagetutorialOn = true;
                GlobalData.BattletutorialOn = true;
                GlobalData.ShoptutorialOn = true;
                GlobalData.EquiptutorialOn = true;

                GlobalData.isWarriorClear = false;
                GlobalData.isThiefClear = false;
                GlobalData.isMageClear = false;

                PlayerPrefs.DeleteAll();

                this.gameObject.SetActive(false);
            });

        if (cancleBtn != null)
            cancleBtn.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
    }           

    //private void Update() => UpdateFunc();

    //private void UpdateFunc()
    //{
        
    //}
}