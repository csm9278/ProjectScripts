using System.Collections.Generic;
using UnityEngine;
using Altair;

namespace Altair
{
    public class TurretList
    {
        public string m_name;
        public int m_cost;
        public int m_buycost;
        public int m_upgradecost;
        public float m_prepTime; //업그레이드 가격, 타입에 따라서
        public int UpgradeLv = 0;           //그전엔 Lock, 레벨0 이면 아직 구매 안됨 (구매가 완료되면 레벨 1부터)
        public float m_activateTime;
        public int m_isSingleUse;
        public int m_isAtk;
        public int m_attackType;
        public float m_sensor;
        public float m_range;
        public int m_dam;
        public float m_atkDelay;
        public int m_isSingleTargeting;
        public int m_hp;
        public string m_iconRsc;
        public string m_Archive;




        public TurretList()
        {
        }

        public void SetType(int ii)
        {
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["name"], out m_name))return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["cost"], out m_cost)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["buyCost"], out m_buycost)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["upgradeCost"], out m_upgradecost)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["prepTime"], out m_prepTime)) return; 
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["activateTime"], out m_activateTime)) return; 
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["isSingleUse"], out m_isSingleUse)) return; 
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["isAtk"], out m_isAtk)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["atkType"], out m_attackType)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["sensor"], out m_sensor)) return; 
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["range"], out m_range)) return; 
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["dam"], out m_dam)) return; 
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["atkDelay"], out m_atkDelay)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["isSingleTargeting"], out m_isSingleTargeting)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["hp"], out m_hp)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["iconRsc"], out m_iconRsc)) return;
            if (!JSONParser.DataValidation(Altair.GlobalData.turretData[GlobalData.choi_TurretNameList[ii]]["Archive"], out m_Archive)) return;
        }
    }

    internal static partial class GlobalData
    {
        

    }
}