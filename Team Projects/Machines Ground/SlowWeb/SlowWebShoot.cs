using UnityEngine;

namespace Mutant
{
    public class SlowWebShoot : MonoBehaviour
    {
        public GameObject target;
        public GameObject shootObj;

        public float shotDelay;
        float curShotDelay;

        Vector3 shootVec = Vector3.zero;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            curShotDelay = shotDelay;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            LookPlayerFunc();
            ShootSlowWeb();
        }

        void ShootSlowWeb()
        {
            if(curShotDelay >= 0.0f)
            {
                curShotDelay -= Time.deltaTime;
                if(curShotDelay <= 0.0f)
                {
                    GameObject obj = Instantiate(shootObj);
                    obj.transform.position = this.transform.position;

                    if(obj.TryGetComponent(out SlowWebBullet bullet))
                    {
                        bullet.SetTarget(target.transform.position, target);
                    }
                    curShotDelay = shotDelay;
                }
            }
        }

        public void LookPlayerFunc()
        {
            shootVec = target.transform.position - this.transform.position;

            float angle = Mathf.Atan2(shootVec.y, shootVec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
            transform.rotation = rotation;
        }
    }
}