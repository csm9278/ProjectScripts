using UnityEngine;
using System.Runtime.InteropServices;
using Altair_Memory_Pool_Pro;
using Weapon;

namespace HeavyArms
{
    public class HeavyArmsAttack : MonoBehaviour
    {
        //건 오브젝트
        public GameObject gunObject;
        [Range(0f, 10f)]
        public float gunRotSpeed = 0.0f;
        public float scanDistance = 0.0f;
        HeavyArmsBase haBase;
        [SerializeField] LayerMask obstacleMask;

        //TrailedBullet trailBullet;

        [DllImport("Altair_VectorCalc")]
        private static extern float GetAngle(Vector3 dir, bool b = true);

        //공격
        float m_CurAttackDelay;
        public float m_MaxAttackDelay;
        public float attackInDelay;
        float curattackInDelay;
        public Transform shotPoint;
        int attackTime = 0;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, scanDistance);
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (haBase == null)
                haBase = GetComponent<HeavyArmsBase>();

            curattackInDelay = attackInDelay;
            m_CurAttackDelay = m_MaxAttackDelay;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            CheckPlayerDistance();
            EnemyAttackFunc();
        }
        
        void CheckPlayerDistance()
        {
            Vector2 dir = haBase.player.transform.position - transform.position;

            if (dir.magnitude <= scanDistance)
            {
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, dir.magnitude, obstacleMask);
                ADebug.DrawRay(this.transform.position, dir, Color.red);
                if (hit)
                {
                    //ADebug.Log(hit.collider.gameObject);
                }
                else
                {
                    //2d 기준
                    float angle = GetAngle(dir);
                    Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                    gunObject.transform.rotation = Quaternion.Lerp(gunObject.transform.rotation, angleAxis, gunRotSpeed * Time.deltaTime);
                }


            }
        }

        public void EnemyAttackFunc()
        {
            if(curattackInDelay >= 0.0f)
            {
                curattackInDelay -= Time.deltaTime;
                return;
            }    

            if(m_CurAttackDelay >= 0.0f)
            {
                m_CurAttackDelay -= Time.deltaTime;
                if (m_CurAttackDelay <= 0.0f)
                {
                    GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, gunObject.transform.eulerAngles);
                    ////obj?.GetComponent<TrailedBullet>().SetBullet(100.0f, 0.5f, 5);
                    if(obj != null)
                    {
                        if(obj.TryGetComponent(out TrailedBullet bullet))

                        if(bullet != null)
                        {
                                bullet.SetBullet(15.0f, 0.5f, 5);
                        }
                        else
                        {
                            obj.TryGetComponent(out bullet);
                                bullet.SetBullet(15.0f, 0.5f, 5);
                        }
                    }

                    //findAttack = true;
                    m_CurAttackDelay = m_MaxAttackDelay;

                    attackTime++;

                    if (attackTime >= 50)    // 50회 공격후에는 딜레이 적용합니다
                    {
                        attackTime = 0;
                        curattackInDelay = attackInDelay;
                    }
                }
            }


        }
    }
}