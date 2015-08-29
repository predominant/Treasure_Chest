#if UFPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [RequireComponent(typeof(ObjectTriggererItemUFPS))]
    public partial class EquippableUFPSInventoryItem : UFPSInventoryItemBase
    {
        public vp_ItemType itemType;


        public uint currentClipCount { get; set; }
        //public override uint currentStackSize
        //{
        //    get { return currentClipCount; }
        //    set { currentClipCount = value; }
        //}

        public override string name
        {
            get
            {
                if (useUFPSItemData && itemType != null)
                    return itemType.DisplayName;
                else
                    return base.name;
            }
            set { base.name = value; }
        }

        public override string description
        {
            get
            {
                if (useUFPSItemData && itemType != null)
                    return itemType.Description;
                else
                    return base.description;
            }
            set { base.description = value; }
        }


        protected ItemCollectionBase tempCollection { get; set; }

        protected override void Awake()
        {
            base.Awake();

            currentClipCount = 0;
            OnEquipped += to =>
            {
                tempCollection = itemCollection;

                objectTriggererItemUfps.TryGiveToPlayer(InventoryPlayerManager.instance.currentPlayer.GetComponentInChildren<Collider>(), (int)currentClipCount);
                
                eventHandler.Register(this); // Enable UFPS events
            };
            OnUnEquipped += () =>
            {
                //eventHandler.RemoveItem.Try(new object[] { itemType });
                ufpsInventory.TryRemoveItem(itemType, 0);
                currentClipCount = (uint)eventHandler.CurrentWeaponAmmoCount.Get();

                tempCollection = itemCollection;
                eventHandler.Unregister(this); // Disable UFPS events            
            };
        }

        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
#if UFPS_MULTIPLAYER

            //var dropObj = base.Drop(location, rotation);
            var dropPos = GetDropPosition(location, rotation);
            NotifyItemDropped(null);

            //gameObject.SetActive(false);
            vp_MPPickupManager.Instance.photonView.RPC("InventoryDroppedObject", PhotonTargets.AllBuffered, (int)ID, objectTriggererItemUfps.ID, (int)currentClipCount, dropPos, rotation);

            return null;

#else

            return base.Drop(location, rotation);

#endif
        }


        //// UFPS EVENT
        protected virtual void OnStop_Reload()
        {
            gameObject.SetActive(true);
            StartCoroutine(TestUpdateAmmoCount());
        }

        public override bool PickupItem()
        {
            bool pickedUp = base.PickupItem();
            if (pickedUp)
                transform.position = Vector3.zero; // Reset position to avoid the user from looting it twice when reloading (reloading temp. enables the item)

            return pickedUp;
        }

        protected IEnumerator TestUpdateAmmoCount()
        {
            yield return new WaitForFixedUpdate();

            UpdateAmmoCount();
            gameObject.SetActive(false);
        }

        protected virtual void UpdateAmmoCount()
        {
            var bankType = itemType as vp_UnitBankType;
            if (bankType != null)
            {
                int count = ufpsInventory.GetUnitCount(bankType.Unit);
                foreach (var item in itemCollection.items)
                {
                    var i = item.item as UnitTypeUFPSInventoryItem;
                    if (i != null && i.unitType == bankType.Unit)
                    {
                        // It's ammo!
                        i.currentStackSize = (uint)count;
                        if (item.item.currentStackSize == 0)
                        {
                            Destroy(item.item.gameObject);
                            item.item = null;
                        }
                        item.Repaint();
                        break; // Currently only supporting 1 stack of same ammo type
                    }
                }
            }
        }

        //public override LinkedList<InfoBoxUI.Row[]> GetInfo()
        //{
        //    var basic = base.GetInfo();
        //    basic.Remove(basic.First.Next);
        //    //basic.AddAfter(basic.First, new InfoBoxUI.Row[]
        //    //{
        //    //    new InfoBoxUI.Row("Ammo", )
        //    //});


        //    return basic;
        //}
    }
}

#endif