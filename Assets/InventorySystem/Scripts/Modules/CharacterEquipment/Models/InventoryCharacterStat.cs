using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    /// <summary>
    /// Holds the final stat as well as all the affectors changing the item's behaviour.
    /// </summary>
    public partial class InventoryCharacterStat
    {
        /// <summary>
        /// Name of this stat
        /// </summary>
        public string statName { get; set; }

        public string valueStringFormat { get; set; }

        /// <summary>
        /// The factor by which the value is multiplied.
        /// </summary>
        public float currentFactor { get; protected set; }

        /// <summary>
        /// The factor by which the max value is multiplied.
        /// </summary>
        public float currentFactorMax { get; protected set; }

        /// <summary>
        /// The current value of this stat. (baseValue + currentValueRaw) * currentFactor * currentFactorMaxValue
        /// </summary>
        public float currentValue
        {
            get
            {
                return Mathf.Clamp(currentValueNotClamped, float.MinValue, maxValue);
            }
        }

        protected float currentValueNotClamped
        {
            get
            {
                return (baseValue + currentValueRaw) * currentFactor;
            }
        }


        /// <summary>
        /// The current value without the factor and base value applied.
        /// </summary>
        public float currentValueRaw { get; set; }


        public float currentValueNormalized
        {
            get { return currentValue / maxValue; }
        }



        public float maxValue
        {
            get
            {
                return maxValueRaw * currentFactorMax;
            }
        }

        private float maxValueRaw;
        

        /// <summary>
        /// The base / starting value of this property.
        /// </summary>
        public float baseValue { get; protected set; }

        /// <summary>
        /// Should the item be shown in the UI?
        /// </summary>
        public bool showInUI = true;


        public InventoryPlayer player { get; protected set; }


        public InventoryCharacterStat(InventoryPlayer player, string statName, string valueStringFormat, float baseValue, float maxValue, bool showInUI)
        {
            this.player = player;
            this.statName = statName;
            this.showInUI = showInUI;
            this.valueStringFormat = valueStringFormat;

            currentFactor = 1.0f;
            currentFactorMax = 1.0f;

            SetMaxValueRaw(maxValue, false);
            SetBaseValue(baseValue, false);
        }

        public InventoryCharacterStat(InventoryPlayer player, InventoryItemProperty property)
            : this(player, property.name, property.valueStringFormat, property.baseValue, property.maxValue, property.showInUI)
        { }


        public virtual void Reset()
        {
            SetCurrentValueRaw(0, false);
        }

        protected void ClamClampCurrentValueRaw()
        {
            float over = currentValueNotClamped - maxValue;
            if (over > 0.0f)
            {
                currentValueRaw -= over; // "clamp" the currentValue raw, so that currentValueRaw + all other stats hit the maxValueRaw.
            }
        }

        public void SetCurrentValueRaw(float value, bool fireEvents = true)
        {
            currentValueRaw = value;
            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }

        public void ChangeCurrentValueRaw(float value, bool fireEvents = true)
        {
            SetCurrentValueRaw(currentValueRaw + value, fireEvents);
        }


        public void SetFactor(float value, bool fireEvents = true)
        {
            currentFactor = value;
            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }
        public void ChangeFactor(float value, bool fireEvents = true)
        {
            SetFactor(currentFactor + value, fireEvents);
        }

        public void SetFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            float prevMax = maxValue;
            currentFactorMax = value;
            if (andIncreaseCurrentValue)
            {
                float increase = maxValue - prevMax;
                ChangeCurrentValueRaw(increase, false); // Updating below..
            }

            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }

        public void ChangeFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            SetFactorMax(currentFactorMax + value, andIncreaseCurrentValue, fireEvents);
        }


        public void SetMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            float prevMax = maxValue;
            maxValueRaw = value;
            if (andIncreaseCurrentValue)
            {
                float increase = maxValue - prevMax;
                ChangeCurrentValueRaw(increase, false); // Updating below..
            }

            ClamClampCurrentValueRaw();


            if (fireEvents)
                NotifyCharacterCollection();
        }
        public void ChangeMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            SetMaxValueRaw(maxValueRaw + value, andIncreaseCurrentValue, fireEvents);
        }


        public void SetBaseValue(float value, bool fireEvents = true)
        {
            baseValue = value;
            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }
        public void ChangeBaseValue(float value, bool fireEvents = true)
        {
            SetBaseValue(baseValue + value, fireEvents);
        }
        

        private void NotifyCharacterCollection()
        {
            if (player != null && player.characterCollection != null)
                player.characterCollection.NotifyStatChanged(this);
        }



        //// Re-fills the stat to the max value
        //public void Restore()
        //{
        //    currentValueRaw = (maxValueRaw - baseValue);
        //}


        public override string ToString()
        {
            return string.Format(valueStringFormat, System.Math.Round(currentValue, 2), maxValue, statName);
        }
    }
}