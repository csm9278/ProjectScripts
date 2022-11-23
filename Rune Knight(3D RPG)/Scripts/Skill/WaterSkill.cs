using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSkill : MonoBehaviour
{
    LayerMask enemyLayer;
    public bool isCheck;

    ParticleSystem _particle;

    int damage;
    public Transform nextPos;
    public Transform firstPos;


    // Start is called before the first frame update
    void Start()
    {
        enemyLayer = 1 << 8;
        damage = GlobalData.beforeStatus[(int)GlobalData.statues.WaterFall] + GlobalData.itemStatus[(int)GlobalData.statues.WaterFall];

        StartCoroutine(skillEff());


        if (isCheck)
            return;

    }

    private void OnEnable()
    {
        if (isCheck)
            StartCoroutine(checkArea());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator checkArea()
    {
        while (true)
        {
            Collider[] colls = Physics.OverlapSphere(this.transform.position, 2.5f, enemyLayer);

            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i].TryGetComponent(out ChangeColor color))
                    {
                        color.SetColor(Color.red);
                        color.ChangeDelay = 0.1f;
                    }
                }


            }

            yield return new WaitForSeconds(0.1f);
        }

    }

    IEnumerator skillEff()
    {
        _particle = GetComponentInChildren<ParticleSystem>();
        this.transform.position = firstPos.position;

        for(int i = 0; i < 3; i++)
        {
            Collider[] colls = Physics.OverlapSphere(this.transform.position, 1.5f, enemyLayer);
            _particle.Play();



            for (int j = 0; j < colls.Length; j++)
            {

                if (colls[j].TryGetComponent(out IDamageable enemy))
                {
                    enemy.TakeDamage(damage);
                }

                if(colls[j].TryGetComponent(out Rigidbody rigid))
                {
                    rigid.AddForce(Vector3.up * 1500);
                }
            }

            if (colls.Length > 0)
                Player.SetBackUpTarget(colls[0].gameObject);

            yield return new WaitForSeconds(0.75f);
            this.transform.position = nextPos.position;
        }

        Destroy(this.gameObject, 3.0f);
        yield break;
    }

    public void SkillEff()
    {
        StartCoroutine(skillEff());
    }
}