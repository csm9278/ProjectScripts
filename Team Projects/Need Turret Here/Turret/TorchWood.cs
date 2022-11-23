using UnityEngine;
using Altair_Memory_Pool_Pro;
using Choi;

namespace SungJae
{
    public class TorchWood : Turret_Ctrl
    {
        private void Start() => StartFunc();

        private void StartFunc()
        {
         
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        public void UpgradeBullet(BulletCtrl ctrl)
        {
            if (ctrl.isUpgraded)
                return;

            if (ctrl.isSlow)
            {
                ctrl.isSlow = false;
                ctrl.GetComponentInChildren<ParticleSystem>().Stop();
                ctrl.isUpgraded = true;
                return;
            }

            Debug.Log("강화 완료 " + ctrl.Damage + " - >" + ctrl.Damage * 2);
            ctrl.GetComponentInChildren<ParticleSystem>().Play();
            ctrl.isUpgraded = true;
            ctrl.Damage *= 2;
            ctrl.splashType = BulletCtrl.SplashType.Splash;
            ctrl.SplashVec = new Vector2(1, 1);
            

        }
    }
}