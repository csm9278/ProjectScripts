using SimpleJSON;
using UnityEngine;

namespace Turret
{
    public class ShieldCtrl : MonoBehaviour, IDamageable
    {
        public int hp;
        [SerializeField] Collider2D obcoll;
        RootMain.ExplosionFunc exp;
        float brokenTimer = -0.1f;

        public bool isShiledBroken = false;
        public GameObject shieldCorpseObj;

        private void Awake()
        {
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (obcoll == null)
                obcoll = GetComponent<Collider2D>();

            if (exp == null)
                exp = GetComponent<RootMain.ExplosionFunc>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if(brokenTimer >= 0.0f)
            {
                brokenTimer -= Time.deltaTime;
                if(brokenTimer <= 0.0f)
                {
                    isShiledBroken = true;
                    this.gameObject.SetActive(false);
                }
            }
        }

        public void OnDamage(JSONNode node, string key, Vector3? pos, WeaponType type = WeaponType.Null, bool isSplash = false, float gauge = 0.0f)
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
                }
                int damage = Random.Range(minDam, dam + 1);
                hp -= damage;
            }

            //ShiledImgChanger();


            if (hp <= 0)
            {
                brokenTimer = 0.1f;
                Vector3 expos = (Vector3)pos;
                exp.expPos = expos;
                exp.enabled = true;

                if (shieldCorpseObj != null)
                    shieldCorpseObj.transform.rotation = this.gameObject.transform.rotation;
            }
        }

        void ShiledImgChanger()
        {

        }
    }
}