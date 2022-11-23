using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;
using Altair_Memory_Pool_Pro;
using System.Runtime.InteropServices;
using SimpleJSON;

namespace Turret
{
    public class Turret : Target, IHitted, IToxic
    {
        public enum TurretType
        {
            Normal,
            Shield
        }

        GameObject m_Player = null;

        public TurretType turretType = TurretType.Normal;
        public bool m_bDebugMode = true;
        public bool m_SightSet = true;

        public float FindDistance = 0.0f;   //�ͷ� �߰� ����
        public float AtkDelay = 0.0f;       //�ͷ� ���� �ӵ�
        public Transform shotPoint = null;  //�ͷ� ���� ����Ʈ

        float CurAtkDelay = 0.0f;
        float DistanceFromPlayer = 0.0f;    //�÷��̾���� �Ÿ�
        [SerializeField] bool FindPlayer = false;
        Vector3 CalcVec = Vector3.zero;

        //�ͷ� ȸ��
        [Header("--- Scroll Turret ---")]
        [Range(0, 360.0f)]
        [SerializeField] private float horizontalViewAngle;
        float horizontalViewHalfAngle;

        //���� ����Ʈ ����
        [Range(0, 360.0f)]
        [SerializeField] float scanAngle;
        [Range(0, 360.0f)]
        [SerializeField] float CenterAngle;
        float scanHalfAngle;
        Vector3 horizontalScanRight;
        Vector3 horizontalScanLeft;
        bool bTurnRight = false;
        float halfScanAngleFloatRight;  // +����
        float halfScanAngleFloatLeft;   // -����

        //�þ��� ����� ����
        Vector3 curhorizontalScanRight;
        Vector3 curhorizontalScanLeft;

        public bool IsHitted = false;
        float lookTimer;

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

        //�νĿ� ����
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        ShieldCtrl shieldCtrl;
        bool distUpgrade = false;   //�÷��̾ �߰����� �� ���ø��� �ѹ��� �þ߹��� ���


        protected override void Awake()
        {
            base.Awake();
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

            //���� ������
            Gizmos.DrawWireSphere(this.transform.position, FindDistance);

            //�þ� ������
            Vector3 lookDir = AngleToDirZ(viewRotateZ);
            ADebug.DrawRay(this.transform.position, lookDir * FindDistance, Color.green);

            ADebug.DrawRay(this.transform.position, horizontalLeftDir * FindDistance, Color.cyan);
            ADebug.DrawRay(this.transform.position, horizontalRightDir * FindDistance, Color.cyan);

            //����������
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
            CurAtkDelay = AtkDelay;
            Rotvec = new Vector3(0, 0, 1);
            horizontalScanRight = AngleToDirZ(-scanHalfAngle + CenterAngle);
            horizontalScanLeft = AngleToDirZ(scanHalfAngle + CenterAngle);

            //ó�� ���� �� �߾��� �ٶ󺸰�
            halfScanAngleFloatRight = CenterAngle + (scanAngle * 0.5f);
            halfScanAngleFloatLeft = CenterAngle - (scanAngle * 0.5f);
            if (halfScanAngleFloatRight > 360)
                halfScanAngleFloatRight -= 360;

            if (halfScanAngleFloatLeft < 0)
                halfScanAngleFloatLeft = 360 + halfScanAngleFloatLeft;
            this.transform.eulerAngles = new Vector3(0, 0, CenterAngle);

            turretCurRotSpeed = turretRotSpeed;

            enemyName = "Turret";
            InitStatus();

            if (turretType == TurretType.Shield)
                shieldCtrl = GetComponentInChildren<ShieldCtrl>();
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

            ToxicEff();

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
                turretSight();
            }
            else
            {
                if (FindPlayer)
                {
                    CheckAngleDist();
                }
            }
        }

        void CheckAngleDist()   //�� �߰� �� ������ �� �ޱ� �������� �̵�
        {
            if (FindPlayer == false)
                return;

            float leftDist = Mathf.Abs(this.transform.eulerAngles.z - halfScanAngleFloatLeft);
            float rightDist = Mathf.Abs(this.transform.eulerAngles.z - halfScanAngleFloatRight);

            if (leftDist > 180)     //�������� ������
                leftDist -= 180;

            if (leftDist < rightDist)
                bTurnRight = false;
            else
                bTurnRight = true;

            turretCurRotSpeed *= 2;
            FindPlayer = false;
            RootMain.GameManager.instance.DecreaseBattleNum();   //�� �������� ����
        }

        void ScrollTurret() //�ͷ� ȸ����Ű�� �Լ�
        {
            if (bTurnRight) //ȸ��
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

        // �Է��� -180~180�� ���� Up Vector ���� Local Direction���� ��ȯ������.
        private Vector3 AngleToDirZ(float angleInDegree)
        {
            float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
        }

        void turretSight()
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
                        {
                            CheckAngleDist();
                        }
                    }
                    else
                    {
                        if (m_bDebugMode)
                            Debug.DrawLine(originPos, m_Player.transform.position, Color.red);
                        if(!FindPlayer)
                        {
                            if (!distUpgrade)
                            {
                                distUpgrade = true;
                                FindDistance *= 1.5f;
                            }

                            RootMain.GameManager.instance.AddBattleNum();   //�� �������� �߰�
                            FindPlayer = true;
                        }
                        IsHitted = false;   //���ݴ��ؼ� �߽߰� ����
                        LookPlayer();
                    }
                }
            }

        }

        public void HittedEnemy(float stuntime)    // ���ݴ��ҽ� �ҷ����� �Լ�
        {
            if (IsHitted == false)
                IsHitted = true;

            if(!distUpgrade)
            {
                distUpgrade = true;
                FindDistance *= 1.5f;
            }

            lookTimer = 5.0f;
        }

        public void HittedFromPlayer() //���ݴ��ҽ� �÷��̾� �ٶ󺸱�
        {
            if (IsHitted)
            {
                LookPlayer();
                if (CalcVec.magnitude > FindDistance)
                {
                    lookTimer -= Time.deltaTime;
                    if (lookTimer <= 0.0f)
                        IsHitted = false;
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
            transform.rotation = Quaternion.Lerp(this.transform.rotation, angleAxis, 0.3f);
        }

        void ShootUpdate()
        {
            if (AtkDelay <= 0.0f)
                return;

            if (shotPoint == null)
                return;

            if (SwitchManager.GetSwitch("BackOriginMonster"))
                return;

            if (CurAtkDelay >= 0.0f)
            {
                CurAtkDelay -= Time.deltaTime;
                if (CurAtkDelay <= 0.0f)
                {
                    float rot = Mathf.Rad2Deg * (Mathf.Atan2(CalcVec.normalized.y, CalcVec.normalized.x));

                    GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, Quaternion.AngleAxis(rot - 90, Vector3.forward));
                    obj?.GetComponent<TrailedBullet>().SetBullet(8.0f, .5f, 5);

                    CurAtkDelay = AtkDelay;
                }
            }

            LookPlayer();
        }

        public override void OnDamage(JSONNode node, string key, Vector3? pos, WeaponType type = WeaponType.Null, bool isSplash = false, float gauge = 0.0f)
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
                hp -= expDamage;
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
                hp -= damage;
            }

            if (hp <= 0)
            {
                if(FindPlayer) //���� �� ����� �������� ���Ҹ� ����
                {
                    RootMain.GameManager.instance.DecreaseBattleNum();   //�� �������� ����
                }

                gameObject.SetActive(false);

            }
        }

        public void ToxicEnemy(float toxicTime = 0)
        {
            if (this.toxicTime <= 0.0f)
                this.toxicTime = 2.0f;
            else
                this.toxicTime += toxicTime;

            if (this.toxicTime >= maxToxicTime)
                this.toxicTime = maxToxicTime;

            toxicEff.Play();
        }

        public void ToxicEff()
        {
            if (toxicTime >= 0.0f)
            {
                toxicTime -= Time.deltaTime;
                curToxicDamageTime -= Time.deltaTime;

                if (curToxicDamageTime <= 0.0f)
                {
                    if (aspect.characterAspect == Aspect.CharacterAspect.LivingThing)
                    {
                        ADebug.Log("���� ������");
                        curToxicDamageTime = toxicDamageTime;
                    }
                    else if (aspect.characterAspect == Aspect.CharacterAspect.Machine)
                    {
                        ADebug.Log("���� �߰� ������");
                        curToxicDamageTime = toxicDamageTime;
                    }
                }

                if (toxicTime <= 0.0f)
                {
                    toxicEff.Stop();
                }
            }
        }
    }
}



