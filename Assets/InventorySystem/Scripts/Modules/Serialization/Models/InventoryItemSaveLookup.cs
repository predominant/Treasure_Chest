using System;

namespace Devdog.InventorySystem.Models
{
    public class InventoryItemSaveLookup
    {
        /// <summary>
        /// ID is -1 if no item is in the given slot.
        /// </summary>
        public int itemID;
        public uint amount;

        public InventoryItemSaveLookup()
            : this(-1, 0)
        { }

        public InventoryItemSaveLookup(int itemID, uint amount)
        {
            this.itemID = itemID;
            this.amount = amount;
        }
    }
}
