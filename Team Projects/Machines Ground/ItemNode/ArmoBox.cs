using UnityEngine;
using Player;

namespace Item
{
    public class ArmoBox : ItemNode
    {
        public SpriteRenderer _spRenderer;
        PlayerController playerctrl;
        Mutant.PoolingSc poolingSC;

        public bool isSetting = false;

        GameObject player;
        Vector3 playerVec;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            ImageChanger();
            player = GameObject.Find("Player");


            poolingSC = GetComponent<Mutant.PoolingSc>();


        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            //playerVec = player.transform.position - this.transform.position;

            //float angle = Mathf.Atan2(playerVec.y, playerVec.x) * Mathf.Rad2Deg;
            //Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            //transform.rotation = angleAxis;

            this.transform.eulerAngles = player.transform.eulerAngles;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.TryGetComponent(out Aspect aspect))
            {
                if (aspect.characterAspect == Aspect.CharacterAspect.Player)
                {
                    playerctrl = collision.GetComponent<PlayerController>();
                    ItemEff(itemType, playerctrl);

                    if(isSetting)
                        this.gameObject.SetActive(false);   //미리 세팅해놓은 아이템이면 꺼줌
                    else
                        poolingSC.objectReturn();   //몬스터에게 드랍된 애들은 오브젝트 리턴

                }
            }
        }

        public void ItemEff(ItemType itemType, PlayerController pCtrl)
        {
            switch (itemType)
            {
                case ItemType.AmmoBox:
                    GetArmo(armoType, sizeType, pCtrl);
                    break;

                case ItemType.Repair:
                    GetRepair(repairType, sizeType, pCtrl);
                    break;

                case ItemType.Parts:
                    GetParts(pCtrl, sizeType);
                    break;

                case ItemType.Scrap:
                    GetBattery(pCtrl);
                    break;
            }
        }


        public void ImageChanger()
        {
            switch(itemType)
            {
                case ItemType.AmmoBox:
                    ArmoImageChange();
                    break;
                case ItemType.Repair:
                    RepairImageChange();
                    break;
                case ItemType.Parts:
                    _spRenderer.sprite = GlobalData.partsWeaponSprites[0];
                    break;
                case ItemType.Scrap:
                    ScrapImageChange();
                    break;
            }
        }

        void ArmoImageChange()
        {
            switch (armoType)
            {
                case AmmoType.MainAmmo:
                    switch (sizeType)
                    {

                        case SizeType.Large:
                            _spRenderer.sprite = GlobalData.mainWeaponSprites[(int)ItemNode.SizeType.Large];
                            break;
                        case SizeType.Medium:
                            _spRenderer.sprite = GlobalData.mainWeaponSprites[(int)ItemNode.SizeType.Medium];
                            break;
                        case SizeType.Small:
                            _spRenderer.sprite = GlobalData.mainWeaponSprites[(int)ItemNode.SizeType.Small];
                            break;
                    }
                    break;

                case AmmoType.SubAmmo:
                    switch (sizeType)
                    {
                        case SizeType.Large:
                            _spRenderer.sprite = GlobalData.subWeaponSprites[(int)ItemNode.SizeType.Large];
                            break;
                        case SizeType.Medium:
                            _spRenderer.sprite = GlobalData.subWeaponSprites[(int)ItemNode.SizeType.Medium];
                            break;
                        case SizeType.Small:
                            _spRenderer.sprite = GlobalData.subWeaponSprites[(int)ItemNode.SizeType.Small];
                            break;
                    }
                    break;
            }
        }

        void RepairImageChange()
        {
            switch(repairType)
            {
                case RepairType.Health:
                    switch(sizeType)
                    {
                        case SizeType.Large:
                            _spRenderer.sprite = GlobalData.repairSprites[(int)ItemNode.SizeType.Large];
                            break;
                        case SizeType.Medium:
                            _spRenderer.sprite = GlobalData.repairSprites[(int)ItemNode.SizeType.Medium];
                            break;
                        case SizeType.Small:
                            _spRenderer.sprite = GlobalData.repairSprites[(int)ItemNode.SizeType.Small];
                            break;
                    }
                    break;

                case RepairType.Armor:
                    switch (sizeType)
                    {
                        case SizeType.Large:
                            _spRenderer.sprite = GlobalData.armorSprites[(int)ItemNode.SizeType.Large];
                            break;
                        case SizeType.Medium:
                            _spRenderer.sprite = GlobalData.armorSprites[(int)ItemNode.SizeType.Medium];
                            break;
                        case SizeType.Small:
                            _spRenderer.sprite = GlobalData.armorSprites[(int)ItemNode.SizeType.Small];
                            break;
                    }
                    break;
            }
        }

        void ScrapImageChange()
        {
            switch (sizeType)
            {
                case SizeType.Large:
                    _spRenderer.sprite = GlobalData.scrapSprites[(int)ItemNode.SizeType.Large];
                    break;
                case SizeType.Medium:
                    _spRenderer.sprite = GlobalData.scrapSprites[(int)ItemNode.SizeType.Medium];
                    break;
                case SizeType.Small:
                    _spRenderer.sprite = GlobalData.scrapSprites[(int)ItemNode.SizeType.Small];
                    break;
            }
        }
    }
}