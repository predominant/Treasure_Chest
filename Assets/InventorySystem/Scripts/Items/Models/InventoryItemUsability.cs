using System;

namespace Devdog.InventorySystem.Models
{
    public partial class InventoryItemUsability
    {
        public delegate void UseItemCallback(InventoryItemBase item);

        public string actionName;
        public UseItemCallback useItemCallback;
        public bool isActive;

        public InventoryItemUsability(string actionName, UseItemCallback useItemCallback)
        {
            this.actionName = actionName;
            this.useItemCallback = useItemCallback;
        }
    }
}