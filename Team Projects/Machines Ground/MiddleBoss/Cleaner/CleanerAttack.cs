using UnityEngine;
using System.Runtime.InteropServices;
using Altair_Memory_Pool_Pro;
using System.Collections;

namespace Cleaner
{
    public class CleanerAttack : MonoBehaviour
    {
        //거리 변수
        float distancefromPlayer;
        Vector3 playerVec;

        //건 오브젝트
        public GameObject gunObject;
        [Range(0f, 10f)]
        public float gunRotSpeed = 0.0f;
        public float scanDistance = 0.0f;
        CleanerBase cleanerBase;
        [SerializeField] LayerMask obstacleMask;

        [DllImport("Altair_VectorCalc")]
        private static extern float GetAngle(Vector3 dir, bool b = true);

        //미사일 관련 변수
        float missleDelay = 5;
        float curMissileDelay;
        public Transform[] shotPoint;

        //전기톱 관련 변수
        [Header("--- Saw ---")]
        [Range(0, 1.0f)]
        public float sawAngle = 0.0f;
        public float sawDistance = 0.0f;


        [Header("--- Fire ---")]
        public GameObject fireObjs;
        public float fireDelay;
        float curFireDealy;
        ParticleSystem[] fireParticles;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            cleanerBase = GetComponent<CleanerBase>();
            fireParticles = fireObjs.GetComponentsInChildren<ParticleSystem>();
            curFireDealy = fireDelay;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            CheckPlayerDistance();
            MissileUpdate();
            SawUpdate();
            FireUpdate();
        }


        void CheckPlayerDistance()
        {
            playerVec = cleanerBase.player.transform.position - transform.position;
            distancefromPlayer = playerVec.magnitude;

            if (distancefromPlayer <= scanDistance)
            {
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, playerVec, playerVec.magnitude, obstacleMask);
                ADebug.DrawRay(this.transform.position, playerVec, Color.red);
                if (hit)
                {
                    //ADebug.Log(hit.collider.gameObject);
                }
                else
                {
                    //2d 기준
                    float angle = GetAngle(playerVec);
                    Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                    gunObject.transform.rotation = Quaternion.Lerp(gunObject.transform.rotation, angleAxis, gunRotSpeed * Time.deltaTime);
                }


            }
        }

        void MissileUpdate()
        {
            if(curMissileDelay >= 0.0f)
            {
                curMissileDelay -= Time.deltaTime;
                if(curMissileDelay <= 0.0f)
                {
                    StartCoroutine(MissileShot());
                }
            }
        }

        IEnumerator MissileShot()
        {
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < shotPoint.Length; j++)
                {
                    MemoryPoolManager.instance.BringObject("EnemyMissile", shotPoint[j].transform.position, gunObject.transform.rotation);
                }

                yield return new WaitForSeconds(.5f);
            }
            curMissileDelay = missleDelay;
        }

        //톱
        void SawUpdate()
        {

        }

        //화방
        void FireUpdate()
        {
            if (fireParticles.Length <= 0)
                return;

            if (curFireDealy >= 0.0f)
            {
                curFireDealy -= Time.deltaTime;
                if(curFireDealy <= 0.0f)
                {
                    for(int i = 0; i < fireParticles.Length; i++)
                    {
                        fireParticles[i].Play();
                    }
                    curFireDealy = fireDelay;
                }
            }
        }

    }
}