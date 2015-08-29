using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(ItemCollectionBase))]
    public partial class CollectionPopulator : MonoBehaviour
    {

        public InventoryItemBase[] items = new InventoryItemBase[0];
        public bool fireAddItemEvents = false;

        public void Start()
        {
            var col = GetComponent<ItemCollectionBase>();
            if (col == null)
            {
                Debug.LogError("CollectionPopulator can only be used on a collection.", transform);
                return;
            }

            foreach (var item in items)
            {
                col.AddItem(item.IsInstanceObject() ? item : Instantiate<InventoryItemBase>(item), null, true, fireAddItemEvents);
            }
        }
    }
}