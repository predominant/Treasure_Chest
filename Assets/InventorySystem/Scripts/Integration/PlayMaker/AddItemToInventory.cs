#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{

    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Adds an item in the inventory, and if you have multiple inventories it will select the best inventory for the item.")]
    public class AddItemToInventory : FsmStateAction
    {
        //public ItemCollectionBase collection;
        public InventoryItemBase item;
        public FsmInt amount = 1;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            item.currentStackSize = (uint)amount.Value;
            InventoryManager.AddItem(item);
            Finish();
        }
    }
}

#endif