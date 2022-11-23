using UnityEngine;

namespace RootMain
{
    public class ExplosionFunc : MonoBehaviour
    {

        public GameObject[] corpseParts;    //���ڸ����� ��������� ����
        public GameObject[] explosionParts; //������ ���ư��� ������
        Rigidbody[] rigidbodys;             //���ڸ��� ������
        Rigidbody2D[] rigidbodys2D;         //���ư��¿� ������

        public GameObject deActiveParts;    //��ƾ������� ����
        public GameObject[] deActiveCorpse; //���� ���ư��� ������ �ٷ� �������� ������Ʈ
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