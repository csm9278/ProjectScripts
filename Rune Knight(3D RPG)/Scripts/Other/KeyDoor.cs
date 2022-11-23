using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    public Transform endPos;
    public float moveSpeed;
    public bool isWork = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        Vector3 vec = endPos.position - this.transform.position;


        if (isWork)
            this.transform.Translate(vec.normalized  * Time.deltaTime * moveSpeed);

        if (vec.magnitude <= 0.1f)
            this.gameObject.SetActive(false);
    }
}