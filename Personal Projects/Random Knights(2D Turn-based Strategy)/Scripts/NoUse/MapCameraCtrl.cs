using UnityEngine;

public class MapCameraCtrl : MonoBehaviour
{
    public float minY, maxY;

    Vector2 firstPos;
    Vector2 LastPos;

    float firstY, NowY;

    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(Input.GetMouseButtonDown(0))
        {
            firstY = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            NowY = Input.mousePosition.y;

            if (firstY > NowY)
                this.transform.Translate(Vector2.up * (Mathf.Abs(firstY - NowY) * 0.1f) * Time.deltaTime);
            else
                this.transform.Translate(Vector2.down * (Mathf.Abs(firstY - NowY) * 0.1f) * Time.deltaTime);

            if (this.transform.position.y >= maxY)
                this.transform.position = new Vector3(0, maxY, this.transform.position.z);
            else if(this.transform.position.y <= minY)
                this.transform.position = new Vector3(0, minY, this.transform.position.z);

        }

        if (Input.GetMouseButtonUp(0))
        {
            firstY = 0;
        }
    }
}