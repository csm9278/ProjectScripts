using UnityEngine;

namespace RootMain
{
    public class ExplosionFunc : MonoBehaviour
    {

        public GameObject[] corpseParts;    //제자리에서 사망연출할 부위
        public GameObject[] explosionParts; //실제로 날아가는 부위들
        Rigidbody[] rigidbodys;             //제자리용 리지드
        Rigidbody2D[] rigidbodys2D;         //날아가는용 리지드

        public GameObject deActiveParts;    //녹아없어질때 부위
        public GameObject[] deActiveCorpse; //실제 날아가는 부위를 바로 끄기위한 오브젝트
        public Vector3 expPos;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            for(int i = 0; i < deActiveCorpse.Length; i++)
            {
                deActiveCorpse[i].SetActive(false);
            }

            if (corpseParts.Length > 0)
            {
                rigidbodys = new Rigidbody[corpseParts.Length];
                for (int i = 0; i < corpseParts.Length; i++)
                {
                    rigidbodys[i] = corpseParts[i].GetComponent<Rigidbody>();
                    Vector3 dir = corpseParts[i].transform.position - expPos;
                    dir.z = 0;
                    dir.Normalize();
                    rigidbodys[i].useGravity = true;

                    rigidbodys[i].AddForce(dir * 50);




                    if (rigidbodys[i].TryGetComponent(out PartsRotate rotate))
                    {
                        rotate.enabled = true;
                    }
                }
            }

            if (explosionParts.Length > 0)
            {
                rigidbodys2D = new Rigidbody2D[explosionParts.Length];
                for (int i = 0; i < explosionParts.Length; i++)
                {
                    explosionParts[i].SetActive(true);
                    //rigidbodys2D[i] = explosionParts[i].GetComponent<Rigidbody2D>();
                    Rigidbody2D rg = explosionParts[i].GetComponent<Rigidbody2D>();
                    Vector2 dir = explosionParts[i].transform.position - expPos;
                    dir.Normalize();


                    float force = Random.Range(250.0f, 500.0f);
                    rg.AddForce(dir * force);

                }
            }

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (deActiveParts == null)
                return;

            Vector3 partVec = deActiveParts.transform.position - expPos;

            if (partVec.magnitude >= 6.0f)
                deActiveParts.SetActive(false);
        }
    }
}