using UnityEngine;
using Pathfinding;
using SimpleJSON;
using Item;

namespace Drone
{
    //�������� ����..
    //�ʹ� ���� ���� Ÿ���� ã�⺸�ٴ� �������� �ڱ� ��ġ ����� �ʴ� �ѿ��� Ž���ϴ� ��
    //���� ����?

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


        //���ΰ� ��ħ
        [HideInInspector] public Vector2 lastSeePos;

        public bool isNotPatrol = false;


        /// <summary>
        /// ó���� ������ ��ǥ�� ������ �����
        /// </summary>
        bool setPosesbool = false;

        //private int hp = 150;

        //�÷��̾� �Ÿ� ��� ����
        [HideInInspector] public GameObject playerRef = null;
        Vector3 playerVec = Vector3.zero;
        float distFromPlayer = 0.0f;


        //���� ����
        float stunTime = 0.0f;
        [HideInInspector] public bool attackFromPlayer = false;  //���ݴ����� ��

        //AnimatorHash
        [HideInInspector] public int attackfromPlayerHash = Animator.StringToHash("AttackFromPlayer");
        [HideInInspector] public int hittedHash = Animator.StringToHash("Hitted");
        [HideInInspector] public int distanceFromPlayerHash = Animator.StringToHash("DistanceFromPlayer");
        [HideInInspector] public int findPlayerHash = Animator.StringToHash("FindPlayer");

        //�νĿ� ����
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        //����� ����
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
        /// ���Ͻð�üũ �� �ٽ� AIȰ��ȭ�� �߰�
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
        /// �÷��̾ �߰������� �����ϴ� �Լ��Դϴ�. StateMachine���� �ҷ����ϴ�.
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

            //�������� �� �߰�
            RootMain.GameManager.instance.AddBattleNum();
        }


        /// <summary>
        /// ���� ������ �� AI�ӽ� ��Ȱ��ȭ �� ���� �ð� ��� �����ʿ��� ȣ��
        /// </summary>
        /// <param name="a_stuntime">�� ���� �ð�</param>
        public void HittedEnemy(float a_stuntime = 0.0f)
        {
            if (hp <= 0.0f)
                return;

            if (a_stuntime > 0)
            {
                _aipath.enabled = false;
                stunTime = a_stuntime;
            }

            if (_animator.GetBool(attackfromPlayerHash))   //�̹� ���ݻ����϶��� ����
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
                if(attackFromPlayer)// ������ ����� �������� ���Ҹ� ����
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