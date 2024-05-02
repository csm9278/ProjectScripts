using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RootMain
{
    public class PrologueManager : MonoBehaviour
    {
        public enum PrologueState
        {
            FadeIn = 0,
            PracticeMove = 1,
            gunTuto = 3,
            gunTutoEnd = 4,
            BreakWallTuto = 5,
            ManyBattle = 10,
            GranadeTuto = 11,
            MiniMissileTuto = 12,
            GatlingGunTuto = 14,
            RaileGunTuto = 15,
            ShotGunTuto = 17,
            TurretTuto = 18,
            FlameTuto = 20,
            SeeBomb = 23,
            BombAttack = 24,
            AttackAfter = 25,

            Nothing
        }

        float practiceMoveTime = 10.0f;
        public PrologueState pState = PrologueState.FadeIn;
        Prologue_Dialog_Mgr dialogMgr;
        Player.PlayerAttack pCtrl;
        WaitForSeconds checkMons = new WaitForSeconds(0.1f);

        [Header("--- FadeInOut ---")]
        public Image fadeOutImage;
        public Image fadeInImage;
        public float fadeSpeed = 2.0f;
        Color fadeColor;
        float fadeAlpha;

        //gunTuto
        public Target[] gunTarget;
        public Door_Sensor_Ctrl gunDoor;
        public Door_Sensor_Ctrl gunDoor2;
        int checkCount = 0;
        bool startCo = false;

        //BreakWallTuto
        public GameObject breakWall;

        //battleManyEnemys
        [Header("--- ManyMonsterBattle ---")]
        public Door_Sensor_Ctrl openDoor0;  //많은 적과 싸우는 구간에서 열릴 문
        public Target[] manyMonsters;

        [Header("--- Granade Tutorial ---")]
        public Target[] granadeMonsters;
        public Door_Sensor_Ctrl granadeDoor;

        [Header("--- MiniMIssile Tutorial ---")]
        public Target[] MiniMissileMonsters;
        public Door_Sensor_Ctrl MiniMissileDoor;

        [Header("--- Gatling Tutorial ---")]
        public Target[] GatlingMonsters;
        public Door_Sensor_Ctrl GatlingDoor;

        [Header("--- RailGun Tutorial ---")]
        public Target raileGunMonster;
        public Door_Sensor_Ctrl railGunDoor;
        public Collider2D railGunNoEntryColl;
        int shotCount = 0;

        [Header("--- ShotGun Tutorial ---")]
        public Target[] shotGunMonsters;
        public Door_Sensor_Ctrl shotGunDoor;

        [Header("--- Turret Tutorial ---")]
        public Target[] turretMonsters;
        public Door_Sensor_Ctrl turretDoor;

        [Header("--- FlameThrower Tutorial ---")]
        public Target[] flameMonsters;
        public Door_Sensor_Ctrl flameDoor;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            dialogMgr = FindObjectOfType<Prologue_Dialog_Mgr>();
            pCtrl = FindObjectOfType<Player.PlayerAttack>();
            dialogMgr.pMgr = this;
            GameManager.instance.isNonePlayable = true;
            fadeColor = fadeOutImage.color;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            PstateUpdate();
        }

        #region Update부 함수
        void PstateUpdate()
        {
            switch (pState)
            {
                case PrologueState.FadeIn:
                    FadeOutFunc();
                    break;

                case PrologueState.gunTuto:
                    if(!startCo)
                    {
                        StartCoroutine(CheckMonster(gunTarget));
                        pCtrl.SetPrologWeapon(WeaponState.Sub, Weapon.SubWeaponType.Standard_Gun);
                        startCo = true; 
                    }
                    break;

                case PrologueState.gunTutoEnd:
                    if (!startCo)
                    {
                        gunDoor.area_Correct = true;
                        gunDoor2.area_Correct = true;
                    }
                    break;

                case PrologueState.BreakWallTuto:
                    if(!startCo)
                    {
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Standard_Launcher);
                        startCo = true;
                    }
                    CheckBreakWallFunc();
                    break;

                case PrologueState.ManyBattle:
                    if(!startCo)
                    {
                        startCo = true;
                        StartCoroutine(CheckMonster(manyMonsters, openDoor0));
                    }
                    break;

                case PrologueState.GranadeTuto:
                    if (!startCo)
                    {
                        StartCoroutine(CheckMonster(granadeMonsters, granadeDoor));
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Standard_Grenade);
                        startCo = true;
                    }
                    break;

                case PrologueState.MiniMissileTuto:
                    if (!startCo)
                    {
                        StartCoroutine(CheckMonster(MiniMissileMonsters, MiniMissileDoor, true));
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Mini_Missile);
                        startCo = true;
                    }
                    break;

                case PrologueState.GatlingGunTuto:
                    if (!startCo)
                    {
                        StartCoroutine(CheckMonster(GatlingMonsters, GatlingDoor));
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Gatling_Gun);
                        startCo = true;
                    }
                    break;

                case PrologueState.RaileGunTuto:
                    if(!startCo)
                    {
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Rail_Gun);
                        SwitchManager.SetSwitch("Prologue_RailGun", true);
                        startCo = true;
                    }
                    if(!dialogMgr.isPlayDialog)
                    CheckRailGun();
                    break;

                case PrologueState.ShotGunTuto:
                    if (!startCo)
                    {
                        StartCoroutine(CheckMonster(shotGunMonsters, shotGunDoor));
                        pCtrl.SetPrologWeapon(WeaponState.Sub, Weapon.SubWeaponType.Standard_Shotgun);
                        startCo = true;
                    }
                    break;

                case PrologueState.TurretTuto:
                    if (!startCo)
                    {
                        SwitchManager.SetSwitch("Prologue_Turret", true);
                        StartCoroutine(CheckMonster(turretMonsters, turretDoor, true));
                        pCtrl.isProlog = false;
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Standard_Launcher);
                        pCtrl.SetPrologWeapon(WeaponState.Sub, Weapon.SubWeaponType.Sentry_Turret);
                        
                        
                        for(int i = 0; i < turretMonsters.Length; i++)
                        {
                             turretMonsters[i].gameObject.SetActive(true);
                        }
                        startCo = true;
                    }
                    break;

                case PrologueState.FlameTuto:
                    if (!startCo)
                    {
                        pCtrl.isProlog = true;
                        StartCoroutine(CheckMonster(flameMonsters, flameDoor));
                        pCtrl.SetPrologWeapon(WeaponState.Main, Weapon.MainWeaponType.Flame_Thrower);

                        for(int i = 0; i < flameMonsters.Length; i++)
                        {
                            if(flameMonsters[i].TryGetComponent(out Mutant.EnemyBase_2 enemybase))
                            {
                                enemybase.SetForceAttack(true);
                            }
                        }
                        startCo = true;
                    }
                    break;

                case PrologueState.SeeBomb:
                    if(!startCo)
                    {
                        GameManager.instance.isNonePlayable = true;
                        startCo = true;
                    }
                    if(dialogMgr.isPlayDialog == false)
                    {
                        dialogMgr.d_dialog[(int)pState + 1].gameObject.SetActive(true);
                    }

                    break;

                case PrologueState.BombAttack:
                    if(!startCo)
                    {
                        GameManager.instance.isNonePlayable = true;
                        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            pCtrl.FireAttack();
                            startCo = true;
                        }
                    }
                    break;

                case PrologueState.AttackAfter:
                    if (!startCo)
                    {
                        GameManager.instance.isNonePlayable = true;
                        if (dialogMgr.isPlayDialog == false)
                        {
                            pCtrl.FireAttack();
                            fadeInImage.gameObject.SetActive(true);
                            fadeColor = fadeInImage.color;
                            startCo = true;
                        }
                    }
                    else
                    {
                        FadeInFunc();
                    }
 
                    break;


                default:
                    break;
            }
        }
        #endregion

        public void SetPState(int idx)
        {
            startCo = false;
            pState = (PrologueState)idx;
        }

        #region 페이드 인 아웃 함수들
        void FadeOutFunc()
        {
            if (fadeColor.a <= 0.0f)
                return;
            if (GameManager.instance.isNonePlayable == false)
                GameManager.instance.isNonePlayable = true;

            fadeColor = fadeOutImage.color;
            fadeAlpha = fadeColor.a;
            fadeAlpha -= Time.deltaTime * fadeSpeed;

            fadeColor.a = fadeAlpha;

            fadeOutImage.color = fadeColor;

            if(fadeColor.a <= 0)
            {
                fadeOutImage.gameObject.SetActive(false);
                dialogMgr.d_dialog[0].gameObject.SetActive(true);
            }
        }

        void FadeInFunc()
        {
            if (fadeColor.a >= 1.0f)
                return;
            if (GameManager.instance.isNonePlayable == false)
                GameManager.instance.isNonePlayable = true;

            fadeColor = fadeInImage.color;
            fadeAlpha = fadeColor.a;
            fadeAlpha += Time.deltaTime * fadeSpeed;

            fadeColor.a = fadeAlpha;

            fadeInImage.color = fadeColor;

            if (fadeColor.a >= 1)
            {

            }
        }
        #endregion

        IEnumerator CheckMonster(Target[] targets, Door_Sensor_Ctrl openDoor = null, bool TriggerActive = false)
        {
            while(true)
            {
                checkCount = targets.Length;
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i].isDie)
                        checkCount--;
                }

                if (checkCount <= 0)
                {
                    if (openDoor == null)
                        dialogMgr.d_dialog[(int)pState + 1].gameObject.SetActive(true);
                    else
                    {
                        openDoor.area_Correct = true;
                        if(TriggerActive)
                            dialogMgr.d_dialog[(int)pState + 1].gameObject.SetActive(true);
                    }
                    yield break;
                }

                yield return checkMons;
            }

        }


        void CheckRailGun()
        {
            if(Input.GetKeyUp(KeyCode.UpArrow)) //몇번 쐈는지 체크하기 위해
                shotCount++;

            if(raileGunMonster.isDie)
            {
                Debug.Log("shotcount : " + shotCount);
                if (shotCount > 1)
                    SwitchManager.SetSwitch("Prologue_RailGun", false);

                dialogMgr.d_dialog[(int)pState + 1].gameObject.SetActive(true);
                railGunDoor.area_Correct = true;
                railGunNoEntryColl.gameObject.SetActive(false);
            }
        }

        void CheckBreakWallFunc()
        {
            if(!breakWall.activeSelf)
            {
                dialogMgr.d_dialog[(int)pState + 1].gameObject.SetActive(true);
                startCo = false;
            }
        }

        void LookGunTarget()
        {
            //Vector2 dir = gunTarget[0].transform.position - pCtrl.gameObject.transform.position;
            ////float angle = GetAngle(dir);
            //Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            //transform.rotation = Quaternion.Lerp(this.transform.rotation, angleAxis, 0.3f);
        }

    }
}