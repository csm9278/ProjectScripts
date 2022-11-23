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

        //���ΰ� ��ħ
        [HideInInspector] public Vector2 lastSeePos;

        //�÷��̾� �Ÿ� ��� ����
        [HideInInspector] public GameObject player = null;
        Vector3 m_PlayerVec = Vector3.zero;
        float m_DistFromPlayer = 0.0f;

        //���� ����
        float StunTime = 0.0f;
        public bool m_AttackedFromPlayer = false;  //���ݴ����� ��
        float m_AttackChaseTime = 5.0f;

        //���� ��ü ����Ʈ (���� ����Ʈ ���� ����)
        public GameObject[] mutantList;                 //����Ʈ ���� �Լ�
        [SerializeField] GameObject mutantObj;          //��ȯ�� ����Ʈ ������Ʈ
        public int maxMutantCount;                      //��ȯ�� ����Ʈ�� �ִ� ����
        public float mutantsRange;                      //����Ʈ�� ������ �ٴ� ����
        [SerializeField] LayerMask obstacleMask;        //��ȯ�ÿ� ����ó���� ���̾�
        CheckMother checkMother;                        //üũ���� ����
        [SerializeField] float spawnDelay;              //���� Ÿ�̸�
        float spawnCurDealy;
        Queue<int> SpawnQueue;                          //���� ������ ���� ť

        float initDelaytimer = 0.2f;

        //�i�ƿ��� ���� ����
        public Transform[] babyFollowPoses;

        //��Ÿ������� ����
        public ParticleSystem burningEff;
        float burnTime = -0.1f;
        float maxburnTime = 10.0f;
        float burnDamageTime = 1.0f;
        float curBurnDamageTime = 1.0f;

        //�νĿ� ����
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
        /// ó���� ����Ʈ�� ��ȯ�ϰ� ����Ʈ�� �߰��� ���� �Լ� Start�ʿ��� �θ� ����
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
        /// ���������� ���� �Լ�
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
        /// ����Ʈ�� �ε����� �޾Ƽ� ���� �������ִ� �Լ�
        /// </summary>
        /// <param name="Dieidx">���� ����</param>
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
        /// �ܺο��� �ҷ�����
        /// </summary>
        /// <param name="Dieidx">���� ������ ��ȣ</param>
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
        /// �ȷο� ��ġ �̵����� ������ ���� ����
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
        /// ���� ���Ϳ��� �ҷ����� ���� ��ü�� �÷��̾ �߰� �� �ߵ�
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
        /// ���� ������ �� AI�ӽ� ��Ȱ��ȭ �� ���� �ð� ���
        /// </summary>
        /// <param name="a_stuntime">�� ���� �ð�</param>
        public void HittedEnemy(float a_stuntime = 0.0f)
        {
            if (a_stuntime > 0)
            {
                _aipath.enabled = false;
                StunTime = a_stuntime;
            }

            if (m_AttackedFromPlayer)   //�̹� ���ݻ����϶��� ����
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
        /// ���Ͻð�üũ �� �ٽ� AIȰ��ȭ�� �߰�
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
                        ADebug.Log("ȭ�� �߰� ������");
                        curBurnDamageTime = burnDamageTime;
                    }
                    else if (aspect.characterAspect == Aspect.CharacterAspect.Machine)
                    {
                        ADebug.Log("ȭ�� ������");
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