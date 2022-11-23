using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxMarble : MonoBehaviour
{
    public float moveSpeed = 0.0f;
    public float rotateSpeed = 0.0f;

    public float Damage = 10.0f;
    public float shotDelay = 0.0f;
    bool shootRight = false;

    SpriteRenderer _spriteRenderer;
    Collider2D _collider;
    Player_TakeDamage playerdmg;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    private void Update()
    {
        if (shotDelay >= 0.0f)
        {
            shotDelay -= Time.deltaTime;
            if (shotDelay <= 0.0f)
            {
                _spriteRenderer.enabled = true;
                _collider.enabled = true;
            }

        }
        else
        {

            if (shootRight)
                this.transform.position += Vector3.right * Time.deltaTime * moveSpeed;
            else
                this.transform.position += Vector3.left * Time.deltaTime * moveSpeed;
            transform.Rotate(new Vector3(0.0f, 0.0f, rotateSpeed));
        }
    }

    public void SetRightBool(bool b)
    {
        shootRight = b;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out playerdmg) && shotDelay <= 0.0f)
        {
            playerdmg.P_TakeDamage(Damage);
            Destroy(this.gameObject);
        }
        
        if(collision.CompareTag("Platform"))
        {
            Destroy(this.gameObject);
        }
    }
}