#if PLAYMAKER

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem
{
    using System.Linq;

    using HutongGames.PlayMaker;

    /// <summary>
    /// Relays all events to PlayMaker's FSM
    /// </summary>
    public partial class ItemCollectionBase
    {
        [NonSerialized]
        private PlayMakerFSM[] playMakerFSMs = new PlayMakerFSM[0];

        ///// <inheritdoc />
        //partial void Start3()
        //{
        //    playMakerFSMs = GetComponents<PlayMakerFSM>();

        //    if (playMakerFSMs.Length > 0)
        //    {
        //        OnAddedItem += OnAddedItemPlayMaker;
        //        OnAddedItemCollectionFull += OnAddedItemCollectionFullPlayMaker;
        //        OnDroppedItem += OnDroppedItemPlayMaker;
        //        OnRemovedItem += OnRemovedItemPlayMaker;
        //        OnRemovedReference += OnRemovedReferencePlayMaker;
        //        OnResized += OnResizedPlayMaker;
        //        OnSorted += OnSortedPlayMaker;
        //        OnSwappedItems += OnSwappedItemsPlayMaker;
        //        OnUnstackedItem += OnUnstackedItemPlayMaker;
        //        OnUnstackedItemCollectionFull += OnUnstackedItemCollectionFullPlayMaker;
        //        OnUsedItem += OnUsedItemPlayMaker;
        //        OnUsedReference += OnUsedReferencePlayMaker;
        //    }
        //}


        //partial void OnDestroy3()
        //{
        //    if (playMakerFSMs.Length > 0)
        //    {
        //        OnAddedItem -= OnAddedItemPlayMaker;
        //        OnAddedItemCollectionFull -= OnAddedItemCollectionFullPlayMaker;
        //        OnDroppedItem -= OnDroppedItemPlayMaker;
        //        OnRemovedItem -= OnRemovedItemPlayMaker;
        //        OnRemovedReference -= OnRemovedReferencePlayMaker;
        //        OnResized -= OnResizedPlayMaker;
        //        OnSorted -= OnSortedPlayMaker;
        //        OnSwappedItems -= OnSwappedItemsPlayMaker;
        //        OnUnstackedItem -= OnUnstackedItemPlayMaker;
        //        OnUnstackedItemCollectionFull -= OnUnstackedItemCollectionFullPlayMaker;
        //        OnUsedItem -= OnUsedItemPlayMaker;
        //        OnUsedReference -= OnUsedReferencePlayMaker;
        //    }
        //}

        private void SendEvent(string name)
        {
            foreach (var fsm in playMakerFSMs)
            {
                fsm.Fsm.Event(name);
            }

            Debug.Log("Send event " + name + " to playmaker");
        }

        private void OnAddedItemPlayMaker(IEnumerable<InventoryItemBase> items, uint amount, bool cameFromCollection)
        {
            foreach (var fsm in playMakerFSMs)
            {
                var obj = fsm.Fsm.Variables.FindFsmObject("INV_PRO_ADDED_ITEM");
                if (obj != null)
                    obj.Value = items.First();

                fsm.Fsm.Event("INV_PRO_ON_ADDED_ITEM");
            }
        }

        private void OnUsedReferencePlayMaker(InventoryItemBase actualItem, uint itemID, uint referenceSlot, uint amountUsed)
        {
            SendEvent("INV_PRO_ON_USED_REFERENCE");
        }

        private void OnUsedItemPlayMaker(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            SendEvent("INV_PRO_ON_USED_ITEM_FROM_COLLECTION");
        }

        private void OnUnstackedItemCollectionFullPlayMaker(uint slot)
        {

        }

        private void OnUnstackedItemPlayMaker(ItemCollectionBase fromCollection, uint startSlot, ItemCollectionBase toCollection, uint endSlot, uint amount)
        {

        }

        private void OnSwappedItemsPlayMaker(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot)
        {

        }

        private void OnSortedPlayMaker()
        {

        }

        private void OnResizedPlayMaker(uint fromSize, uint toSize)
        {

        }

        private void OnRemovedReferencePlayMaker(InventoryItemBase item, uint slot)
        {

        }

        private void OnRemovedItemPlayMaker(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {

        }

        private void OnDroppedItemPlayMaker(InventoryItemBase item, uint slot, GameObject droppedObj)
        {

        }

        private void OnAddedItemCollectionFullPlayMaker(InventoryItemBase item, bool cameFromCollection)
        {

        }
    }
}

#endif