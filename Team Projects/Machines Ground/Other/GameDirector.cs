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
                    ADebug.Log("ź�� �������� ����");
                }

                if (curMain / maxMain < curSub / maxSub)    // �ֹ����� źâ �������� ���깫���� źâ ������ ���� ���
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
                    ADebug.Log("ź�� ���� ����");
                    isUp = true;
                }
            }
        }
    }
}