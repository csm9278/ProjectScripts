using UnityEngine;

public class FireBallSkill : MonoBehaviour
{
    public float moveSpeed;
    public ParticleSystem expFx;
    public GameObject[] otherFx;
    bool isMove = true;

    int damage;
    LayerMask enemyLayer;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        Destroy(this.gameObject, 3.0f);

        enemyLayer = 1 << 8;
        damage = GlobalData.beforeStatus[(int)GlobalData.statues.FireBall] + GlobalData.itemStatus[(int)GlobalData.statues.FireBall];
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(isMove)
        this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
            other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            for(int i = 0; i < otherFx.Length; i++)
            {
                otherFx[i].gameObject.SetActive(false);
            }
            expFx.gameObject.SetActive(true);

            Collider[] colls = Physics.OverlapSphere(this.transform.position, 1.0f, enemyLayer);

            if(colls.Length > 0)
            {
                for(int i = 0; i < colls.Length; i++)
                {
                    if(colls[i].TryGetComponent(out IDamageable damage))
                    {
                        damage.TakeDamage(this.damage);
                    }
                }
                Player.SetBackUpTarget(colls[0].gameObject);
            }

            this.gameObject.GetComponent<Collider>().enabled = false;
            Destroy(this.gameObject, 2.0f);
            isMove = false;

        }
    }

}