using UnityEngine;
using System.Collections;

namespace RootMain
{
    public class EnemyAppear : MonoBehaviour
    {
        public enum Difficult
        {
            Easy,
            Normal,
            Hard,
            Very_Hard,
            VeryVery_Hard,
            VVVery_Hard,
        }

        public enum Area
        {
            Area1 = 0,
            Area2,
            Area3,
            Area4,
            Area5,
        }

        public float CheckSecond;
        bool isAppear = false;

        public Difficult difficult = Difficult.Easy;
        public Area area = Area.Area1;

        WaitForSeconds checkTime;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (CheckSecond <= 0)
                CheckSecond = 3;

            if (GlobalData.Difficult < (int)difficult)
                this.gameObject.SetActive(false);

            checkTime = new WaitForSeconds(CheckSecond);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        IEnumerator CheckAppear()
        {
            while(true)
            {
                if (GlobalData.areaNum <= (int)this.area)
                {
                    this.gameObject.SetActive(true);
                    yield break;
                }
                else
                    this.gameObject.SetActive(false);

                yield return checkTime;

            }


        }
    }
}