using UnityEngine;

public class Bullet : MemoryPoolObject
{
    public float bulletSpeed;

    public float lifeTime = 3.0f;
    float maxlifeTime = 0.0f;
    public int bulletDamage;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        maxlifeTime = lifeTime;
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        this.transform.Translate(Vector2.up * Time.deltaTime * bulletSpeed);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0.0f)
        {
            lifeTime = maxlifeTime;
            ObjectReturn();
        }
    }

    public void SetBullet(Quaternion rot)
    {
        this.transform.rotation = rot;
    }
}