using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float rotateSpeed;
    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        this.transform.Rotate(Vector3.up,  rotateSpeed);
    }
}