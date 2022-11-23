using UnityEngine;
using Player;
using Altair_Memory_Pool_Pro;

namespace Item
{
    public class ItemNode : MonoBehaviour
    {
        public LayerMask ignoreLayer;

        public enum ItemType
        {
            AmmoBox,
            Repair,
            Parts,
            Scrap,
            MaxCount
        }

        public enum AmmoType
        {
            MainAmmo,
            SubAmmo,
            MaxCount
        }

        public enum RepairType
        {
            Armor,
            Health,
            MaxCount
        }

        public enum SizeType
        {
            Large,
            Medium,
            Small,
            MaxCount
        }

        public ItemType itemType = ItemType.MaxCount;
        public AmmoType armoType = AmmoType.MaxCount;
        public RepairType repairType = RepairType.MaxCount;
        public SizeType sizeType = SizeType.MaxCount;
        //[HideInInspector] public PlayerController pCtrl;
        bool isDrop = false;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            ignoreLayer = (1 << 19) + (1 << LayerMask.NameToLayer("Obstacle")); 
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }


        public void DieTrigger(Vector3 dieVec)
        {
            if (isDrop == true)
                return;

            isDrop = true;

            //int percent = Random.Range(1, 101);
            //if (percent <= 50)
            ItemDrop(dieVec);

        }


        /// <summary>
        /// 아이템을 드랍 및 설정하는 함수
        /// </summary>
        /// <param name="dieVec"></param>
        void ItemDrop(Vector3 dieVec)
        {
            float maxX = 1.5f;
            dieVec.z = -8.0f;

            for(int i = 0; i < 3; i++)
            {
                int dropPercent = Random.Range(0, 100);

                if(dropPercent < GlobalData.itemDropsPercent[i])
                {
                    for (int j = 0; j < 30; j++)
                    {
                        Vector3 randPos = new Vector3(Random.Range(dieVec.x - maxX, dieVec.x + maxX),
                                                      Random.Range(dieVec.y - maxX, dieVec.y + maxX),
                                                      dieVec.z);

                        Collider2D coll =  Physics2D.OverlapCircle(randPos, .3f, ignoreLayer);

                        if (coll != null)
                        {
                            ADebug.Log(coll.gameObject.name);
                            continue;
                        }
                        else
                        {
                            GameObject item = MemoryPoolManager.instance.GetObject("Item", randPos);
                            itemType = (ItemType)i;

                            switch (this.itemType)
                            {
                                case ItemType.AmmoBox:
                                    armoType = SetArmoBox();
                                    sizeType = SetSize();
                                    break;

                                case ItemType.Repair:
                                    repairType = SetScrap();
                                    sizeType = SetSize();
                                    break;

                                case ItemType.Parts:
                                    sizeType = SetSize(true);
                                    break;
                            }

                            ////이미지 교체를 위해 넘겨준다.
                            if (item.TryGetComponent(out ArmoBox box))
                            {
                                box.itemType = this.itemType;
                                box.armoType = this.armoType;
                                box.repairType = this.repairType;
                                box.sizeType = this.sizeType;
                                box.ImageChanger();
                            }

                            break;
                        }
                    }
                }



            }

            #region 기존버전
            //GameObject item = Instantiate(GlobalData.ItemObj);
            //dieVec.z -= 4.0f;
            //GameObject item = MemoryPoolManager.instance.GetObject("Item", dieVec);

            //int ammoPercent = Random.Range(0, 100);

            //int repairPercent = GlobalData.ammoDropPercent + GlobalData.scrapDropPercent;
            //int partsPercent = repairPercent + GlobalData.partsDropPercent;

            //if (ammoPercent < GlobalData.ammoDropPercent)
            //    itemType = ItemType.AmmoBox;
            //else if (GlobalData.ammoDropPercent <= ammoPercent && ammoPercent < repairPercent)
            //    itemType = ItemType.Repair;
            //else if (repairPercent <= ammoPercent && ammoPercent < partsPercent)
            //    itemType = ItemType.Parts;
            //else
            //    itemType = ItemType.Scrap;

            //switch(this.itemType)
            //{
            //    case ItemType.AmmoBox:
            //        armoType = SetArmoBox();
            //        sizeType = SetSize();
            //        break;

            //    case ItemType.Repair:
            //        repairType = SetScrap();
            //        sizeType = SetSize();
            //        break;

            //    case ItemType.Parts:
            //        sizeType = SetSize(true);
            //        break;
            //}

            ////이미지 교체를 위해 넘겨준다.
            //if(item.TryGetComponent(out ArmoBox box))
            //{
            //    box.itemType = this.itemType;
            //    box.armoType = this.armoType;
            //    box.repairType = this.repairType;
            //    box.sizeType = this.sizeType;
            //    box.ImageChanger();
            //}
            #endregion
        }

        AmmoType SetArmoBox()
        {
            int rand = Random.Range(0, 100);

            if (rand < GlobalData.subAmmoDropPercent)
                return AmmoType.SubAmmo;
            else
                return AmmoType.MainAmmo;
        }

        RepairType SetScrap()
        {
            int rand = Random.Range(0, 100);

            if (rand < GlobalData.armorScrapDropPercent)
                return RepairType.Armor;
            else
                return RepairType.Health;
        }

        /// <summary>
        /// 사이즈를 결정하는 함수 Part는 true를 매개변수로 넣어줘야함
        /// </summary>
        /// <param name="isParts">ItemType이 Parts인지 아닌지</param>
        /// <returns></returns>
        SizeType SetSize(bool isParts = false)
        {
            if(!isParts)
            {
                int rand = Random.Range(0, 100);

                if (rand < GlobalData.smallDropPercent)
                    return SizeType.Small;
                else if (GlobalData.smallDropPercent <= rand && rand < GlobalData.smallDropPercent + GlobalData.mediumDropPercent)
                    return SizeType.Medium;
                else
                    return SizeType.Large;
            }
            else
            {
                float rand = Random.Range(0, 99);

                if (rand < GlobalData.smallPartDropPercent)
                    return SizeType.Small;
                else if (GlobalData.smallPartDropPercent <= rand && rand < GlobalData.smallPartDropPercent + GlobalData.mediumPartDropPercent)
                    return SizeType.Medium;
                else
                    return SizeType.Large;
            }
        }



        public void GetArmo(AmmoType type, SizeType rType, PlayerController pCtrl)
        {
            int ammoVal = 0;


            switch (rType)
            {
                case SizeType.Large:
                    ammoVal = 3;
                    break;

                case SizeType.Medium:
                    ammoVal = 2;
                    break;

                case SizeType.Small:
                    ammoVal = 1;
                    break;
            }

            switch (type)
            {
                case AmmoType.MainAmmo:
                    pCtrl.AddMainAmmo(ammoVal);
                    break;

                case AmmoType.SubAmmo:
                    pCtrl.AddSubAmmo(ammoVal);
                    break;
            }
        }

        public void GetRepair(RepairType type, SizeType rType, PlayerController pCtrl)
        {
            int repairVal = 0;

            switch(rType)
            {
                case SizeType.Large:
                    repairVal = 100;
                    break;

                case SizeType.Medium:
                    repairVal = 50;
                    break;

                case SizeType.Small:
                    repairVal = 20;
                    break;
            }

            switch(type)
            {
                case RepairType.Armor:
                    pCtrl.AddAmmor(repairVal);
                    break;

                case RepairType.Health:
                    pCtrl.AddHealth(repairVal);
                    break;
            }
        }

        public void GetParts(PlayerController pCtrl, SizeType rType)
        {
            switch (rType)
            {
                case SizeType.Large:
                    break;

                case SizeType.Medium:
                    break;

                case SizeType.Small:
                    break;
            }
        }

        public void GetBattery(PlayerController pCtrl)
        {

        }
    }
}