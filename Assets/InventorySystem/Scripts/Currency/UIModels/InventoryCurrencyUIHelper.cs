using UnityEngine;
using System.Collections;

using Devdog.InventorySystem;

namespace Devdog.InventorySystem.UI
{
    using UnityEngine.UI;

    public class InventoryCurrencyUIHelper : MonoBehaviour
    {
        public uint currencyID;
        public bool allowCurrencyConversions = true;

        public ItemCollectionBase toCollection;
        public Button button;


        public void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    TriggerAddCurrencyToCollection(toCollection);
                });
            }
        }

        public void TriggerAddCurrencyToCollection(ItemCollectionBase collection)
        {
            InventoryManager.instance.intValDialog.ShowDialog(transform, "Amount", "", 1, 9999, value =>
            {
                // Yes callback
                if (InventoryManager.instance.inventory.CanRemoveCurrency((float)value, currencyID, allowCurrencyConversions))
                {
                    InventoryManager.instance.inventory.RemoveCurrency(value, currencyID, allowCurrencyConversions);
                    toCollection.AddCurrency(value, currencyID);
                }

            }, value =>
            {
                // No callback

            }
            );
        }
    }
}
