using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Mutant
{
    public class DropParticle : MonoBehaviour
    {
        public GameObject particleObj;
        ParticleSystem particle;

        public float maxDropTimer = -0.1f;
        float curDropTimer = -0.1f;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            curDropTimer = maxDropTimer;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            Dropparticle();
        }

        void Dropparticle()
        {
            if(curDropTimer >= 0.0f)
            {
                curDropTimer -= Time.deltaTime;
                if(curDropTimer <= 0.0f)
                {
                    //GameObject p = Instantiate(particleObj);
                    //p.transform.position = this.transform.position;
                    //particle = p.GetComponent<ParticleSystem>();
                    //particle.Play();

                    particleObj = MemoryPoolManager.instance.GetObject("MutantGas", this.transform.position);

                    curDropTimer = maxDropTimer;
                }
            }
        }
    }
}