using UnityEngine;

namespace SungJae
{
    public class SniperCheck : MonoBehaviour
    {
        public Sniper sniper;
        float CheckTime = 1.0f;

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(this.transform.position, new Vector2(3.5f, 3.5f));
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (sniper == null)
                sniper = GetComponentInParent<Sniper>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

            //if (sniper.isInEnemy)
            //{
            if (CheckTime >= 0.0f)
            {
                CheckTime -= Time.deltaTime;
                if (CheckTime <= 0.0f)
                {
                    Collider2D coll = Physics2D.OverlapBox(this.transform.position, new Vector2(3.5f, 3.5f), 0, sniper.enemylayer);

                    if (coll != null)//.Length > 0)
                    {
                        Debug.Log("있음");
                        sniper.isInEnemy = true;
                        sniper.sniperImage.SetActive(false);
                        CheckTime = 1.0f;
                        Debug.Log("find");
                    }
                    else
                    {
                        Debug.Log("없음");
                        sniper.isInEnemy = false;
                        sniper.sniperImage.SetActive(true);
                        CheckTime = 1.0f;
                    }
                }
            }
            //}
        }

    }       
}           