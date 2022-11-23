using UnityEngine;

namespace RootMain
{
    public class GameDirector : MonoBehaviour
    {

        bool isUp = false;
        public void WeaponDirector(int maxMain, int curMain, int maxSub, int curSub)
        {
            if (isUp)
            {
                if (maxMain * 0.5f < curMain)
                {
                    GlobalData.AmmoDropup(false);
                    isUp = false;
                    ADebug.Log("탄약 증가상태 종료");
                }

                if (curMain / maxMain < curSub / maxSub)    // 주무기의 탄창 비율보다 서브무기의 탄창 비율이 높을 경우
                {
                    GlobalData.MainAmmoDropUp(true);
                }
                else
                {
                    GlobalData.MainAmmoDropUp(false);
                }
            }
            else
            {
                if (maxMain * 0.3f > curMain)
                {
                    GlobalData.AmmoDropup(true);
                    ADebug.Log("탄약 증가 상태");
                    isUp = true;
                }
            }
        }
    }
}