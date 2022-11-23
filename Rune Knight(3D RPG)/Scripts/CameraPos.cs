using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    float rotY = 0;

    Vector2 input;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = player.transform.position;

        if (Input.GetMouseButton(1))
        {
            Rotate();
        }

    }


    void Rotate()
    {
        if (GlobalData.gameStop)
            return;

        input.x = Input.GetAxis("Mouse X");

        if (input.x != 0)
        {
            rotY += input.x * 2;


            Quaternion nextRot = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, rotY, 0), 0.5f);
            this.transform.rotation = nextRot;

        }

    }
}
