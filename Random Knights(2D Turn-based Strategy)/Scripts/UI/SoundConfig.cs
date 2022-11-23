using UnityEngine;
using UnityEngine.UI;

public class SoundConfig : MonoBehaviour
{
    public Toggle soundToggle;
    public Slider soundSlider;
    public Button okBtn;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (soundToggle != null)
            soundToggle.onValueChanged.AddListener((bool val) =>
            {
                SoundMgr.Instance.SoundOnOff(val);
            });

        if (soundSlider != null)
            soundSlider.onValueChanged.AddListener((float val) =>
            {
                SoundMgr.Instance.SoundVolume(val);
            });

        if (okBtn != null)
            okBtn.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}