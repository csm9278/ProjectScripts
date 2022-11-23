using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle_2 : MonoBehaviour
{
    float m_DelayTime = 0.0f;
    Animator m_Ani;

    private void OnEnable()
    {
        m_DelayTime = Random.Range(2.0f, 3.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_DelayTime >= 0.0f)
        {
            m_DelayTime -= Time.deltaTime;
            m_Ani.SetFloat("DelayTime", m_DelayTime);
        }
    }


}
