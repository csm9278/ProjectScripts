using UnityEngine;
using Pathfinding;

namespace Cleaner
{
    public class CleanerBase : Target, IToxic
    {
        public GameObject player = null;
        public Transform[] MovePos;

        public AIPath _aiPath;

        //부식용 변수
        public ParticleSystem toxicEff;
        float toxicTime = -0.1f;
        float maxToxicTime = 10.0f;
        float toxicDamageTime = 1.0f;
        float curToxicDamageTime = 1.0f;

        [Header("--- Saw Trigger --- ")]
        public CleanerSawTrigger sawTrigger;


        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (player == null)
                player = GameObject.Find("Player");

            if (collider == null)
                collider = GetComponent<Collider2D>();
            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_aiPath == null)
                _aiPath = GetComponent<AIPath>();

            if (aspect == null)
                aspect = GetComponent<Aspect>();

            //플레이어 데미지어블 추가
            if(player.TryGetComponent(out IDamageable playerDam))
            {
                sawTrigger.playerDamage = playerDam;
            }
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            ToxicEff();
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