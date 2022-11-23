using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGirlWeapon : MonoBehaviour
{
    public float moveSpeed = 0.0f;
    bool goRight = false;

    Player_TakeDamage playerTakeDmg;
    // Start is called before the first frame update
    void Start()
    {
        //Destroy(this.gameObject, 3.0f);

        if (this.transform.localEulerAngles.y == 180)
            goRight = false;
        else
            goRight = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(goRight)
            this.transform.position += Vector3.right * Time.deltaTime * moveSpeed;    
        else
            this.transform.position += Vector3.left * Time.deltaTime * moveSpeed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out playerTakeDmg))
        {
            playerTakeDmg.P_TakeDamage();
            Destroy(this.gameObject);
        }
    }
}
