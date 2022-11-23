using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class BabyMutantAttack : MonoBehaviour
    {
        public float m_CurAttackDelay;
        public float m_MaxAttackDelay;
        public float attackInDelay;
        public int attackDmg;

        IDamageable playerDamageable;

        [HideInInspector] public bool findAttack = true;

        public LayerMask playerMask;
        public LayerMask obstacleMask;

        BabyMutantBase babyBase;

        Vector3 playerVec;

        //공격 애니메이션
        AlienCreatureCharacter _alienCtrl;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (babyBase == null)
                this.gameObject.TryGetComponent(out babyBase);

            m_CurAttackDelay = m_MaxAttackDelay;

            if (attackDmg <= 0)
                attackDmg = 25;

            if (playerDamageable == null)
                babyBase.player.TryGetComponent(out playerDamageable);

            if (_alienCtrl == null)
                _alienCtrl = this.gameObject.GetComponentInChildren<AlienCreatureCharacter>();

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (m_CurAttackDelay >= 0.0f)
            {
                m_CurAttackDelay -= Time.deltaTime;
            }

            if (babyBase.canAttack)
            {
                LookPlayerFunc();
                EnemyAttackFunc();
            }
        }

        public void LookPlayerFunc()
        {
            playerVec = new Vector2(this.transform.position.x - babyBase.player.transform.position.x,
                                        this.transform.position.y - babyBase.player.transform.position.y);

            float angle = Mathf.Atan2(playerVec.y, playerVec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
            transform.rotation = rotation;
        }

        public void EnemyAttackFunc()
        {
            if (m_CurAttackDelay <= 0.0f)
            {
                float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
                Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);

                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vec, 1.5f, playerMask);
                //Gizmos.DrawRay(this.transform.position, vec * 2.5f);

                if (hit)
                {
                    _alienCtrl.Attack();

                    if (playerDamageable != null)
                    {
                        //playerDamageable.OnDamage(attackDmg, null, WeaponType.Null);
                        playerDamageable.OnDamage(GlobalData.meleeData, "Baby Gajhe", hit.point, WeaponType.Null);
                        ADebug.Log("AttackSuccess");
                        m_CurAttackDelay = m_MaxAttackDelay;
                        findAttack = false;
                    }
                }

            }

            if (findAttack)
            {
                float radian = (this.transform.rotation.z - transform.eulerAngles.z) * Mathf.Deg2Rad;
                Vector3 vec = new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);

                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vec, 1.5f, playerMask);
                if (hit)
                {
                    _alienCtrl.Attack();

                    if (playerDamageable != null)
                    {
                        playerDamageable.OnDamage(GlobalData.meleeData, "Baby Gajhe", hit.point, WeaponType.Null);
                        ADebug.Log("AttackSuccess");
                        m_CurAttackDelay = m_MaxAttackDelay;
                        findAttack = false;
                    }
                }

            }
        }
    }
}