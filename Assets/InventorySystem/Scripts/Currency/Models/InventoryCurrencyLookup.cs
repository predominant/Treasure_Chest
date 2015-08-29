using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    [Serializable]
    public partial class InventoryCurrencyLookup
    {
        #region Delegates & Events

        public delegate void CurrencyChanged(float before, float after);

        public event CurrencyChanged OnCurrencyChanged;

        #endregion


        public InventoryCurrencyLookupContainer parentContainer { get; set; }
        


        [SerializeField]
        public uint _currencyID;
        public virtual InventoryCurrency currency
        {
            get
            {
                return ItemManager.instance.currencies.FirstOrDefault(o => o.ID == _currencyID);
            }
        }

        //public InventoryCurrency currency;
        
        [SerializeField]
        private float _amount;
        public virtual float amount
        {
            get { return _amount; }
            set
            {
                float before = _amount;
                //if (currency.allowFractions == false)
                //{
                //    value = Mathf.Floor(value);
                //}

                _amount = value;

                TryAutoRefillCurrency();
                TryAutoConvertCurrency();
                TryAutoConvertFractions();

                if (before != _amount)
                    NotifyOnCurrencyChanged(before, _amount);
            }
        }

        public InventoryCurrencyLookup(InventoryCurrency currency)
            : this(currency, 0)
        { }

        public InventoryCurrencyLookup(InventoryCurrency currency, float startAmount)
        {
            _currencyID = currency.ID;
            _amount = startAmount;
        }

        private void TryAutoRefillCurrency()
        {
            if (parentContainer == null)
                return;

            // Value wen't below 0, grab currency from the next one up the chain.
            foreach (var lookup in parentContainer.lookups)
            {
                if (_amount < 0)
                {
                    // Find currency that converts to our current currency. (this = copper, find silver -> converts to copper)
                    var convertable = lookup.currency.currencyConversions.FirstOrDefault(o => o.currencyID == _currencyID && o.useInAutoConversion);
                    if (convertable != null)
                    {
                        float amountToGrab = Mathf.Abs(_amount) / convertable.factor;
                        _amount = 0f; // Reset this, grabbing from next currency.
                        lookup.amount -= amountToGrab; // Example: abs(-10) = 10 * 0.01f = 0.1f silver. 
                        break;
                    }
                }
            }
        }

        protected virtual void TryAutoConvertFractions()
        {
            if (parentContainer == null)
                return;

            if (currency.autoConvertFractions)
            {
                double fraction = _amount % 1;
                fraction = System.Math.Round(fraction, 4); // Because of so much conversion float point accuracy is a problem, so round it.
                if (fraction > 0.00001f)
                {
                    var convertTo = parentContainer.GetCurrency(currency.autoConvertFractionsToCurrencyID);
                    if (convertTo == null)
                    {
                        Debug.LogWarning("Can't convert fractions, container doesn't accept currency type.");
                        return;
                    }

                    var convertToConversionList = currency.currencyConversions.FirstOrDefault(o => o.currencyID == currency.autoConvertFractionsToCurrencyID);
                    if (convertToConversionList == null)
                    {
                        Debug.LogError("Can't convert to this type, conversion is not set in conversion list.");
                        return;
                    }

                    _amount = Mathf.Floor(_amount); // Remove our fraction.
                    convertTo.amount += (float)(fraction * convertToConversionList.factor); // Works recursive, calls the next currency that can convert it down.
                }
            }
            else
            {
                if (currency.allowFractions == false)
                {
                    //_amount = Mathf.Round(_amount);
                }
            }
        }

        protected virtual void TryAutoConvertCurrency()
        {
            if (parentContainer == null)
                return;

            if (currency.autoConvertOnMax)
            {
                if (_amount >= currency.autoConvertOnMaxAmount)
                {
                    var convertTo = parentContainer.GetCurrency(currency.autoConvertOnMaxCurrencyID);
                    if (convertTo == null)
                    {
                        Debug.LogWarning("Can't convert currency, container doesn't accept currency type.");
                        return;
                    }

                    var convertToConversionList = currency.currencyConversions.FirstOrDefault(o => o.currencyID == currency.autoConvertOnMaxCurrencyID);
                    if (convertToConversionList == null)
                    {
                        Debug.LogError("Can't convert to this type, conversion is not set in conversion list.");
                        return;
                    }

                    // How many times can we extract it without going below 0? ( 26 / 5 = 5.2 ( floored = 5x ))
                    int canExtractTimes = Mathf.FloorToInt(_amount / currency.autoConvertOnMaxAmount);
                    _amount -= currency.autoConvertOnMaxAmount * canExtractTimes;

                    convertTo.amount += ((canExtractTimes * currency.autoConvertOnMaxAmount) * convertToConversionList.factor); // Works recursive
                }
            }
        }

        private void NotifyOnCurrencyChanged(float before, float after)
        {
            if (OnCurrencyChanged != null)
                OnCurrencyChanged(before, after);
        }



        public virtual float CanAddCount()
        {
            return float.MaxValue - amount;
        }

        public virtual float CanRemoveCount(bool allowCurrencyConversions)
        {
            if (allowCurrencyConversions == false)
                return amount;

            if (parentContainer == null)
            {
                Debug.LogWarning("Can't convert currency because this lookup is not in a container. (can't grab other currency amounts)");    
                return amount;
            }

            float totalAmount = amount;
            var dict = new Dictionary<uint, float>();
            _GetConversionFactor(dict, currency, 1.0f);

            foreach (var tuple in dict)
            {
                var lookup = parentContainer.lookups.First(o => o._currencyID == tuple.Key);
                totalAmount += lookup.amount / tuple.Value; // How much the higher currency is worth in this currency ( gold worth in copper )
            }
            
            return totalAmount;
        }

        /// <summary>
        /// Grab the total factor of converting from a currency to another.
        /// For example copper to silver = 0.01 > silver to gold = 0.01 * 0.01 = 0.0001
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private float _GetConversionFactor(Dictionary<uint, float> dict, InventoryCurrency fromCurrency, float factor)
        {
            foreach (var lookup in parentContainer.lookups)
            {
                // Find currency that converts to our current currency.
                var convertToCurrent = lookup.currency.currencyConversions.FirstOrDefault(o => o.currencyID == fromCurrency.ID && o.useInAutoConversion);
                if (convertToCurrent != null)
                {
                    if (dict.ContainsKey(lookup._currencyID))
                    {
                        //Debug.Log("need update?  factor: " + factor + " convertToCurrent factor: " + convertToCurrent.factor + " at " + lookup.currency.pluralName);
                    }
                    else
                    {
                        // One conversion higher
                        factor /= convertToCurrent.factor; // Division because we're going the other way...

                        dict.Add(lookup._currencyID, factor);
                        return _GetConversionFactor(dict, lookup.currency, factor);
                    }
                }
            }


            return factor;
        }

        public virtual bool CanAdd(float addAmount)
        {
            return CanAddCount() >= addAmount;
        }

        public virtual bool CanRemove(float removeAmount, bool allowCurrencyConversions)
        {
            return CanRemoveCount(allowCurrencyConversions) >= removeAmount;
        }

        /// <summary>
        /// Get the formatted string of this currency lookup.
        /// Adds the current amount and the maxAmount to string.Format.
        /// </summary>
        /// <param name="amountMultiplier">A multiplier to increase the amount. For example a boot costs 5 gold, I sold it 10 times -> a multiplier of 10</param>
        /// <returns></returns>
        public virtual string GetFormattedString(float amountMultiplier = 1.0f, string overrideFormat = "")
        {
            if (currency == null)
                return "";

            return currency.GetFormattedString(amount * amountMultiplier, float.MinValue, float.MaxValue, overrideFormat);
        }
    }
}