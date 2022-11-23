using UnityEngine;
using Pathfinding;

namespace HeavyArms
{
    public class HeavyArmsBase : Target, IToxic
    {
        public GameObject player = null;
        public bool isRand = false; //랜덤 좌표로 생성했는지?
        public Transform[] MovePos;
        public Vector3[] randVec;
        public LayerMask ignorelayer;

        public AIPath _aiPath;

        //부식용 변수
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        protected override void Awake()
        {
            if(collider == null)
                collider = GetComponent<Collider2D>();
            if(_animator == null)
                _animator = GetComponent<Animator>();

            if (_aiPath == null)
                _aiPath = GetComponent<AIPath>();

            if (aspect == null)
                aspect = GetComponent<Aspect>();

            if (player == null)
                player = GameObject.Find("Player");

            enemyName = "HeavyArms";
            
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            ignorelayer = LayerMask.NameToLayer("Obstacle");

            if (MovePos.Length == 0)
            {
                SetRandPos();
                isRand = true;
            }
            else
            {
                if (MovePos[0] == null)
                {
                    SetRandPos();
                    isRand = true;
                }
                else
                    Debug.Log("요소가 있네");
            }
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            ToxicEff();
        }

        public override void AddDeathEffect()
        {
            this.gameObject.SetActive(false);
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

        public void SetRandPos()
        {
            randVec = new Vector3[4];
            for(int i = 0; i < 4; i++)
            {
                int rands = 20;

                for(int j = 0; j < 100; j++)
                {
                    Vector3 randPos = new Vector3(Random.Range(this.transform.position.x - rands, this.transform.position.x + rands),
                              Random.Range(this.transform.position.y - rands, this.transform.position.y + rands),
                              this.transform.position.z);

                    Collider2D coll = Physics2D.OverlapCircle(randPos, 1.5f, ignorelayer);

                    if (coll != null)
                        continue;

                    //MovePos[i] = new GameObject().GetComponent<Transform>();
                    randVec[i] = randPos;
                }

            }
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