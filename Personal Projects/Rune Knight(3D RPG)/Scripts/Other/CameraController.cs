using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CamType
    {
        Top,
        Back,
        Target
    }

    public GameObject Player;
    public CamType camType = CamType.Top;

    Vector3 targetPos = Vector3.zero;
    Vector3 nextPos = Vector3.zero;
    public float camSpeed;

    public Transform Target;

    float targetTimer = 5.0f;
    float curTargetTimer = 5.0f;

    public float rotX;
    float rotY = 0;

    Vector2 input;
    public Transform playerCamPos;

    float curXPlus;
    bool isRotate = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        Player = GameObject.FindObjectOfType<Player>().gameObject;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {

        if (camType == CamType.Top)
        {
            //targetPos = new Vector3(Player.transform.position.x, Player.transform.position.y + 6.5f, Player.transform.position.z + -4.0f);
            //nextPos = Vector3.Lerp(this.transform.position, targetPos, camSpeed * Time.deltaTime);
            if (!isRotate)
                nextPos = Vector3.Lerp(this.transform.position, playerCamPos.position, .5f);
            else
                nextPos = Vector3.Lerp(this.transform.position, playerCamPos.position, .5f);


            this.transform.position = nextPos;
            //this.transform.rotation = Quaternion.Euler(rotX, rotY, 0);

            Quaternion nextRot = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(rotX, rotY, 0), .5f);
            this.transform.rotation = nextRot;
        }
        else if(camType == CamType.Target)
        {
            nextPos = Vector3.Lerp(this.transform.position, Target.position, 0.05f);
            Quaternion nextRot = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, 0, 0), 0.05f);
            this.transform.position = nextPos;
            this.transform.rotation = nextRot;

            if(curTargetTimer >= 0.0f)
            {
                curTargetTimer -= Time.deltaTime;
                if(curTargetTimer <= 0.0f)
                {
                    camType = CamType.Top;
                    GlobalData.gameStop = false;
                    Time.timeScale = 1;
                }
            }    
        }

        if (Input.GetMouseButton(1))
        {
            Rotate();
        }
        else
            isRotate = false;

    }

    void Rotate()
    {
        if (GlobalData.gameStop)
            return;


        input.x = Input.GetAxis("Mouse X");

        if(input.x != 0)
        {
            rotY += input.x * 2;

            isRotate = true;
        }
       
    }
}