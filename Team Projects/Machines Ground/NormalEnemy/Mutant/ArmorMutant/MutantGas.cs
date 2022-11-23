using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Mutant
{
    public class MutantGas : MemoryPoolingFlag
    {
        GameObject player;
        ParticleSystem _particle;
        float damageTick = 0.1f;
        float bringTimer = 5.0f;
        Vector2 playerVec;

        bool isStop = false;
        public bool always = false;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            player = GameObject.Find("Player");
            _particle = GetComponent<ParticleSystem>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if(always == false)
                BringObjectFunc();

            playerVec = player.transform.position - this.transform.position;

            if(playerVec.magnitude <= 2.0f)
            {
                TickDamageFunc();
            }
        }

        void BringObjectFunc()
        {
            bringTimer -= Time.deltaTime;

            if(bringTimer <= 1.5f && isStop == false)   // 이펙트 사라지는 효과
            {
                _particle.Stop();
                isStop = true;
            }

            if(bringTimer <= 0.0f)
            {
                bringTimer = 5.0f;
                isStop = false;
                ObjectReturn();
            }
        }

        void TickDamageFunc()
        {
            damageTick -= Time.deltaTime;
            if(damageTick <= 0.0f)
            {
                damageTick = 0.1f;
                if(player.TryGetComponent(out IDamageable damage))
                    damage.OnDamage(GlobalData.flameData, "Toxin Sprayer", this.transform.position, WeaponType.Bio);
            }
        }
    }
}