using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
using Weapon;
using Pathfinding;

namespace Drone
{
    public class DroneAttack : MonoBehaviour
    {
        public enum DroneType
        {
            Gun,
            ShotGun,
            Rifle,
            SlowShot,
            Rocket,
            Grenade,
            Gatling,
            GunRocket
        }

        public DroneType droneType = DroneType.Rifle;
        public float m_CurAttackDelay;
        public float m_MaxAttackDelay;
        public float attackInDelay;
        float attackDistance;
        public Transform shotPoint;
        int attackTime = 0;
        float ChaseSpeed = 10.0f;   //회전 속도

        // 공격 딜레이 동안 움직임 변수
        [HideInInspector ]public AIPath _aipath;
        public Animator _animator;
        int findPlayerHash = Animator.StringToHash("FindPlayer");
        int curAttackTimer = Animator.StringToHash("CurAttackTimer");
        int attackTimeHash = Animator.StringToHash("GatlingTimer");

        //공격 움직임 좌표 설정 변수
        float attackPosRandRange = 3.0f;    //최대 좌표값

        [HideInInspector] public GameObject m_RefPlayer;
        Vector2 m_PlayerVec = Vector2.zero;

        [HideInInspector] public bool findAttack = true;

        [SerializeField] private LayerMask m_viewTargetMask;
        [SerializeField] private LayerMask obstacleMask;

        public bool isTargetOn = true;

        //개틀링형 변수
        float attackTimer = -0.1f;
        float gatlingCharge;

        //로켓형 변수
        public Transform[] rocketPoint;
        int rocketIdx = 0;
        //건로켓 변수
        bool gunAttack = false;
        int gunAttackNum = 0;
        public Transform[] weaponPos;

        //Muzzle Flash
        public ParticleSystem muzzleFlash;
        public ParticleSystem[] rocketmuzzleFlashs;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            m_CurAttackDelay = m_MaxAttackDelay;
            if (droneType == DroneType.Gatling)
            {
                gatlingCharge = m_MaxAttackDelay;
                ChaseSpeed = 8;
            }

            m_RefPlayer = GameObject.Find("Player");

            if (_aipath == null)
                this.gameObject.TryGetComponent(out _aipath);

            if (_animator == null)
                this.gameObject.TryGetComponent(out _animator);

            attackDistance = this.gameObject.GetComponent<EnemySight>().viewRadius;

            obstacleMask = ~obstacleMask;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (m_CurAttackDelay >= 0.0f && attackInDelay <= 0.0f)
            {
                m_CurAttackDelay -= Time.deltaTime;
            }
        }

        public void LookPlayerFunc()
        {
            if (!_animator.GetBool(findPlayerHash))
                return;

            m_PlayerVec = (m_RefPlayer.transform.position - this.transform.position) * -1;

            float angle = Mathf.Atan2(m_PlayerVec.y, m_PlayerVec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, ChaseSpeed * Time.deltaTime);
            transform.eulerAngles = rotation.eulerAngles;
        }

        public void EnemyAttackFunc()
        {
            if (m_CurAttackDelay <= 0.0f)
            {
                if(isTargetOn)
                {
                    switch (droneType)
                    {
                        case DroneType.Gun:
                            GunAttack();
                            break;

                        case DroneType.ShotGun:
                            ShotGunAttack();
                            break;

                        case DroneType.Rifle:
                            RifleAttack();
                            break;

                        case DroneType.SlowShot:
                            SlowAttack();
                            break;

                        case DroneType.Rocket:
                            RocketAttack();
                            break;

                        case DroneType.Grenade:
                            GranadeAttack();
                            break;

                        case DroneType.Gatling:
                            GatlingAttack();
                            break;

                        case DroneType.GunRocket:
                            GunRocketAttack();
                            break;
                    }
                }
                else
                {
                    //RaycastHit2D hit;
                    //float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
                    //Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
                    //hit = Physics2D.Raycast(shotPoint.position, vec, attackDistance, obstacleMask);
                    //if (hit)
                    //{
                    //    Aspect aspect = hit.collider.GetComponent<Aspect>();
                    //    if (aspect != null)
                    //    {
                    //        if (aspect.characterAspect == Aspect.CharacterAspect.Player)
                    //            isTargetOn = true;
                    //    }
                    //}
                    if (_animator.GetBool(findPlayerHash))
                        isTargetOn = true;
                }
            }
        }

        void GunAttack()
        {
            GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, this.transform.eulerAngles);
            //obj?.GetComponent<BulletController>().SetBullet(8.0f, .5f, 5);
            obj?.GetComponent<TrailedBullet>().SetBullet(attackDistance, 0.5f, 5);

            //findAttack = true;
            m_CurAttackDelay = m_MaxAttackDelay;
            muzzleFlash.Play();

            attackTime++;

            if (attackTime >= 1)    // 5회 공격후에는 딜레이 적용합니다
            {
                attackInDelay = 0.5f;
                _animator.SetFloat(curAttackTimer, attackInDelay);
                attackTime = 0;
                isTargetOn = false;
            }
        }

        void ShotGunAttack()
        {
            for(int i = 0; i < 4; i++)
            {
                GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, this.transform.eulerAngles);
                //obj?.GetComponent<BulletController>().SetBullet(8.0f, .5f, 5);
                obj?.GetComponent<TrailedBullet>().SetBullet(attackDistance, 0.5f, 3);
            }

            //findAttack = true;
            m_CurAttackDelay = m_MaxAttackDelay;
            muzzleFlash.Play();

            attackTime++;

            if (attackTime >= 1)    // 5회 공격후에는 딜레이 적용합니다
            {
                attackInDelay = 1.5f;
                _animator.SetFloat(curAttackTimer, attackInDelay);
                attackTime = 0;
                isTargetOn = false;
            }
        }

        void RifleAttack()
        {
            GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, this.transform.eulerAngles);
            //obj?.GetComponent<BulletController>().SetBullet(8.0f, .5f, 5);
            obj?.GetComponent<TrailedBullet>().SetBullet(attackDistance, 0.5f, 5);

            //findAttack = true;
            m_CurAttackDelay = m_MaxAttackDelay;
            muzzleFlash.Play();

            attackTime++;

            if (attackTime >= 5)    // 5회 공격후에는 딜레이 적용합니다
            {
                attackInDelay = 0.5f;
                _animator.SetFloat(curAttackTimer, attackInDelay);
                attackTime = 0;
                isTargetOn = false;
            }
        }

        void RocketAttack()
        {

            GameObject obj = MemoryPoolManager.instance.GetObject("EnemyMissile", rocketPoint[rocketIdx].position, this.transform.eulerAngles);

            m_CurAttackDelay = m_MaxAttackDelay;
            rocketIdx++;
            attackTime++;
            if (rocketIdx >= rocketPoint.Length)
                rocketIdx = 0;

            if (attackTime >= 10)    // 5회 공격후에는 딜레이 적용합니다
            {
                attackInDelay = 1.0f;
                _animator.SetFloat(curAttackTimer, attackInDelay);
                attackTime = 0;
                isTargetOn = false;
            }
        }

        void GranadeAttack()
        {
            GameObject obj = MemoryPoolManager.instance.GetObject("EnemyGrenade", shotPoint.position, this.transform.eulerAngles);

            if(obj.TryGetComponent(out RootMain.EnemyGrenade grenade))
                grenade.SetDirVector(transform.up);

            //findAttack = true;
            m_CurAttackDelay = m_MaxAttackDelay;

            attackTime++;

            if (attackTime >= 1)    // 5회 공격후에는 딜레이 적용합니다
            {
                attackInDelay = 0.5f;
                _animator.SetFloat(curAttackTimer, attackInDelay);
                attackTime = 0;
                isTargetOn = false;
            }
        }

        void GatlingAttack()
        {
            GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, this.transform.eulerAngles);
            obj?.GetComponent<TrailedBullet>().SetBullet(attackDistance, 0.5f, 5);

            m_CurAttackDelay = gatlingCharge;
            muzzleFlash.Play();

            if (_animator.GetBool(findPlayerHash))
            {
                attackTimer = 5.0f;
                if (gatlingCharge > 0.1f)
                    gatlingCharge *= 0.8f;
                else
                    gatlingCharge = 0.1f;
                _animator.SetFloat(attackTimeHash, attackTimer);
            }
            else
            {
                attackTimer -= m_CurAttackDelay;
                if (gatlingCharge > 0.1f)
                    gatlingCharge *= 0.8f;
                else
                    gatlingCharge = 0.1f;
                _animator.SetFloat(attackTimeHash, attackTimer);
                if (attackTimer <= 0.0f)
                {
                    gatlingCharge = m_MaxAttackDelay;
                    isTargetOn = false;
                }
            }
        }


        void GunRocketAttack()
        {
            if(m_PlayerVec.magnitude < 5.0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, weaponPos[0].transform.eulerAngles);
                    obj?.GetComponent<TrailedBullet>().SetBullet(attackDistance, 0.5f, 3);
                }

                m_CurAttackDelay = m_MaxAttackDelay * 5;

                gunAttackNum++;
                muzzleFlash.Play();

                if (gunAttackNum >= 2)    // 5회 공격후에는 딜레이 적용합니다
                {
                    attackInDelay = 1.0f;
                    _animator.SetFloat(curAttackTimer, attackInDelay);
                    attackTime = 0;
                    isTargetOn = false;
                    gunAttackNum = 0;
                }
            }
            else
            {
                GameObject obj = MemoryPoolManager.instance.GetObject("EnemyMissile", rocketPoint[rocketIdx].position, this.transform.eulerAngles);

                rocketmuzzleFlashs[rocketIdx].Play();
                m_CurAttackDelay = m_MaxAttackDelay;
                rocketIdx++;
                attackTime++;
                if (rocketIdx >= rocketPoint.Length)
                    rocketIdx = 0;

                if (attackTime >= 10)    // 5회 공격후에는 딜레이 적용합니다
                {
                    attackInDelay = 1.0f;
                    _animator.SetFloat(curAttackTimer, attackInDelay);
                    attackTime = 0;
                    isTargetOn = false;
                    gunAttackNum = 0;
                }
            }
        }

        void SlowAttack()
        {
            GameObject obj = MemoryPoolManager.instance.GetObject(0, shotPoint.position, this.transform.eulerAngles);
            //obj?.GetComponent<BulletController>().SetBullet(8.0f, .5f, 5);
            obj?.GetComponent<TrailedBullet>().SetBullet(attackDistance, 0.5f, 5 , true);

            //findAttack = true;
            m_CurAttackDelay = m_MaxAttackDelay;

            attackTime++;

            if (attackTime >= 1)    // 5회 공격후에는 딜레이 적용합니다
            {
                attackInDelay = 0.5f;
                _animator.SetFloat(curAttackTimer, attackInDelay);
                attackTime = 0;
                isTargetOn = false;
            }
        }

        /// <summary>
        /// 공격과 공격사이의 딜레이 체크 함수 스테이트머신에서 불러지고 있음.
        /// </summary>
        public void AttackInDelayFunc()
        {
            if (attackInDelay > 0.0f)
            {
                attackInDelay -= Time.deltaTime;
                _animator.SetFloat(curAttackTimer, attackInDelay);
            }
        }

        /// <summary>
        /// 공격 사이사이 움직임 좌표를 설정하는 함수
        /// </summary>
        public void SetAttackMovePos()
        {
            float randX = Random.Range(this.transform.position.x - attackPosRandRange,
                                       this.transform.position.x + attackPosRandRange);
            float randY = Random.Range(this.transform.position.y - attackPosRandRange,
                                       this.transform.position.y + attackPosRandRange);

            Vector3 movePos = new Vector3(randX, randY, 0);

            _aipath.destination = movePos;
        }
    }
}
