using UnityEngine;

public class MonAttackEff : MonoBehaviour
{
    Player_TakeDamage takedam;


    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 공격당함");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit");
            if (collision.gameObject.TryGetComponent(out takedam))
            {
                takedam.P_TakeDamage();
            }
            else
                Debug.Log("플레이어 공격당함 / takedam 없음");
        }
        else
            Debug.Log(collision.gameObject);
    }
}