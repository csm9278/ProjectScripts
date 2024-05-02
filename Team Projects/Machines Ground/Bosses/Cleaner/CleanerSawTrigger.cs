using UnityEngine;

namespace Cleaner
{
    public class CleanerSawTrigger : MonoBehaviour
    {
        int playerLayer;
        public bool isSawActive = false;
        public bool isInPlayer = false;
        public float delayTime = 0.15f;
        float curdelayTime = 0.15f;
        public IDamageable playerDamage;
        ParticleSystem sawParticle;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            playerLayer = LayerMask.NameToLayer("Player");
            sawParticle = GetComponentInChildren<ParticleSystem>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (isInPlayer && isSawActive)
            {
                curdelayTime -= Time.deltaTime;
                if(curdelayTime <= 0.0f)
                {
                    sawParticle.Play();
                    curdelayTime = delayTime;
                    playerDamage.OnDamage(GlobalData.gunData, "Standard Gun", this.transform.position, WeaponType.Null);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == playerLayer)
            {
                isInPlayer = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == playerLayer)
            {
                isInPlayer = false;
            }
        }
    }
}