using UnityEngine;
using Pathfinding;
using SimpleJSON;
using Item;

namespace Drone
{
    //넓은범위 순찰..
    //너무 많은 랜덤 타겟을 찾기보다는 넓은지역 자기 위치 벗어나지 않는 한에서 탐색하는 형
    //몬스터 내분?

    public class DroneBase : Target, IHitted, IDamageable, IToxic
    {

        public enum DroneType
        {
            Normal,
            Minimum
        }

        public DroneType droneType = DroneType.Normal;

        public Transform[] poses;
        [HideInInspector] public int posIdx = 1;
        [HideInInspector] public AIPath _aipath = null;
        [HideInInspector] public ChoiDroneAnimCtrl droneAnimCtrl;


        //주인공 놓침
        [HideInInspector] public Vector2 lastSeePos;

        public bool isNotPatrol = false;


        /// <summary>
        /// 처음에 순찰할 좌표를 지정해 줬는지
        /// </summary>
        bool setPosesbool = false;

        //private int hp = 150;

        //플레이어 거리 계산 변수
        [HideInInspector] public GameObject playerRef = null;
        Vector3 playerVec = Vector3.zero;
        float distFromPlayer = 0.0f;


        //공격 변수
        float stunTime = 0.0f;
        [HideInInspector] public bool attackFromPlayer = false;  //공격당했을 시

        //AnimatorHash
        [HideInInspector] public int attackfromPlayerHash = Animator.StringToHash("AttackFromPlayer");
        [HideInInspector] public int hittedHash = Animator.StringToHash("Hitted");
        [HideInInspector] public int distanceFromPlayerHash = Animator.StringToHash("DistanceFromPlayer");
        [HideInInspector] public int findPlayerHash = Animator.StringToHash("FindPlayer");

        //부식용 변수
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        //사망용 오일
        public GameObject deathOilObj;

        protected override void Awake()
        {
            base.Awake();

            if (poses == null)
                poses = GameObject.Find("EnemyPoses").GetComponentsInChildren<Transform>();
            _aipath = GetComponent<AIPath>();
            //_animator = GetComponent<Animator>();
            playerRef = GameObject.Find("Player");
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            droneAnimCtrl = GetComponentInChildren<ChoiDroneAnimCtrl>();
            itemnode = GetComponent<ItemNode>();
            enemyName = "drone";
            InitStatus(_aipath);

            if (this.droneType == DroneType.Minimum)
                _animator.SetBool("IsMinimum", true);

            if (forceAttackPlayer)
                _animator.SetBool("ForceAttack", forceAttackPlayer);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (SwitchManager.GetSwitch("BackOriginMonster")) backOriginPos = true;

            checkBackOrigin();

            CheckDistFromPlayer();
            StunFunc();

            ToxicEff();
            TargetContact();
        }

        public void CheckDistFromPlayer()
        {
            if (playerRef == null)
                return;

            playerVec = playerRef.transform.position - this.transform.position;
            playerVec.z = 0;
            distFromPlayer = playerVec.magnitude;
            _animator.SetFloat("DistanceFromPlayer", distFromPlayer);

            if (distFromPlayer >= 8.0f && droneType == DroneType.Normal)
                _animator.SetBool("FindPlayer", false);
        }

        /// <summary>
        /// 스턴시간체크 후 다시 AI활성화해 추격
        /// </summary>
        public void StunFunc()
        {
            if (stunTime >= 0.0f)
            {
                stunTime -= Time.deltaTime;
                if (stunTime <= 0.0f)
                {
                    _aipath.enabled = true;
                }
            }
        }

        /// <summary>
        /// 플레이어를 발견했을때 동작하는 함수입니다. StateMachine에서 불러집니다.
        /// </summary>
        public void FindEnemy()
        {
            if (attackFromPlayer)
                return;

            if (isNotPatrol)
                isNotPatrol = false;

            attackFromPlayer = true;
            _animator.SetBool(attackfromPlayerHash, attackFromPlayer);

            _aipath.destination = playerRef.transform.position;
            //_aipath.endReachedDistance = 6.0f;

            //전투상태 적 추가
            RootMain.GameManager.instance.AddBattleNum();
        }


        /// <summary>
        /// 공격 당했을 시 AI임시 비활성화 및 스턴 시간 계산 무기쪽에서 호출
        /// </summary>
        /// <param name="a_stuntime">적 경직 시간</param>
        public void HittedEnemy(float a_stuntime = 0.0f)
        {
            if (hp <= 0.0f)
                return;

            if (a_stuntime > 0)
            {
                _aipath.enabled = false;
                stunTime = a_stuntime;
            }

            if (_animator.GetBool(attackfromPlayerHash))   //이미 공격상태일때는 리턴
                return;
            else
            {
                _animator.SetTrigger(hittedHash);
            }
        }

        public override void OnDamage(JSONNode node, string key, Vector3? pos, WeaponType type = WeaponType.Null, bool isSplash = false, float gauge = 0.0f)
        {
            int tr = Calculator.BindToInt(GlobalData.characterData["Drone"]["expRes"],
                                          GlobalData.characterData["Drone"]["burnRes"],
                                          GlobalData.characterData["Drone"]["pierRes"]);

            if (isSplash)
            {
                if (!GlobalData.DataValidation(node[key][GlobalData.minExplosionDamage], out int expMinDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.explosionDamage], out int expDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.explosionDamageMaxRestore], out int expRst)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.critDam], out int critDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.pierDam], out int pierDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.mainType], out int weaponType)) return;

                if (expRst > 0)
                {
                    float interval = .0f;
                    interval = (expDam - expMinDam) / expRst;

                    //int level = GlobalData.LauncherData.stdRocketRst.b;
                    //interval *= level;
                    //expMinDam += (int)interval;
                }

                int expDamage = Random.Range(expMinDam, expDam + 1);
                hp -= Calculator.TargetDamageCal(expDamage, GlobalData.characterData["Drone"]["dt"], GlobalData.characterData["Drone"]["dr"], out isCri, out isPier,
                    Player.PlayerController.critPer, critDam, Player.PlayerController.pierPer, pierDam, weaponType, tr);
            }
            else
            {
                if (!GlobalData.DataValidation(node[key][GlobalData.minDamage], out int minDam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.damage], out int dam)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.damageMaxRestore], out int damRst)) return;
                if (!GlobalData.DataValidation(node[key][GlobalData.critDam], out int critDam)) return;

                if (!GlobalData.DataValidation(node[key][GlobalData.pierDam], out int pierDam))
                {
                    Debug.Log("pierDam");
                    return;
                }

                if (!GlobalData.DataValidation(node[key][GlobalData.mainType], out int weaponType))
                {
                    Debug.Log("weaponType");
                    return;
                }



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
                hp -= Calculator.TargetDamageCal(damage, GlobalData.characterData["Drone"]["dt"], GlobalData.characterData["Drone"]["dr"], out isCri, out isPier,
                    Player.PlayerController.critPer, critDam, Player.PlayerController.pierPer, pierDam, weaponType, tr);
            }

            if (hp <= 0.0f)
            {
                isDie = true;
                if(attackFromPlayer)// 전투중 사망시 전투상태 감소를 위해
                {
                    RootMain.GameManager.instance.DecreaseBattleNum();
                }

                collider.enabled = false;
                _aipath.destination = this.transform.position;

                ItemDrop(this.transform.position);
                //this.gameObject.SetActive(false);
                //droneAnimCtrl.ChangeAnim(ChoiDroneAnimCtrl.DroneAnimType.Death);
                droneAnimCtrl._animator.enabled = false;
                deathOilObj.SetActive(true);
                if (_animator != null)
                    _animator.enabled = false;

                if(TryGetComponent(out RootMain.ExplosionFunc exp))
                {
                    Vector3 hitPos = (Vector3)pos;
                    exp.expPos = hitPos;
                    exp.enabled = true;
                }
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(!isContactTarget)
            if(collision.gameObject.TryGetComponent(out Aspect aspect))
            {
                if (aspect.characterAspect != Aspect.CharacterAspect.Player)
                {
                    isContactTarget = true;
                    contactTimer = 2.0f;
                }
            }
        }
    }
}