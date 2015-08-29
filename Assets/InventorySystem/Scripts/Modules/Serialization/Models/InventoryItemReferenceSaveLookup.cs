using System;

namespace Devdog.InventorySystem.Models
{
    public class InventoryItemReferenceSaveLookup : InventoryItemSaveLookup
    {
        public string referenceOfCollection;

        public InventoryItemReferenceSaveLookup()
            : this(-1, 0, string.Empty)
        { }

        public InventoryItemReferenceSaveLookup(int itemID, uint amount, string collectionName)
        {
            this.itemID = itemID;
            this.amount = amount;
            this.referenceOfCollection = collectionName;
        }
    }
}