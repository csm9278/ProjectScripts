using UnityEngine;
using Pathfinding;
using Altair_Memory_Pool_Pro;

namespace Mutant
{
    public class EnemyAttack_2 : MonoBehaviour
    {
        public float m_CurAttackDelay;
        public float m_MaxAttackDelay;
        public float attackInDelay;
        public int attackDmg;

        GameObject player;
        Vector2 targetVec = Vector2.zero;

        [HideInInspector] public bool findAttack = true;

        public LayerMask playerMask;
        public LayerMask obstacleMask;

        // 공격 딜레이 동안 움직임 변수
        [HideInInspector] public AIPath _aipath;
        Animator _animator;
        int findPlayerHash = Animator.StringToHash("FindPlayer");
        int curAttackTimer = Animator.StringToHash("CurAttackTimer");

        //공격 애니메이션
        AlienCreatureCharacter _alienCtrl;

        //공격 움직임 좌표 설정 변수
        float attackPosRandRange = 3.0f;    //최대 좌표값

        //슬로우 뮤턴트 전용 변수
        public GameObject shootObj;
        public GameObject shootPoint;

        public bool noAttacking = false;

        EnemySight sight;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            m_CurAttackDelay = m_MaxAttackDelay;

            if (attackDmg <= 0)
                attackDmg = 50;

            player = GameObject.Find("Player");


            if (_aipath == null)
                this.gameObject.TryGetComponent(out _aipath);

            if (_animator == null)
                this.gameObject.TryGetComponent(out _animator);

            if (sight == null)
                this.gameObject.TryGetComponent(out sight);

            if (_alienCtrl == null)
                _alienCtrl = this.gameObject.GetComponentInChildren<AlienCreatureCharacter>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if(_alienCtrl.attacking)
            {
                if(targetVec.magnitude <= 2.5f)
                {
                    if (sight.attackTarget.TryGetComponent(out IDamageable targetDamageable))
                    {
                        targetDamageable.OnDamage(GlobalData.meleeData, "Nerp Gajhe", this.transform.position, WeaponType.Null);
                    }
                    else
                    {
                        Debug.Log("Idamageable 없음");
                        Debug.Log(sight.attackTarget.gameObject.name);
                    }

                    findAttack = false;

                    attackInDelay = 0.5f;
                    _animator.SetFloat(curAttackTimer, attackInDelay);
                    _alienCtrl.attacking = false;
                }
                else
                {
                    Debug.Log(targetVec.magnitude);
                    _alienCtrl.attacking = false;
                }
            }
            else
            {
                if (m_CurAttackDelay >= 0.0f)
                {
                    m_CurAttackDelay -= Time.deltaTime;
                }
            }
        }

        public void LookPlayerFunc()
        {
            if (sight.attackTarget == null)
                return;

            targetVec = new Vector2(this.transform.position.x - sight.attackTarget.transform.position.x,
                                      this.transform.position.y - sight.attackTarget.transform.position.y);

            float angle = Mathf.Atan2(targetVec.y, targetVec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
            transform.rotation = rotation;
        }

        public void EnemyAttackFunc()
        {
            if (noAttacking)
                return;

            if (m_CurAttackDelay <= 0.0f)
            {

                float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
                Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);

                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vec, 1.0f, obstacleMask);
                if (!hit)
                {
                    hit = Physics2D.Raycast(this.transform.position, vec, 2.5f, playerMask);
                    if(hit)
                    {
                        _alienCtrl.Attack(); //공격 애니메이션 실행
                        m_CurAttackDelay = m_MaxAttackDelay;
                    }
                }
            }

            //if (findAttack)
            //{
            //    float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
            //    Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);

            //    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vec, 3.0f, obstacleMask);
            //    if (!hit)
            //    {
            //        hit = Physics2D.Raycast(this.transform.position, vec, 3.0f, playerMask);
            //        if (hit)
            //        {
            //            if (sight.attackTarget.TryGetComponent(out IDamageable targetDamageable))
            //            {
            //                targetDamageable.OnDamage(GlobalData.meleeData, "Nerp Gajhe", hit.point, WeaponType.Null);
            //            }

            //            m_CurAttackDelay = m_MaxAttackDelay;
            //            findAttack = false;
            //        }
            //    }
            //}
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


        /// <summary>
        /// 슬로우 뮤턴트의 공격용 변수
        /// </summary>
        public void ShootSlowWeb()
        {
            if (m_CurAttackDelay <= 0.0f)
            {
                float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
                Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);

                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vec, 9.0f, playerMask);
                if(hit)
                {
                    GameObject obj = MemoryPoolManager.instance.GetObject("SlimeShot", shootPoint.transform.position);

                    if (obj.TryGetComponent(out SlowWebBullet bullet))
                    {
                        bullet.SetTarget(player.transform.position, player);
                    }

                    m_CurAttackDelay = m_MaxAttackDelay;
                }
            }
        }


    }   //class End
}