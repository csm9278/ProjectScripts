using UnityEngine;
using UnityEngine.UI;

public class FillamountController : MonoBehaviour
{
    public enum UpDown
    {
        Up,
        Down
    }
        
    public float speed = 0.3f;
    public UpDown upDown = UpDown.Down;
    Image controllImage;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        controllImage = GetComponent<Image>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        switch(upDown)
        {
            case UpDown.Down:
                controllImage.fillAmount -= Time.deltaTime * speed;
                if (controllImage.fillAmount <= 0)
                    this.enabled = false;
                break;
            case UpDown.Up:
                controllImage.fillAmount += Time.deltaTime * speed;
                if (controllImage.fillAmount >= 1)
                    this.enabled = false;
                break;
        }
    }
}