using UnityEngine;

public class SelectArrow : MonoBehaviour
{
    RectTransform rectTr;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        rectTr = GetComponent<RectTransform>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}