using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainSkill : MonoBehaviour, ISkillEff
{
    LayerMask enemyLayer;
    public bool isCheck;
    public GameObject ExpFx;

    ParticleSystem _particle;
    int damage;
    float skillTime;
    float gainSpeed = 300;

    // Start is called before the first frame update
    void Start()
    {
        enemyLayer = 1 << 8;

        damage = GlobalData.beforeStatus[(int)GlobalData.statues.GainExplosion] + GlobalData.itemStatus[(int)GlobalData.statues.GainExplosion];

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
        while(true)
        {
            Collider[] colls = Physics.OverlapSphere(this.transform.position, 2.5f, enemyLayer);

            if (colls.Length > 0)
            {
                for(int i = 0; i < colls.Length; i++)
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
        skillTime = _particle.duration + 1.0f;  // 파티클 시스템 시간 받아와서 설정

        while (true)
        {
            Collider[] colls = Physics.OverlapSphere(this.transform.position, 2.5f, enemyLayer);

            if (colls.Length > 0)
            {
                Rigidbody[] rigids = new Rigidbody[colls.Length];

                for(int i = 0; i < colls.Length; i++)   //리지드 바디들 찾아오기
                {
                    rigids[i] = colls[i].GetComponent<Rigidbody>();
                }

                for (int i = 0; i < colls.Length; i++)
                {
                    Vector3 gainVec = this.transform.position - colls[i].transform.position;

                    gainVec.Normalize();

                    //rigids[i].transform.position += gainVec * Time.deltaTime * gainSpeed;
                    rigids[i].velocity = gainVec * Time.deltaTime * gainSpeed * 1.5f;
                }
            }

            skillTime -= 0.05f;

            //데미지 주는 부분
            if (skillTime <= 0.0f)
            {
                for(int i = 0; i < colls.Length; i++)
                {
                    if(colls[i].TryGetComponent(out IDamageable enemy))
                    {
                        enemy.TakeDamage(damage);
                    }
                }

                if(colls.Length > 0)
                    Player.SetBackUpTarget(colls[0].gameObject);

                ExpFx.SetActive(true);
                Destroy(this.gameObject, 3.0f);
                yield break;
            }
            else
                yield return new WaitForSeconds(0.05f);
        }

    }

    public void SkillEff()
    {
        StartCoroutine(skillEff());
    }
}
