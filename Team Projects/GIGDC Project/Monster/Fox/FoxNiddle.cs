using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxNiddle : MonoBehaviour
{
    public float moveSpeed = 0.0f;
    public float rotateSpeed = 0.0f;

    public float shotDelay = 0.0f;
    bool shootRight = false;
    Vector3 shootVec = Vector3.zero;

    SpriteRenderer _spriteRenderer;
    Player_TakeDamage playerdmg;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (shotDelay >= 0.0f)
        {
            shotDelay -= Time.deltaTime;
            if (shotDelay <= 0.0f)
                _spriteRenderer.enabled = true;
        }
        else
        {
            this.transform.position += shootVec * Time.deltaTime * moveSpeed;
        }
    }

    public void NiddleInit( Vector3 a_ShotVec)
    {
        shootVec = a_ShotVec;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out playerdmg))
        {
            playerdmg.P_TakeDamage();
            Destroy(this.gameObject);
        }
    }

}
