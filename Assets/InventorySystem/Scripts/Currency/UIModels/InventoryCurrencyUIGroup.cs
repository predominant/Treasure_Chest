using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem.Models;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    public partial class InventoryCurrencyUIGroup : MonoBehaviour
    {
        private InventoryCurrencyUI[] _currencies;

        private InventoryCurrencyUI[] currencies
        {
            get
            {
                if (_currencies == null)
                    _currencies = GetComponentsInChildren<InventoryCurrencyUI>(true);

                return _currencies;
            }
        }

        public void Repaint(InventoryCurrencyLookup amount)
        {
            // TODO: Maybe add fractions conversion to lower values, as done in main system (1.12 silver is 1 silver and 12 copper).

            foreach (var currencyUI in currencies)
            {
                if (currencyUI.currency == null)
                {
                    Debug.LogWarning("Empty currencyUIElement in group ", currencyUI.transform);
                    continue;
                }

                if (currencyUI.currency.ID == amount._currencyID)
                {
                    currencyUI.currencyUIElement.Repaint(amount);
                }
                else
                {
                    currencyUI.currencyUIElement.Reset();
                }
            }
        }
    }
}
