using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    //[Serializable]
    public partial class InventoryCurrencyLookupContainer
    {
        public List<InventoryCurrencyLookup> lookups { get; protected set; }
        
        public void AddCurrencyLookup(InventoryCurrencyLookup lookup)
        {
            lookup.parentContainer = this;
            lookups.Add(lookup);
        }

        public void RemoveCurrencyLookup(InventoryCurrencyLookup lookup)
        {
            lookups.Remove(lookup);
        }

        public InventoryCurrencyLookupContainer(bool loadAllCurrencies)
        {
            lookups = new List<InventoryCurrencyLookup>(ItemManager.instance.currencies.Length);

            if (loadAllCurrencies)
            {
                LoadCurrencyLookups();
            }
        }

        public InventoryCurrencyLookup GetCurrency(uint currencyID)
        {
            return lookups.FirstOrDefault(o => o._currencyID == currencyID);
        }

        private void LoadCurrencyLookups()
        {
            foreach (var currency in ItemManager.instance.currencies)
            {
                lookups.Add(new InventoryCurrencyLookup(currency) { parentContainer = this });
            }
        }
    }
}