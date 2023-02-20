using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    public GameObject target;

    [Header("--- LockCam ---")]
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (target == null)
            target = GameManager.inst.player;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        //this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target.transform.position.x, target.transform.position.y, -10), 1.0f);
        Vector3 nextPos = new Vector3(target.transform.position.x, target.transform.position.y, -10);
        if (nextPos.x <= minX)
            nextPos.x = minX;
        if (nextPos.x >= maxX)
            nextPos.x = maxX;

        if (nextPos.y <= minY)
            nextPos.y = minY;
        if (nextPos.y >= maxY)
            nextPos.y = maxY;


        this.transform.position = nextPos;
    }
}