using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;
using Altair_Memory_Pool_Pro;
using System.Runtime.InteropServices;
using SimpleJSON;

namespace Turret
{
    public class GuardTurret : MonoBehaviour, IDamageable, IHitted
    {
        GameObject m_Player = null;

        public bool m_bDebugMode = true;
        public bool m_SightSet = true;
        public int maxHP;   //터렛의 최대 체력
        public int curHP;

        public float FindDistance = 0.0f;   //터렛 발견 범위
        public float AtkDelay = 0.0f;       //터렛 공격 속도
        public Transform shotPoint = null;  //터렛 공격 포인트

        float CurAtkDelay = 0.0f;
        float DistanceFromPlayer = 0.0f;    //플레이어와의 거리
        bool FindPlayer = false;
        Vector3 CalcVec = Vector3.zero;

        //터렛 회전
        [Header("--- Scroll Turret ---")]
        [Range(0, 360.0f)]
        [SerializeField] private float horizontalViewAngle;
        float horizontalViewHalfAngle;

        //순찰 포인트 지정
        [Range(0, 360.0f)]
        [SerializeField] float scanAngle;
        [Range(0, 360.0f)]
        [SerializeField] float CenterAngle;
        float scanHalfAngle;
        Vector3 horizontalScanRight;
        Vector3 horizontalScanLeft;
        bool bTurnRight = false;
        float halfScanAngleFloatRight;  // +방향
        float halfScanAngleFloatLeft;   // -방향

        //시야의 디버깅 변수
        Vector3 curhorizontalScanRight;
        Vector3 curhorizontalScanLeft;

        public bool IsHitted = false;
        float lookTimer;

        //쉴드 포탑
        ShieldCtrl shieldCtrl;
        float shiledOldhp;

        public float turretRotSpeed;
        float turretCurRotSpeed;
        float viewRotateZ;
        Vector3 Rotvec;

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;

        [DllImport("Altair_VectorCalc")]
        private static extern Vector3 GetVector(float angle, bool b = true);
        [DllImport("Altair_VectorCalc")]
        private static extern float GetAngle(Vector3 dir, bool b = true);

        private void Awake()
        {
            horizontalViewHalfAngle = horizontalViewAngle * 0.3f;
            scanHalfAngle = scanAngle * 0.5f;

            curhorizontalScanRight = AngleToDirZ(-scanHalfAngle + -CenterAngle);
            curhorizontalScanLeft = AngleToDirZ(scanHalfAngle + -CenterAngle);

        }

        private void OnDrawGizmos()
        {
            horizontalViewHalfAngle = horizontalViewAngle * 0.3f;
            scanHalfAngle = scanAngle * 0.5f;

            Vector3 horizontalRightDir = AngleToDirZ(-horizontalViewHalfAngle + viewRotateZ);
            Vector3 horizontalLeftDir = AngleToDirZ(horizontalViewHalfAngle + viewRotateZ);

            //범위 지정선
            Gizmos.DrawWireSphere(this.transform.position, FindDistance);

            //시야 지정선
            Vector3 lookDir = AngleToDirZ(viewRotateZ);
            ADebug.DrawRay(this.transform.position, lookDir * FindDistance, Color.green);

            ADebug.DrawRay(this.transform.position, horizontalLeftDir * FindDistance, Color.cyan);
            ADebug.DrawRay(this.transform.position, horizontalRightDir * FindDistance, Color.cyan);

            //센터지정선
            if (m_SightSet)
            {
                Vector3 centerAngleDir = AngleToDirZ(-CenterAngle);
                ADebug.DrawRay(this.transform.position, centerAngleDir * FindDistance, Color.blue);

                horizontalScanRight = AngleToDirZ(-scanHalfAngle + -CenterAngle);
                horizontalScanLeft = AngleToDirZ(scanHalfAngle + -CenterAngle);

                ADebug.DrawRay(this.transform.position, horizontalScanRight * FindDistance, Color.blue);
                ADebug.DrawRay(this.transform.position, horizontalScanLeft * FindDistance, Color.blue);
            }
            else
            {
                ADebug.DrawRay(this.transform.position, curhorizontalScanRight * FindDistance, Color.blue);
                ADebug.DrawRay(this.transform.position, curhorizontalScanLeft * FindDistance, Color.blue);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            m_Player = GameObject.Find("Player");

            m_SightSet = false;
            curHP = maxHP;
            CurAtkDelay = AtkDelay;
            Rotvec = new Vector3(0, 0, 1);
            horizontalScanRight = AngleToDirZ(-scanHalfAngle + CenterAngle);    
            horizontalScanLeft = AngleToDirZ(scanHalfAngle + CenterAngle);

            //처음 시작 시 중앙을 바라보게
            halfScanAngleFloatRight = CenterAngle + (scanAngle * 0.5f);
            halfScanAngleFloatLeft = CenterAngle - (scanAngle * 0.5f);
            if (halfScanAngleFloatRight > 360)
                halfScanAngleFloatRight -= 360;

            if (halfScanAngleFloatLeft < 0)
                halfScanAngleFloatLeft = 360 + halfScanAngleFloatLeft;
            this.transform.eulerAngles = new Vector3(0, 0, CenterAngle);

            turretCurRotSpeed = turretRotSpeed;

            shieldCtrl = GetComponentInChildren<ShieldCtrl>();
            shiledOldhp = shieldCtrl.hp;
        }

        // Update is called once per frame
        void Update()
        {
            DistanceFromPlayerUpdate();
            viewRotateZ = this.transform.rotation.z * -1;

            if (FindPlayer)
                ShootUpdate();
            else if (FindPlayer == false && IsHitted == true)
                HittedFromPlayer();
            else if (FindPlayer == false && IsHitted == false)
                ScrollTurret();
        }

        void DistanceFromPlayerUpdate()
        {
            if (m_Player == null)
                return;

            CalcVec = m_Player.transform.position - this.transform.position;
            CalcVec.z = 0;
            DistanceFromPlayer = CalcVec.magnitude;

            if (DistanceFromPlayer < FindDistance)
            {
                TurretSight();
            }
            else
            {
                if (FindPlayer)
                {
                    ReturnAngle();
                }
            }
        }

        private void FixedUpdate()
        {
            CheckShieldHitted();
        }

        void ReturnAngle()   //적 발견 후 놓쳤을 때 앵글 방향으로 이동
        {
            if (FindPlayer == false)
                return;

            float leftDist = Mathf.Abs(this.transform.eulerAngles.z - halfScanAngleFloatLeft);
            float rightDist = Mathf.Abs(this.transform.eulerAngles.z - halfScanAngleFloatRight);

            if (leftDist > 180)     //각도문제 수정용
                leftDist -= 180;

            if (leftDist < rightDist)
                bTurnRight = false;
            else
                bTurnRight = true;

            turretCurRotSpeed = turretRotSpeed * 2;
            FindPlayer = false;
        }

        void ScrollTurret() //터렛 회전시키는 함수
        {
            if (bTurnRight) //회전
            {
                this.transform.Rotate(-Rotvec * turretCurRotSpeed * Time.deltaTime);

                if (Mathf.Abs(this.transform.eulerAngles.z - halfScanAngleFloatLeft) <= 1.0f)
                {
                    bTurnRight = false;

                    if (turretCurRotSpeed != turretRotSpeed)
                        turretCurRotSpeed = turretRotSpeed;
                }
            }
            else
            {
                this.transform.Rotate(Rotvec * turretCurRotSpeed * Time.deltaTime);

                if (Mathf.Abs(this.transform.eulerAngles.z - halfScanAngleFloatRight) <= 1.0f)
                {
                    bTurnRight = true;

                    if (turretCurRotSpeed != turretRotSpeed)
                        turretCurRotSpeed = turretRotSpeed;
                }

            }

        }

        // 입력한 -180~180의 값을 Up Vector 기준 Local Direction으로 변환시켜줌.
        private Vector3 AngleToDirZ(float angleInDegree)
        {
            float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
        }

        /// <summary>
        /// 터렛의 시야처리 함수
        /// </summary>
        void TurretSight()
        {
            Vector2 originPos = transform.position;

            if (DistanceFromPlayer < FindDistance)
            {
                Vector2 dir = CalcVec.normalized;
                Vector2 lookDir = AngleToDirZ(viewRotateZ);

                float dot = Vector2.Dot(lookDir, dir);
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                if (angle < horizontalViewHalfAngle)
                {
                    RaycastHit2D rayHitedTarget = Physics2D.Raycast(originPos, dir, DistanceFromPlayer, obstacleMask);
                    if (rayHitedTarget)
                    {
                        if (m_bDebugMode)
                            Debug.DrawLine(originPos, rayHitedTarget.point, Color.yellow);
                        if (FindPlayer)
                            ReturnAngle();
                    }
                    else
                    {
                        if (m_bDebugMode)
                            Debug.DrawLine(originPos, m_Player.transform.position, Color.red);
                        FindPlayer = true;
                        IsHitted = false;   //공격당해서 발견시 끄기
                        LookPlayer();
                    }
                }
            }

        }

        public void HittedEnemy(float stuntime)    // 공격당할시 불러오는 함수
        {
            if (IsHitted == false)
                IsHitted = true;

            lookTimer = 5.0f;
            FindDistance = 10.0f;
            //FindDistance = 15.0f;
        }

        public void HittedFromPlayer() //공격당할시 플레이어 바라보기
        {
            if (IsHitted)
            {
                LookPlayer();
                if (CalcVec.magnitude > FindDistance)
                {
                    lookTimer -= Time.deltaTime;
                    if (lookTimer <= 0.0f)
                    {
                        FindDistance = 7.0f;
                        IsHitted = false;
                    }
                }
                else
                    lookTimer = 5.0f;
            }

        }

        public void LookPlayer()
        {
            Vector2 dir = m_Player.transform.position - transform.position;
            float angle = GetAngle(dir);
            Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, angleAxis, 0.03f);
        }

        void ShootUpdate()
        {
            if (AtkDelay <= 0.0f)
                return;

            if (shotPoint == null)
                return;

            if (CurAtkDelay >= 0.0f)
            {
                CurAtkDelay -= Time.deltaTime;
                if (CurAtkDelay <= 0.0f)
                {
                    Debug.Log("Shoot");
                    float rot = Mathf.Rad2Deg * (Mathf.Atan2(CalcVec.normalized.y, CalcVec.normalized.x));

                    GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, Quaternion.AngleAxis(rot - 90, Vector3.forward));
                    obj?.GetComponent<TrailedBullet>().SetBullet(8.0f, .5f, 5);

                    CurAtkDelay = AtkDelay;
                }
            }
            LookPlayer();
        }


        public void OnDamage(JSONNode node, string key, Vector3? pos, WeaponType type = WeaponType.Null, bool isSplash = false, float gauge = 0.0f)
        {
            if (isSplash)
            {
                if (!GlobalData.DataValidation(node[key][GlobalData.minExplosionDamage], out int expMinDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.explosionDamage], out int expDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.explosionDamageMaxRestore], out int expRst)) return;

                if (expRst > 0)
                {
                    float interval = .0f;
                    interval = (expDam - expMinDam) / expRst;

                    //int level = GlobalData.LauncherData.stdRocketRst.b;
                    //interval *= level;
                    //expMinDam += (int)interval;
                }

                int expDamage = Random.Range(expMinDam, expDam + 1);
                curHP -= expDamage;
            }
            else
            {
                if (!GlobalData.DataValidation(node[key][GlobalData.minDamage], out int minDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.damage], out int dam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.damageMaxRestore], out int damRst)) return;

                if (damRst > 0)
                {
                    float interval = .0f;
                    interval = (dam - minDam) / damRst;

                    //int level = GlobalData.LauncherData.stdRocketRst.a;
                    //interval *= level;
                    //minDam += (int)interval;

                    //print($"{interval} : {minDam}");
                }
                int damage = Random.Range(minDam, dam + 1);
                curHP -= damage;

                if (curHP <= 0.0f)
                    this.gameObject.SetActive(false);
            }
        }

        void CheckShieldHitted()
        {
            if(shiledOldhp == shieldCtrl.hp)
            {
                return;
            }
            else
            {
                Debug.Log("쉴드 공격당함");
                shiledOldhp = shieldCtrl.hp;
                IsHitted = true;
                lookTimer = 5.0f;
                FindDistance = 10.0f;
            }
        }

    }
}



