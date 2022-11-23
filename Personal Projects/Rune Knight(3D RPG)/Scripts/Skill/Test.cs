using UnityEngine;

public class Test : MonoBehaviour
{
    GameObject player;

    LayerMask groundMask;
    RaycastHit groundHit;
    Camera cam;

    Vector3 pos;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        player = GameObject.Find("Player");

        groundMask = 1 << 7;

        cam = Camera.main;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out groundHit, Mathf.Infinity))
        {
            pos = groundHit.point;
        }

        Quaternion rot = Quaternion.LookRotation(pos - player.transform.position);
        rot.eulerAngles = new Vector3(90, rot.eulerAngles.y, rot.eulerAngles.z);

        this.transform.rotation = Quaternion.Lerp(rot, this.transform.rotation, 0f);

    }
}