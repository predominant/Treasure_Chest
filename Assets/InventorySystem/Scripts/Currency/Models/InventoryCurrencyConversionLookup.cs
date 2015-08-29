using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem.Models
{
    [Serializable]
    public class InventoryCurrencyConversionLookup
    {

        public float factor = 1.0f;

        public uint currencyID;
        public InventoryCurrency currency
        {
            get
            {
                return ItemManager.instance.currencies.First(o => o.ID == currencyID);
            }
        }

        public bool useInAutoConversion = false;


    }
}
