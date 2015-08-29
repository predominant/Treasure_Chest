#if UFPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [RequireComponent(typeof(ObjectTriggererItemUFPS))]
    public partial class UnitTypeUFPSInventoryItem : UFPSInventoryItemBase
    {
        public vp_UnitType unitType;
        public uint unitAmount = 1;
        public AudioClip pickupSound;

        public override uint currentStackSize
        {
            get { return unitAmount; }
            set { unitAmount = value; }
        }

        public bool addDirectlyToWeapon = true;

        public override string name
        {
            get
            {
                if (useUFPSItemData && unitType != null)
                    return unitType.DisplayName;

                return base.name;
            }
            set { base.name = value; }
        }

        public override string description
        {
            get
            {
                if (useUFPSItemData && unitType != null)
                    return unitType.Description;
                
                return base.description;
            }
            set { base.description = value; }
        }

        protected override void Awake()
        {
            base.Awake();

        }

        public void Start()
        {
            OnEquipped += to => SetAmmo();
            OnUnEquipped += SetAmmo;

            OnUnstackedItem += (slot, amount) =>
            {
                ((UnitTypeUFPSInventoryItem) itemCollection[slot].item).Start();
            };
            //eventHandler.Register(this);
        }
        
        public void OnDestroy()
        {
            //eventHandler.Unregister(this);            
        }



        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
#if UFPS_MULTIPLAYER

            //var dropObj = base.Drop(location, rotation);
            var dropPos = GetDropPosition(location, rotation);
            NotifyItemDropped(null);

            //gameObject.SetActive(false);
            vp_MPPickupManager.Instance.photonView.RPC("InventoryDroppedObject", PhotonTargets.AllBuffered, (int)ID, objectTriggererItemUfps.ID, (int)unitAmount, dropPos, rotation);

            return null;

#else

            return base.Drop(location, rotation);

#endif
        }


        //protected virtual void OnStop_Attack()
        //{
        //    uint counter = 0;
        //    foreach (var item in itemCollection.items)
        //    {
        //        var i = item.item as UnitTypeUFPSInventoryItem;
        //        if (i != null && i.unitType == unitType)
        //        {
        //            // It's ammo!
        //            counter += i.currentStackSize;
        //        }
        //    }

        //    currentStackSize = counter;
        //    itemCollection[index].Repaint();
        //}

        //private void RemovedItemCollection(InventoryItemBase item, uint itemid, uint slot, uint amount)
        //{
        //    SetAmmo(unitType as vp_UnitBankType);
        //}

        //private void AddedItemCollection(IEnumerable<InventoryItemBase> item, uint slot, uint amount)
        //{
        //    SetAmmo(unitType as vp_UnitBankType);
        //}

        protected virtual void SetAmmo()
        {
            if (InventoryManager.IsEquipToCollection(itemCollection) == false)
            {
                // Item is not in equip collection
                ufpsInventory.TryRemoveUnits(unitType, 9999); // Remove all, then figure out how many we have
                return;
            }

            ufpsInventory.TryRemoveUnits(unitType, 9999); // Remove all, then figure out how many we have
            uint bulletCount = 0;
            foreach (var item in itemCollection.items)
            {
                var i = item.item as UnitTypeUFPSInventoryItem;
                if (i != null && i.unitType == unitType)
                {
                    // It's ammo!
                    bulletCount += i.currentStackSize;
                    //item.Repaint();
                    break; // Currently only supporting 1 stack of same ammo type
                }
            }

            ufpsInventory.TryGiveUnits(unitType, (int)bulletCount);
            //eventHandler.AddItem.Try(new object[] { bankType.Unit, (int)bulletCount });
        }


        //protected virtual bool AddToUFPSAmmo()
        //{
        //    SetAmmo();
        //    return true;
        //    //return eventHandler.AddItem.Try(new object[] { unitType, (int)currentStackSize });
        //}

        //protected virtual bool RemoveFromUFPSAmmo()
        //{
        //    SetAmmo();
        //    return true;
        //    //return eventHandler.RemoveItem.Try(new object[] { unitType, (int)currentStackSize });
        //    //return true;
        //    //eventHandler.CurrentWeaponClipCount.Set(0);
        //    //eventHandler.CurrentWeaponAmmoCount.Set(0);
        //}

        //public override int Use()
        //{
        //    if (addDirectlyToWeapon)
        //        return -1;

        //    int used = base.Use();
        //    if (used < 0)
        //        return used;

        //    // Un-Equipping
        //    bool added = AddToUFPSAmmo();
        //    if (added)
        //    {
        //        currentStackSize = 0; // TODO: Fix this, if UFPS accepts less units...
        //        NotifyItemUsed(currentStackSize, true);
        //        return (int)currentStackSize;
        //    }

        //    return 0;
        //}

        public override bool PickupItem()
        {
            currentStackSize = unitAmount;
            if (addDirectlyToWeapon)
            {
                // Add bullets directly to weapon
                //unitType.Space 
                SetAmmo();
                //if (added)
                //{
                    if (pickupSound != null)
                        InventoryUtility.AudioPlayOneShot(pickupSound);

                    Destroy(gameObject); // Get rid of object
                    return true;
                //}
            }
            else
            {
                bool pickedup = base.PickupItem(); // Add to inventory instead.
                if (pickedup)
                {
                    if (pickupSound != null)
                        InventoryUtility.AudioPlayOneShot(pickupSound);

                    return true;
                }
            }
            
            return false;
        }
    }
}

#endif