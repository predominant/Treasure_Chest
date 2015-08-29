using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    using System.Linq;

    /// <summary>
    /// This is used to represent gold as an item, once the item is picked up gold will be added to the inventory collection.
    /// </summary>
    public partial class CurrencyInventoryItem : InventoryItemBase
    {

        [InventoryStat]
        public float amount;


        [SerializeField]
        private uint _currencyID;
        public InventoryCurrency currencyType
        {
            get
            {
                return ItemManager.instance.currencies.FirstOrDefault(o => o.ID == _currencyID);
            }
        }


        public override bool PickupItem ()
        {
            InventoryManager.AddCurrency(amount, _currencyID);

            if(this.IsInstanceObject())
                Destroy (gameObject); // Don't need to store gold objects
            
            return true;
        }

        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var info = base.GetInfo();
            info.Clear();

            info.AddLast(new InventoryItemInfoRow[]{
                new InventoryItemInfoRow("Amount", currencyType.GetFormattedString(amount, 0f, float.MaxValue))
            });

            return info;
        }

        public override int Use()
        {
            return -2; // Can't use gold
        }
    }
}