using UnityEngine;

public class HpCanvas : MonoBehaviour
{
    Transform m_CameraTr = null;

    // Start is called before the first frame update
    void Start()
    {
        m_CameraTr = Camera.main.transform;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void LateUpdate()
    {
        this.transform.forward = m_CameraTr.forward;  //ºôº¸µå
    }
}