using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Altair_Memory_Pool_Pro;
using Item;
using SimpleJSON;

namespace Mutant
{
    public class MotherMutantBase : Target, IHitted, IBurned, IToxic
    {
        public Transform[] m_Poses;
        [HideInInspector] public int m_Idx = 1;
        [HideInInspector] public AIPath _aipath = null;

        //주인공 놓침
        [HideInInspector] public Vector2 lastSeePos;

        //플레이어 거리 계산 변수
        [HideInInspector] public GameObject player = null;
        Vector3 m_PlayerVec = Vector3.zero;
        float m_DistFromPlayer = 0.0f;

        //공격 변수
        float StunTime = 0.0f;
        public bool m_AttackedFromPlayer = false;  //공격당했을 시
        float m_AttackChaseTime = 5.0f;

        //하위 객체 뮤턴트 (마더 뮤턴트 고유 변수)
        public GameObject[] mutantList;                 //뮤턴트 관리 함수
        [SerializeField] GameObject mutantObj;          //소환할 뮤턴트 오브젝트
        public int maxMutantCount;                      //소환할 뮤턴트의 최대 숫자
        public float mutantsRange;                      //뮤턴트를 데리고 다닐 범위
        [SerializeField] LayerMask obstacleMask;        //소환시에 예외처리할 레이어
        CheckMother checkMother;                        //체크용의 변수
        [SerializeField] float spawnDelay;              //스폰 타이머
        float spawnCurDealy;
        Queue<int> SpawnQueue;                          //순차 스폰을 위한 큐

        float initDelaytimer = 0.2f;

        //쫒아오기 위한 변수
        public Transform[] babyFollowPoses;

        //불타오르기용 변수
        public ParticleSystem burningEff;
        float burnTime = -0.1f;
        float maxburnTime = 10.0f;
        float burnDamageTime = 1.0f;
        float curBurnDamageTime = 1.0f;

        //부식용 변수
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, mutantsRange);
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Poses == null)
                m_Poses = GameObject.Find("EnemyPoses").GetComponentsInChildren<Transform>();
            _aipath = GetComponent<AIPath>();
            player = GameObject.Find("Player");
        }

        // Start is called before the first frame update
        void Start()
        {
            if (hp == 0)
                hp = 150;

            itemnode = GetComponent<ItemNode>();
            enemyName = GlobalData.MutantNames[4];

            spawnCurDealy = spawnDelay;
            SpawnQueue = new Queue<int>();
        }

        // Update is called once per frame
        void Update()
        {
            if (initDelaytimer >= 0.0f)
            {
                initDelaytimer -= Time.deltaTime;
                if (initDelaytimer <= 0.0f)
                    MutantsInit();
            }

            CheckDistFromPlayer();
            StunFunc();

            SpawnTimerUpdate();

            BurrningEff();
            ToxicEff();
        }

        /// <summary>
        /// 처음에 뮤턴트를 소환하고 리스트에 추가해 놓는 함수 Start쪽에서 부를 예정
        /// </summary>
        void MutantsInit()
        {
            mutantList = new GameObject[maxMutantCount];

            for (int i = 0; i < maxMutantCount; i++)
            {
                //GameObject obj = Instantiate(mutantObj);
                //obj.transform.position = babyFollowPoses[i].transform.position;
                GameObject obj = MemoryPoolManager.instance.GetObject("BabyMutant", babyFollowPoses[i].transform.position);

                if(obj.TryGetComponent(out BabyMutantBase babyBase))
                {
                    babyBase.player = this.player;   
                }

                if (obj.TryGetComponent(out checkMother))
                {
                    checkMother.motherBase = this;
                    checkMother.spawnIdx = i;
                }

                //Debug.Log("j : " + j);
                mutantList[i] = obj;
                
            }
        }

        /// <summary>
        /// 스폰딜레이 감소 함수
        /// </summary>
        void SpawnTimerUpdate()
        {
            if (this.hp <= 0.0f)
                return;

            if (spawnCurDealy >= 0.0f)
            {
                spawnCurDealy -= Time.deltaTime;
                if(spawnDelay <= 0.0f && SpawnQueue.Count > 0)
                {
                    SpawnMutant(SpawnQueue.Dequeue());
                    spawnCurDealy = spawnDelay;
                }
            }
            else if (spawnCurDealy <= 0.0f && SpawnQueue.Count > 0)
            {
                SpawnMutant(SpawnQueue.Dequeue());
                spawnCurDealy = spawnDelay;
            }

        }

        /// <summary>
        /// 뮤턴트를 인덱스를 받아서 새로 스폰해주는 함수
        /// </summary>
        /// <param name="Dieidx">죽은 몬스터</param>
        public void SpawnMutant(int Dieidx)
        {
            if (this.hp <= 0.0f)
                return;

            GameObject obj = MemoryPoolManager.instance.GetObject("BabyMutant", babyFollowPoses[Dieidx].position);

            if (obj.TryGetComponent(out checkMother))
            {
                checkMother.motherBase = this;
                checkMother.spawnIdx = Dieidx;
            }

            if (obj.TryGetComponent(out BabyMutantBase babyBase))
            {
                babyBase.player = this.player;
            }

            mutantList[Dieidx] = obj;
        }

        public void ReturnSignal()
        {
            if (mutantList.Length > 0)
            {
                for (int i = 0; i < mutantList.Length; i++)
                {
                    if (mutantList[i].TryGetComponent(out AIPath _aipath))
                    {
                        _aipath.destination = babyFollowPoses[i].position;
                        _aipath.endReachedDistance = 0.2f;
                    }
                }
            }
        }

        /// <summary>
        /// 외부에서 불러진다
        /// </summary>
        /// <param name="Dieidx">죽은 몬스터의 번호</param>
        public void DieSignal(int Dieidx)
        {
            if (this.hp <= 0.0f)
                return;

            if (spawnCurDealy <= 0.0f && SpawnQueue.Count == 0)
            {
                SpawnMutant(Dieidx);
                spawnCurDealy = spawnDelay;
            }
            else
            {
                SpawnQueue.Enqueue(Dieidx);
                //ADebug.Log("InQueue");
            }
        }

        /// <summary>
        /// 팔로우 위치 이동할지 안할지 여부 세팅
        /// </summary>
        /// <param name="isSet"></param>
        public void AiDestSetter(bool isSet, GameObject target = null)
        {
            if (mutantList.Length <= 0)
                return;

            for (int i = 0; i < mutantList.Length; i++)
            {
                if (mutantList[i].TryGetComponent(out AIDestinationSetter aisetter))
                    aisetter.enabled = isSet;

                if (target != null)
                    aisetter.target = target.transform;
            }
        }    

        /// <summary>
        /// 하위 몬스터에서 불러진다 하위 객체가 플레이어를 발견 시 발동
        /// </summary>
        public void FindSignal()
        {
            if (this.hp <= 0.0f)
                return;


            if (mutantList.Length > 0)
            {
                for (int i = 0; i < mutantList.Length; i++)
                {

                    if (mutantList[i].TryGetComponent(out AIPath _aiPath))
                    {
                        _aiPath.destination = player.transform.position;
                        _aiPath.endReachedDistance = 1.7f;
                    }
                }
            }

        }

        public void CheckDistFromPlayer()
        {
            if (player == null)
                return;

            m_PlayerVec = player.transform.position - this.transform.position;
            m_DistFromPlayer = m_PlayerVec.magnitude;
            _animator.SetFloat("DistanceFromPlayer", m_DistFromPlayer);

            //if (m_DistFromPlayer >= 8.0f)
            //{
            //    _animator.SetBool("FindPlayer", false);
            //    m_AttackedFromPlayer = false;
            //}
        }

        /// <summary>
        /// 공격 당했을 시 AI임시 비활성화 및 스턴 시간 계산
        /// </summary>
        /// <param name="a_stuntime">적 경직 시간</param>
        public void HittedEnemy(float a_stuntime = 0.0f)
        {
            if (a_stuntime > 0)
            {
                _aipath.enabled = false;
                StunTime = a_stuntime;
            }

            if (m_AttackedFromPlayer)   //이미 공격상태일때는 리턴
                return;
            else
            {
                _animator.SetTrigger("Hitted");
                //m_AttackedFromPlayer = true;
                //_animator.SetBool("AttackedFromPlayer", m_AttackedFromPlayer);
                _aipath.endReachedDistance = 4.0f;
            }
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

        private void OnCollisionEnter2D(Collision2D coll)
        {
            Aspect targetAspect;
            targetAspect = coll.gameObject.GetComponent<Aspect>();

            if (targetAspect == null)
                return;

            if (targetAspect.characterAspect == Aspect.CharacterAspect.Player)
            {
                _animator.SetTrigger("Hitted");
            }
        }

        public override void AddDeathEffect()
        {
            AiDestSetter(true, player);

            if(m_AttackedFromPlayer)
            {
                m_AttackedFromPlayer = false;
                RootMain.GameManager.instance.DecreaseBattleNum();
            }
        }

        void BurrningEff()
        {
            if (burnTime >= 0.0f)
            {
                burnTime -= Time.deltaTime;
                curBurnDamageTime -= Time.deltaTime;

                if (curBurnDamageTime <= 0.0f)
                {
                    if (aspect.characterAspect == Aspect.CharacterAspect.LivingThing)
                    {
                        ADebug.Log("화염 추가 데미지");
                        curBurnDamageTime = burnDamageTime;
                    }
                    else if (aspect.characterAspect == Aspect.CharacterAspect.Machine)
                    {
                        ADebug.Log("화염 데미지");
                        curBurnDamageTime = burnDamageTime;
                    }
                }

                if (burnTime <= 0.0f)
                {
                    burningEff.Stop();
                }
            }
        }

        public void BurnedEnemy(JSONNode node, string key, float burnningTime = 0)
        {
            if (burnTime <= 0.0f)
                burnTime = 2.0f;
            else
                burnTime += burnningTime;

            if (burnTime >= maxburnTime)
                burnTime = maxburnTime;

            burningEff.Play();
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
                        ADebug.Log("독성 데미지");
                        curToxicDamageTime = toxicDamageTime;
                    }
                    else if (aspect.characterAspect == Aspect.CharacterAspect.Machine)
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