using UnityEngine;
using Pathfinding;
using SimpleJSON;
using Altair_Memory_Pool_Pro;

namespace Mutant
{
    public class BabyMutantBase : Target ,IHitted
    {
        [HideInInspector] public bool canAttack = false;
        MotherMutantBase motherbase;
        public AIPath _aipath;
        public AIDestinationSetter aidestSetter;
        [HideInInspector] public GameObject player;

        CheckMother checkMother;
        [HideInInspector] public Vector2 motherVec;
        float distanceMother;
        [HideInInspector] public Vector2 playerVec;
        float distancePlayer;

        public float findDistance = 0.0f;
        public float attackDistance = 0.0f;

        float StunTime = 0.0f;

        PoolingSc poolingSc;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (checkMother == null)
            {
                this.gameObject.TryGetComponent(out checkMother);
                motherbase = checkMother.motherBase;
            }

            if (aidestSetter == null)
                this.gameObject.TryGetComponent(out aidestSetter);

            if (_aipath == null)
                this.gameObject.TryGetComponent(out _aipath);

            if (poolingSc == null)
                this.gameObject.TryGetComponent(out poolingSc);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            //거리 측정부
            CheckDistanceFromMother();
            CheckDistanceFromPlayer();
            StunFunc();

            TargetContact();
        }


        /// <summary>
        /// 엄마와의 거리 체크 함수
        /// </summary>
        public void CheckDistanceFromMother()
        {
            motherVec = motherbase.transform.position - this.transform.position;
            distanceMother = motherVec.magnitude;
        }


        /// <summary>
        /// 플레이어와의 거리 체크 함수
        /// </summary>
        public void CheckDistanceFromPlayer()
        {
            playerVec = player.transform.position - this.transform.position;
            distancePlayer = playerVec.magnitude;

            FindPlayer();
        }

        public void FindPlayer()
        {
            if(distanceMother >= 10.0f)
            {
                motherbase.ReturnSignal();
                return;
            }


            if(distancePlayer <= findDistance)
            {
                motherbase.FindSignal();
            }

            if(distancePlayer <= attackDistance)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }
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

            //사망 처리
            if (hp <= 0)
            {
                checkMother.SignalToMother();
                hp = 60;
                poolingSc.objectReturn();
            }
        }

        public void HittedEnemy(float a_stuntime = 0)
        {
            if (a_stuntime > 0)
            {
                _aipath.enabled = false;
                StunTime = a_stuntime;
            }

        }

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
            if (coll.gameObject.TryGetComponent(out Aspect aspect))
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