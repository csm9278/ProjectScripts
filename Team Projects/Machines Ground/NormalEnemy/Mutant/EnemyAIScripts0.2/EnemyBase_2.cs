using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Item;
using SimpleJSON;

namespace Mutant
{
    public class EnemyBase_2 : Target, IHitted, IBurned, IToxic
    {
        public enum MutantType
        {
            Normal,
            Armor,
            Stealth,
            Slow,
            Mother,
            Baby,
            Dash,
            Count
        }

        public MutantType type = MutantType.Count;
        public Transform[] m_Poses;
        [HideInInspector] public int m_Idx = 1;
        [HideInInspector] public AIPath _aipath = null;

        //주인공 놓침
        [HideInInspector] public Vector2 lastSeePos;

        //플레이어 거리 계산 변수
        [HideInInspector] public GameObject m_Player = null;
        [HideInInspector] public Vector3 targetVec = Vector3.zero;
        [HideInInspector] public float distanceFromPlayer = 0.0f;
        EnemySight sight;

        //공격 변수
        float StunTime = 0.0f;
        [HideInInspector] public bool m_AttackedFromPlayer = false;  //공격당했을 시
        float m_AttackChaseTime = 5.0f;

        AlienCreatureCharacter _alienCtrl;

        //불타오르기용 변수
        public ParticleSystem burningEff;
        float burnTime = -0.1f;
        float burnDamageTime = 1.0f;
        float curBurnDamageTime = 1.0f;

        //부식용 변수
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        //돌진형
        public bool notMissingTarget = false;
        [HideInInspector] public bool isnotKnockBack = false;

        private void OnDrawGizmos()
        {
            //for (int i = 0; i < m_Poses.Length -1; i++)
            //{
            //    Debug.DrawLine(m_Poses[i].transform.position, m_Poses[i + 1].transform.position);
            //}
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Poses == null)
                m_Poses = GameObject.Find("EnemyPoses").GetComponentsInChildren<Transform>();
            _aipath = GetComponent<AIPath>();
            //_animator = GetComponent<Animator>(); //Target쪽에서 찾아놓는다 주석처리
            m_Player = GameObject.Find("Player");

            if (_alienCtrl == null)
                _alienCtrl = GetComponentInChildren<AlienCreatureCharacter>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (hp == 0)
                hp = 150;

            enemyName = GlobalData.MutantNames[(int)type];
            InitStatus(_aipath);

            itemnode = GetComponent<ItemNode>();
            sight = GetComponent<EnemySight>();

            if (notMissingTarget)
                _animator.SetBool("notMissing", true);

            if (forceAttackPlayer)
                _animator.SetBool("ForceAttack", forceAttackPlayer);
        }

        // Update is called once per frame
        void Update()
        {
            if (SwitchManager.GetSwitch("BackOriginMonster")) backOriginPos = true;

            checkBackOrigin();
            CheckDistFromPlayer();
            StunFunc();
            TargetContact();

            BurrningEff();  //화염 데미지 함수
            ToxicEff();     //부식 데미지 함수

        }

        public void CheckDistFromPlayer()
        {
            if (sight.attackTarget == null)
                return;

            targetVec = sight.attackTarget.transform.position - this.transform.position;
            targetVec.z = 0;
            distanceFromPlayer = targetVec.magnitude;
            _animator.SetFloat("DistanceFromPlayer", distanceFromPlayer);

        }

        /// <summary>
        /// 공격 당했을 시 AI임시 비활성화 및 스턴 시간 계산
        /// </summary>
        /// <param name="a_stuntime">적 경직 시간</param>
        public void HittedEnemy(float a_stuntime = 0.0f)
        {
            if (a_stuntime > 0)
            {
                if (type == MutantType.Armor)
                    return;

                if(isnotKnockBack)
                    return;

                _aipath.enabled = false;
                StunTime = a_stuntime;
            }

            if (m_AttackedFromPlayer)   //이미 공격상태일때는 리턴
                return;
            else
            {
                _animator.SetTrigger("Hitted");
                _aipath.endReachedDistance = 4.0f;
            }
        }

        /// <summary>
        /// 플레이어를 발견했을때 동작하는 함수입니다. StateMachine에서 불러집니다.
        /// </summary>
        public void FindEnemy()
        {
            if (m_AttackedFromPlayer)   //이미 발견 상태면 Return;
                return;

            m_AttackedFromPlayer = true;
            _animator.SetBool("AttackedFromPlayer", m_AttackedFromPlayer);
            m_AttackChaseTime = 8.0f;   //추적 시간
            _aipath.endReachedDistance = 4.0f;

            RootMain.GameManager.instance.AddBattleNum();
        }

        /// <summary>
        /// 스턴시간체크 후 다시 AI활성화해 추격
        /// </summary>
        public void StunFunc()
        {
            if (StunTime >= 0.0f)
            {
                StunTime -= Time.deltaTime;
                if (StunTime <= 0.0f)
                {
                    _aipath.enabled = true;
                }
            }
        }

        public void AttChaseDelayFunc() //공격 당할 시 추격시간동안 추격 현재 사용안함
        {
            if (m_AttackChaseTime >= 0.0f)
            {
                m_AttackChaseTime -= Time.deltaTime;
                if (m_AttackChaseTime <= 0.0f)
                {
                    m_AttackedFromPlayer = false;
                    _animator.SetBool("AttackedFromPlayer", m_AttackedFromPlayer);
                    _aipath.endReachedDistance = 0.2f;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if(coll.gameObject.TryGetComponent(out Aspect aspect))
            {
                if(aspect.characterAspect != Aspect.CharacterAspect.Player)
                {
                    isContactTarget = true;
                    contactTimer = 2.0f;
                }
            }
        }

        /// <summary>
        /// 플레이어 강제공격 함수
        /// </summary>
        public void SetForceAttack(bool setForce)
        {
            forceAttackPlayer = setForce;
            if (forceAttackPlayer)
                _animator.SetBool("ForceAttack", forceAttackPlayer);
        }

        void BurrningEff()
        {
            if(burnTime >= 0.0f)
            {
                burnTime -= Time.deltaTime;
                curBurnDamageTime -= Time.deltaTime;

                if(curBurnDamageTime <= 0.0f)
                {
                    hp -= RootMain.DamageCal.OnDotDamage(GlobalData.flameData, GlobalData.weaponKeys[1], this.transform.position);
                    curBurnDamageTime = burnDamageTime;
                }

                if (burnTime <= 0.0f)
                {
                    burningEff.Stop();
                }
            }
        }

        public void BurnedEnemy(JSONNode node, string key, float burnningTime = 0)
        {

            if (!GlobalData.DataValidation(node[key][GlobalData.duration], out int duration)) return ;
            if (!GlobalData.DataValidation(node[key][GlobalData.minDuration], out int minDuration)) return;

            if (burnTime < minDuration)
            {
                burnTime = minDuration;
                burningEff.Play();
            }
            else
                burnTime += burnningTime;

            if (burnTime >= duration)
                burnTime = duration;
        }


        public override void AddDeathEffect()
        {
            _aipath.enabled = false;

            if(m_AttackedFromPlayer)
            {
                RootMain.GameManager.instance.DecreaseBattleNum();
                m_AttackedFromPlayer = false;
            }

            if (this.TryGetComponent(out DropParticle particle))
                particle.enabled = false;

            MutantGas gas = this.gameObject.GetComponentInChildren<MutantGas>();
            if(gas != null)
            {
                gas.enabled = false;
                gas.gameObject.GetComponent<ParticleSystem>().Stop();
            }
        }

        public void ToxicEnemy(float toxicTime = 0)
        {
            if (this.toxicTime <= 0.0f)
            {
                this.toxicTime = 2.0f;
                toxicEff.Play();
            }
            else
                this.toxicTime += toxicTime;

            if (this.toxicTime >= maxToxicTime)
                this.toxicTime = maxToxicTime;
        }

        public void ToxicEff()
        {
            if (toxicTime >= 0.0f)
            {
                toxicTime -= Time.deltaTime;
                curToxicDamageTime -= Time.deltaTime;

                if (curToxicDamageTime <= 0.0f)
                {

                    if(aspect.characterAspect == Aspect.CharacterAspect.LivingThing)
                    {
                        ADebug.Log("독성 데미지");
                        curToxicDamageTime = toxicDamageTime;
                    }
                    else if(aspect.characterAspect == Aspect.CharacterAspect.Machine)
                    {
                        ADebug.Log("독성 추가 데미지");
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