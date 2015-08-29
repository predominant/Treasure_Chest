using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// Used to represent a bag that extend a collection.
    /// </summary>
    public partial class BagInventoryItem : InventoryItemBase
    {
        [InventoryStat]
        public uint extendInventoryBySlots = 4;
        public AudioClip playOnEquip;

        //public bool isEquipped { get; protected set; }

        public override System.Collections.Generic.LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var list = base.GetInfo();

            list.AddFirst(new []
            {
                new InventoryItemInfoRow("Extra slots", extendInventoryBySlots.ToString())
            });

            return list;
        }

        public override int Use()
        {
            int used = base.Use();
            if(used < 0)
                return used; // Item cannot be used

            var prevCollection = itemCollection;
            uint prevIndex = index;

            bool added = InventoryManager.instance.inventory.inventoryExtenderCollection.AddItemAndRemove(this);
            if (added)
            {
                //Equip();
                NotifyItemUsed(1, false);
                prevCollection.NotifyItemUsed(this, ID, prevIndex, 1);
                return 1;
            }

            return -2;
        }

        public bool Equip()
        {
            // Used from some collection, equip
            bool added = InventoryManager.instance.inventory.AddSlots(extendInventoryBySlots);
            if (added)
            {
                if (playOnEquip)
                    InventoryUtility.AudioPlayOneShot(playOnEquip);
            
                return true;
            }

            return false;
        }

        public bool CanUnEquip()
        {
            return InventoryManager.instance.inventory.CanRemoveSlots(extendInventoryBySlots);
        }

        public bool UnEquip()
        {
            return InventoryManager.instance.inventory.RemoveSlots(extendInventoryBySlots);
        }

    }
}