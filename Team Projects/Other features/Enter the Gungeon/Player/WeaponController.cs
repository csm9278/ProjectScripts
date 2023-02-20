using UnityEngine;

public class WeaponController : MonoBehaviour
{
    Camera mainCam;
    GameObject player;
    PlayerMove prMove;
    public float weaponDistance;
    
    Vector3 mousePos;
    private void Start() => StartFunc();

    private void StartFunc()
    {
        mainCam = Camera.main;
        player = GameObject.Find("Player");
        prMove = player.GetComponent<PlayerMove>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        mousePos = ScreenToMousePos();
        prMove.mouseX = mousePos.x;
        LookFunc();
    }

    public Vector3 ScreenToMousePos()
    {
        Vector3 pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        return pos;
    }

    public void FollowFunc()
    {
    }

    public void LookFunc()
    {
        Vector3 TargetVec = new Vector2(player.transform.position.x - mousePos.x,
                                    player.transform.position.y - mousePos.y);

        Vector3 weaponPos = new Vector2(mousePos.x - player.transform.position.x,
                                        mousePos.y - player.transform.position.y);

        transform.position = (weaponPos.normalized * weaponDistance) + player.transform.position;


        float angle = Mathf.Atan2(TargetVec.y, TargetVec.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
        //Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 1);

        if (mousePos.x >= player.transform.position.x)
            transform.localScale = new Vector3(transform.localScale.x < 0 ? -transform.localScale.x : transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        else
            transform.localScale = new Vector3(transform.localScale.x > 0 ? -transform.localScale.x : transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

            transform.rotation = angleAxis;
    }


}