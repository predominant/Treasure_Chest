using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryItemProperty
    {
        public enum ActionEffect
        {
            /// <summary>
            /// Add to the bonus, increases the maximum
            /// </summary>
            Add,

            ///// <summary>
            ///// Add to the maximum value
            ///// </summary>
            //IncreaseMax,

            /// <summary>
            /// Restore the value (for example consumables, when eating an apple, restore the health).
            /// </summary>
            Restore
        }


        [HideInInspector]
        public int ID;

        public string name;
        public string category = "Default";
        public string value;

        public bool showInUI = true;
        [Tooltip("How the value is shown.\n{0} = Current amount\n{1} = Max amount")]
        public string valueStringFormat = "{0}";

        public bool useInStats = true;
        public Color color = Color.white;

        /// <summary>
        /// The base value is the start value of this property.
        /// </summary>
        [Tooltip("The base value is the start value of this property")]
        public float baseValue;


        public float maxValue = 100.0f;
        
        /// <summary>
        /// (1 = value * 1.0f, 0.1f = value * 0.1f so 10%).
        /// </summary>
        public bool isFactor = false;

        //public bool increaseMax = false; // Increase max or add to?
        public ActionEffect actionEffect = ActionEffect.Restore;
        
        public int intValue
        {
            get
            {
                int v = 0;
                int.TryParse(value, out v);

                return v;
            }
        }

        public float floatValue
        {
            get
            {
                float v = 0.0f;
                float.TryParse(value, out v);

                return v;
            }
        }

        public bool isSingleValue
        {
            get
            {
                float v;
                return float.TryParse(value, out v);
            }
        }


        public string stringValue
        {
            get
            {
                return value;
            }
        }

        public bool boolValue
        {
            get
            {
                return bool.Parse(value);
            }
        }

        public string GetFormattedString(float currentValue)
        {
            try
            {
                return string.Format(valueStringFormat, currentValue, maxValue, name);
            }
            catch (Exception)
            { }

            return "(Formatting not valid)";
        }
    }
}