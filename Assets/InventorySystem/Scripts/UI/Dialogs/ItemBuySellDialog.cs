using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Devdog.InventorySystem.Dialogs
{
    public enum ItemBuySellDialogAction
    {
        Selling,
        Buying,
        BuyingBack
    }


    public partial class ItemBuySellDialog : ItemIntValDialog
    {
        public UnityEngine.UI.Text price;
        protected ItemBuySellDialogAction action;
        protected VendorTriggerer vendor;

        [Header("Audio & Visuals")]
        public Color affordableColor = Color.white;
        public Color unAffordableColor = Color.red;
    

        public override void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            base.ShowDialog(caller, title, description, minValue, maxValue, yesCallback, noCallback);

            inputField.onValueChange.AddListener((string result) =>
            {
                int amount = GetInputValue(); // Let's trust Unity on this...

                string formattedText;
                float finalPrice;
                uint finalCurrencyID;
                GetTransactionInfo(amount, out formattedText, out finalPrice, out finalCurrencyID);

                price.text = formattedText;
                SetPriceColor(finalPrice, finalCurrencyID);

            });
            inputField.onValueChange.Invoke(inputField.text); // Hit the event on start
        }

        private void SetPriceColor(float finalPrice, uint currencyID)
        {
            if (action == ItemBuySellDialogAction.Buying || action == ItemBuySellDialogAction.BuyingBack)
            {
                if (InventoryManager.CanRemoveCurrency(finalPrice, currencyID, true))
                    price.color = affordableColor;
                else
                    price.color = unAffordableColor;
            }
            else
                price.color = affordableColor;
        }

        private void GetTransactionInfo(int amount, out string formattedText, out float finalPrice, out uint finalCurrencyID)
        {
            if (action == ItemBuySellDialogAction.Buying)
            {
                formattedText = inventoryItem.buyPrice.GetFormattedString(amount * vendor.buyPriceFactor);
                finalPrice = vendor.GetBuyPrice(inventoryItem, (uint)amount);
                finalCurrencyID = inventoryItem.buyPrice.currency.ID;
                return;
            }

            if (action == ItemBuySellDialogAction.Selling)
            {
                formattedText = inventoryItem.sellPrice.GetFormattedString(amount * vendor.sellPriceFactor);
                finalPrice = vendor.GetSellPrice(inventoryItem, (uint)amount);
                finalCurrencyID = inventoryItem.sellPrice.currency.ID;
                return;
            }

            if (action == ItemBuySellDialogAction.BuyingBack)
            {
                formattedText = inventoryItem.sellPrice.GetFormattedString(amount * vendor.buyBackPriceFactor);
                finalPrice = vendor.GetBuyBackPrice(inventoryItem, (uint)amount);
                finalCurrencyID = inventoryItem.sellPrice.currency.ID;
                return;
            }

            formattedText = "";
            finalPrice = 0f;
            finalCurrencyID = 0;
        }

        public override void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, InventoryItemBase item, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            inventoryItem = item;
            ShowDialog(caller, string.Format(string.Format(title, item.name, item.description)), string.Format(description, item.name, item.description), minValue, maxValue, yesCallback, noCallback);
        }

        public virtual void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, InventoryItemBase item, ItemBuySellDialogAction action, VendorTriggerer vendor, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            // Don't call base class going directly to this.ShowDialog()
            inventoryItem = item;
            this.action = action;
            this.vendor = vendor;
            ShowDialog(caller, string.Format(string.Format(title, item.name, item.description)), string.Format(description, item.name, item.description), minValue, maxValue, yesCallback, noCallback);
        }
    }
}
