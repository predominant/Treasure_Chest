using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventorySystem.Models
{
    [Serializable]
    public struct InventoryCurrencyUIElement
    {
        [Header("Options")]
        [SerializeField]
        private bool overrideStringFormat;

        [SerializeField]
        private string overrideStringFormatString;


        [Header("Audio & Visuals")]
        [SerializeField]
        private Text amount;

        [SerializeField]
        private Image icon;

        

        public void Repaint(InventoryCurrencyLookup lookup)
        {
            if (amount != null)
                amount.text = lookup.GetFormattedString(1.0f, overrideStringFormat ? overrideStringFormatString : "");

            if (icon != null)
                icon.sprite = lookup.currency.icon;

        }

        public void Reset()
        {
            if (amount != null)
                amount.text = "0";

        }
    }
}
