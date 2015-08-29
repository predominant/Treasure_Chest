#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Set items in a collection, replaces the old items that were in the collection before it.")]
    public class SetItemsCollection : FsmStateAction
    {
        public InventoryItemBase[] items;
        public ItemCollectionBase collection;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            collection.SetItems(items, true);
            Finish();
        }
    }
}

#endif