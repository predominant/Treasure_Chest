using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem.Models;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    public partial class InventoryCurrencyUI : MonoBehaviour
    {
        public ItemCollectionBase collection;

        [SerializeField]
        private uint _currencyID;
        public InventoryCurrency currency
        {
            get
            {
                return ItemManager.instance.currencies.FirstOrDefault(o => o.ID == _currencyID);
            }
        }

        public InventoryCurrencyUIElement currencyUIElement; 

        public virtual void Start()
        {
            currencyUIElement.Reset();

            if (collection != null)
            {
                collection.OnCurrencyChanged += CurrencyChanged;
                currencyUIElement.Repaint(collection.currenciesContainer.GetCurrency(_currencyID));
            }
        }

        private void CurrencyChanged(float amountBefore, InventoryCurrencyLookup lookup)
        {
            if(lookup._currencyID == _currencyID)
                currencyUIElement.Repaint(lookup);
        }
    }
}
