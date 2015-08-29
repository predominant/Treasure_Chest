using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public partial class InventorySerializationUtility
    {

        public static InventoryItemReferenceSaveLookup[] GetCollectionReferenceLookups(ItemCollectionBase collection)
        {
            var l = new InventoryItemReferenceSaveLookup[collection.items.Length];
            for (int i = 0; i < collection.items.Length; i++)
            {
                if (collection.items[i].item == null)
                    l[i] = new InventoryItemReferenceSaveLookup(-1, 0, string.Empty);
                else
                    l[i] = new InventoryItemReferenceSaveLookup((int)collection.items[i].item.ID, collection.items[i].item.currentStackSize, collection.items[i].item.itemCollection.collectionName);
            }

            return l;
        }

        public static InventoryItemSaveLookup[] GetCollectionLookups(ItemCollectionBase collection)
        {
            var l = new InventoryItemSaveLookup[collection.items.Length];
            for (int i = 0; i < collection.items.Length; i++)
            {
                if (collection.items[i].item == null)
                    l[i] = new InventoryItemSaveLookup(-1, 0);
                else
                    l[i] = new InventoryItemSaveLookup((int)collection.items[i].item.ID, collection.items[i].item.currentStackSize);                
            }

            return l;
        }
    }
}
