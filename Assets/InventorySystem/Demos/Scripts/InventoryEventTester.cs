using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    public partial class InventoryEventTester : MonoBehaviour
    {

        [InventoryRequired]
        public InventoryUI inventory;
        
        public void Awake()
        {
            inventory.OnAddedItem += (items, amount, cameFromCollection) =>
            {
                foreach (var item in items)
                {
                    Debug.Log(inventory.collectionName + " - " + item.name + " stored at slot: " + item.index);
                }
            };
            inventory.OnRemovedItem += (item, id, slot, amount) =>
            {
                Debug.Log(inventory.collectionName + " - " + " : removed item");
            };
            inventory.OnSwappedItems += (collection, slot, toCollection, toSlot) =>
            {
                Debug.Log(inventory.collectionName + " - " + " : swapped items");
            };
            inventory.OnUsedItem += (item, id, slot, amount) =>
            {
                Debug.Log(inventory.collectionName + " - " + " : use item from collection");
            };
        }
    }
}
