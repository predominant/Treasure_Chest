using System;
using UnityEngine;


namespace Devdog.InventorySystem.Models
{
    using System.Linq;

    [System.Serializable]
	public partial class InventoryCurrency
	{
        public uint ID;
		public string singleName = "";
        public string pluralName = "";

        public string description = "";

	    public Sprite icon;

        [Tooltip("How should the string be shown?\n{0} = The amount\n{1} = The currency name")]
        public string stringFormat = "{0} {3}";


        public InventoryCurrencyConversionLookup[] currencyConversions = new InventoryCurrencyConversionLookup[0];

        /// <summary>
        /// True if we can we get 0.1 gold (fraction), when false only ints are allowed.
        /// </summary>
        public bool allowFractions = true;
        

        /// <summary>
        /// Usefull when you want to "cap" a currency.
        /// For example in your game contains copper, silver and gold. When copper reaches 100 it can be converted to 1 silver.
        /// </summary>
        public bool autoConvertOnMax = false;
        public float autoConvertOnMaxAmount = 1000f;
        public uint autoConvertOnMaxCurrencyID;
        public InventoryCurrency autoConvertOnMaxCurrency
        {
            get
            {
                return ItemManager.instance.currencies.FirstOrDefault(o => o.ID == autoConvertOnMaxCurrencyID);
            }
        }

        public bool autoConvertFractions = true;
        public uint autoConvertFractionsToCurrencyID;
        public InventoryCurrency autoConvertFractionsToCurrency
        {
            get
            {
                return ItemManager.instance.currencies.FirstOrDefault(o => o.ID == autoConvertFractionsToCurrencyID);
            }
        }


        /// <summary>
        /// Convert this currency to the amount given ID.
        /// </summary>
        /// <param name="currencyID"></param>
        /// <returns></returns>
        public float ConvertTo(float amount, uint currencyID)
        {
            foreach (var conversion in currencyConversions)
            {
                if (conversion.currencyID == currencyID)
                {
                    return amount * conversion.factor;
                }
            }

            Debug.LogWarning("Conversion not possible no conversion found with currencyID " + currencyID);
            return 0.0f;
        }

        public float ConvertTo(float amount, InventoryCurrency currency)
        {
            return ConvertTo(amount, currency.ID);
        }



        public string GetFormattedString(float amount, float minAmount, float maxAmount, string overrideFormat = "")
        {
            try
            {
                return string.Format(overrideFormat == "" ? stringFormat : overrideFormat, amount, minAmount, maxAmount, amount >= -1.0f - float.Epsilon && amount <= 1.0f + float.Epsilon ? singleName : pluralName);
            }
            catch (Exception)
            { }

            return "(Formatting not valid)";
        }
	}
}